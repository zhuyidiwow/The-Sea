using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindmillRotater : MonoBehaviour {

	public float RotationSpeed;
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.forward, RotationSpeed * Time.deltaTime, Space.Self);
	}
}
