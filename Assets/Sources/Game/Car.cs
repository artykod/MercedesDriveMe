using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Car : MonoBehaviour {

	public enum CarType {
		Red,
		Blue,
	}

	[SerializeField]
	private CarType type = CarType.Red;
	[SerializeField]
	private Color lineColor = Color.white;

	private static Dictionary<CarType, Car> allCars = new Dictionary<CarType, Car>();

	private Rigidbody2D body = null;
	private new Transform transform = null;
	private Vector2 lastTarget = Vector2.zero;
	private bool goToTarget = false;
	private bool canMove = false;
	private LineDrawer lineDrawer = null;

	private int lapsDone = 0;
	private float lapTime = 0f;
	private bool raceDone = false;

	private CircleCollider2D[] checkpoints = null;
	private bool[] checkpointsEnabled = null;

	private CubicCurve botPathCurve = null;
	private float botPathEvaluateTime = 0f;

	public static Car CarByType(CarType carType) {
		Car car = null;
		allCars.TryGetValue(carType, out car);
		return car;
	}

	public static Car[] AllCars() {
		return allCars.Values.ToArray();
	}

	public CarType Type {
		get {
			return type;
		}
	}

	public bool IsBot {
		get;
		private set;
	}

	public bool RaceDone {
		get {
			return raceDone;
		}
	}

	public Color LineColor {
		get {
			return lineColor;
		}
	}

	public LineDrawer LineDrawer {
		get {
			return lineDrawer;
		}
		set {
			lineDrawer = value;
		}
	}

	public bool GoToTarget {
		get {
			return goToTarget;
		}
	}

	public string CurrentLapTime() {
		return LapTime();
	}

	public int CurrentLap() {
		return lapsDone >= Level.TotalLaps() ? Level.TotalLaps() - 1 : lapsDone;
	}

	public float InactiveCooldown {
		get;
		private set;
	}

	public bool IsActive {
		get;
		private set;
	}

	private string LapTime() {
		float time = Level.LevelStarted ? Time.time - lapTime : 0f;

		if (raceDone) {
			time = lapTime;
		}

		int lapMinutes = (int)(time / 60f);
		int lapSeconds = (int)(time - lapMinutes * 60f);
		int lapMillis = (int)((time - lapSeconds - lapMinutes * 60f) * 100f);

		return string.Format("{0:00}:{1:00}:{2:000}", lapMinutes, lapSeconds, lapMillis);
	}

	private void CheckpointCollideCheck(Collider2D checkpoint) {

		if (raceDone) {
			return;
		}

		for (int i = 0; i < checkpoints.Length; i++) {
			if (checkpoints[i] == checkpoint) {
				if (i == 0 || !checkpointsEnabled[i - 1]) {
					checkpointsEnabled[i] = false;

					if (i == checkpoints.Length - 1) {
						lapsDone++;

						Debug.LogFormat("Lap done: {0} time: {1}", lapsDone, LapTime());
						
						for (int ch = 0; ch < checkpointsEnabled.Length; ch++) {
							checkpointsEnabled[ch] = true;
						}

						if (lapsDone >= Level.TotalLaps()) {
							Debug.Log("Race done, car = " + gameObject.name, this);
							raceDone = true;
							
							TargetCompleted();

							lapTime = Time.time - lapTime;

							if (GameCore.LastWinner == GameCore.PlayersTypes.Unknown) {
								if (IsBot) {
									GameCore.LastWinner = GameCore.PlayersTypes.Bot;
								} else {
									if (type == CarType.Red) {
										GameCore.LastWinner = GameCore.PlayersTypes.Player1;
									} else {
										GameCore.LastWinner = GameCore.PlayersTypes.Player2;
									}
								}
							}

							return;
						}

						//lapTime = Time.time;

						return;
					}
				}
			}
		}
	}

	private void Awake() {
		raceDone = false;
		IsActive = true;

		InactiveCooldown = Config.PlayerInactiveCooldown;

		lineColor = type == CarType.Red ? Config.RaceLine1Color : Config.RaceLine2Color;

		if (GameCore.GameMode == GameCore.GameModes.OnePlayer && type == CarType.Blue) {
			Destroy(gameObject);
			return;
		} else if (GameCore.GameMode == GameCore.GameModes.OnePlayerWithBot && type == CarType.Blue) {
			IsBot = true;
		}

		allCars[type] = this;

		transform = base.transform;
		body = GetComponent<Rigidbody2D>();

		lastTarget = transform.position;

		canMove = false;
		StartCoroutine(WaitLevelStartPosition());
	}

	private IEnumerator WaitLevelStartPosition() {

		Transform startPoint = null;

		if (LevelsManager.EMPTY_LEVEL) {
			yield return null;
		} else {
			while ((startPoint = (type == CarType.Red ? Level.StartPoint1 : Level.StartPoint2)) == null) {
				yield return null;
			}
		}
		if (startPoint != null) {
			transform.position = startPoint.position;
			transform.rotation = startPoint.rotation;
		}
		lastTarget = transform.position;

		while (!Level.LevelStarted) {
			yield return null;
		}

		botPathCurve = Level.BotPathCurve;

		checkpoints = Level.Checkpoints;
		checkpointsEnabled = new bool[checkpoints.Length];
		for (int i = 0; i < checkpointsEnabled.Length; i++) {
			checkpointsEnabled[i] = true;
		}

		lapTime = Time.time;
		canMove = true;
	}

	private void Update() {
		if (!canMove) {
			return;
		}

		if (lineDrawer != null) {
			if (lineDrawer.HasPoints) {
				lastTarget = lineDrawer.FirstPoint;
				goToTarget = true;
			}
		} else {
			if (!IsBot) {
				goToTarget = false;
			}
		}

		if (IsBot && botPathCurve != null) {
			if (raceDone) {
				//
			} else {
				lastTarget = botPathCurve.Evaluate(botPathEvaluateTime);
				goToTarget = true;
			}
        }

        Vector2 target = lastTarget;
		Vector2 rotation = transform.rotation * Vector2.up;
		Vector2 dir = target - new Vector2(transform.position.x, transform.position.y);
		float distance = dir.magnitude;

		if (goToTarget && Mathf.Abs(distance) > 0.0001f) {

			float maxSpeed = IsBot ? Config.RaceCarSpeedBot : Config.RaceCarSpeedPlayer;

			float velocityMagnitude = Mathf.Min(maxSpeed, distance * maxSpeed);
			float dot = Vector2.Dot(rotation, (dir / distance) * velocityMagnitude);
			Vector2 proj = rotation * dot;

			Vector2 velocity = Vector2.Lerp(body.velocity, proj, 0.1f);
			velocity = rotation * Vector2.Dot(rotation, velocity);
			if (!float.IsNaN(velocity.x) && !float.IsNaN(velocity.y)) {
				body.velocity = velocity;
			} else {
				body.velocity = Vector2.zero;
			}

			float k = Mathf.Min(Mathf.Pow(distance, 4f), 1f);
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion2DHelper.RotationWithDirection(new Vector2(dir.y, -dir.x)), 0.2f * k);

			InactiveCooldown = Config.PlayerInactiveCooldown;
			IsActive = true;
		}

		body.velocity = rotation * Vector2.Dot(rotation, body.velocity);

		body.velocity *= 0.96f;
		body.angularVelocity = 0f;

		if (distance < 0.7f) {

			if (IsBot) {
				botPathEvaluateTime += 0.005f;
				if (botPathEvaluateTime > 1f) {
					botPathEvaluateTime = 0f;
				}
            }

			if (lineDrawer != null && lineDrawer.HasPoints) {
				if (!lineDrawer.RemoveFirst() || lineDrawer.PointsCount < 3) {
					TargetCompleted();
                }

				//lineDrawer.FirstPoint = transform.position;
			} else {
				if (!IsBot) {
					goToTarget = false;
				}
			}
		}

		if (IsActive && !Level.RaceDone && !goToTarget && Level.LevelStarted && !IsBot) {
			InactiveCooldown -= Time.deltaTime;
		}

		// время простоя игрока вышло, он проиграл
		if (IsActive && !Level.RaceDone && InactiveCooldown < 0f) {
			TargetCompleted();
			IsActive = false;
        }

		/*if (body.velocity.magnitude < 0.01f && Mathf.Abs(body.angularVelocity) < 0.01f) {
			if (!IsBot && LineDrawer != null && LineDrawer.HasPoints) {
				TargetCompleted();
            }
			LineDrawer = null;
		}*/
	}

	private void TargetCompleted() {
		lastTarget = transform.position;
		body.velocity *= 0.75f;

		goToTarget = false;

		if (lineDrawer != null && !lineDrawer.IsUnderControl) {
			lineDrawer.Clear();
			lineDrawer = null;
		}
	}

	private void OnTriggerEnter2D(Collider2D collider) {
		if (!canMove) {
			return;
		}

		if (collider.gameObject.layer == LayerMask.NameToLayer("Checkpoint")) {
			CheckpointCollideCheck(collider);
		}
	}
}
