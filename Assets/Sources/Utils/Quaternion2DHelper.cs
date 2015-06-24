//
// Author: Artur Salyakhov
//

using UnityEngine;

/// <summary>
/// вспомогательные функции по работе с кватернионами в 2D
/// ускоряет работу за счет игнорирования координаты Z и всеми связанными с ней вычислениями
/// </summary>
public static class Quaternion2DHelper {
	private const float Deg2Rad = Mathf.Deg2Rad;
	private const float Half_Deg2Rad = Deg2Rad * 0.5f;
	private const float Rad2Deg = Mathf.Rad2Deg;
	private const float Double_Rad2Deg = Rad2Deg * 2f;

	public static Quaternion RotationWithDegrees(float angleInDegrees) {
		angleInDegrees *= Half_Deg2Rad;
		return new Quaternion(0f, 0f, Mathf.Sin(angleInDegrees), Mathf.Cos(angleInDegrees));
	}

	public static Quaternion RotationWithRadians(float angleInRadians) {
		angleInRadians *= 0.5f;
		return new Quaternion(0f, 0f, Mathf.Sin(angleInRadians), Mathf.Cos(angleInRadians));
	}

	public static Quaternion RotationWithDirection(Vector2 direction) {
		float angleInRadians = Mathf.Atan2(direction.y, direction.x) * 0.5f;
		return new Quaternion(0f, 0f, Mathf.Sin(angleInRadians), Mathf.Cos(angleInRadians));
	}

// TODO; протестить будет ли быстрее нормализовать вектор и из него брать синус и косинус
//	public static Quaternion RotationWithDirection(Vector3 direction) {
//		float length = Mathf.Sqrt(direction.x * direction.x + direction.y * direction.y);
//		return new Quaternion(0f, 0f, direction.y / length, direction.x / length);
//	}

	public static float DegreesFromRotation(Quaternion rotation) {
		return Mathf.Acos(rotation.w) * Double_Rad2Deg; 
	}

	public static float RadiansFromRotation(Quaternion rotation) {
		return Mathf.Acos(rotation.w) * 2f;
	}

	public static Vector3 DirectionFromRotation(Quaternion rotation) {
		float a = rotation.z * 2f;
		float b = 1f - rotation.z * a;
		float c = rotation.w * a;
		return new Vector3(b, c, 0f);
	}

	public static Vector3 RotateVector(Quaternion rotation, Vector3 v) {
		float a = rotation.z * 2f;
		float b = 1f - rotation.z * a;
		float c = rotation.w * a;
		return new Vector3(b * v.x - c * v.y, c * v.x + b * v.y, v.z);
	}
}