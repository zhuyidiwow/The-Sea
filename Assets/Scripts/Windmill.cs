using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Windmill : MonoBehaviour {

	private Quaternion originalQuaternion;
	
	// Use this for initialization
	void Start () {
		originalQuaternion = transform.rotation;
	}

	private void FixedUpdate() {
		FaceWind();
	}

	public void FaceWind() {
		float angleFromRightToWind = 0f;
		
		if (Wind.Direction.z > 0f) {
			angleFromRightToWind = Vector3.Angle(Vector3.right, Wind.Direction);
		} else if (Wind.Direction.z < 0f) {
			angleFromRightToWind = 360f - Vector3.Angle(Vector3.right, Wind.Direction);
		}

		transform.rotation = originalQuaternion; // reset rotation
		transform.RotateAround(transform.position, transform.up, -angleFromRightToWind);
	}
}