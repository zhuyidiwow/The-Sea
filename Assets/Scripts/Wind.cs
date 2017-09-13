using UnityEngine;

public static class Wind {
	public static float Force;
	public static Vector3 Direction;

	public static void SetDirection(Vector3 direction) {
		Direction = direction;
	}

	public static void SetForce(float windForce) {
		Force = windForce;
	}
}
