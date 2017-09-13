using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainDrop : MonoBehaviour {

	public float SpeedCap;
	public GameObject Splash;
	private Rigidbody rb;

	private void Start() {
		rb = GetComponent<Rigidbody>();
	}

	private void FixedUpdate() {
		ReceiveWindForce();
		CapSpeed();
		if (transform.position.y < -3f) {
			Destroy(this.gameObject);
		}
	}

	private void CapSpeed() {
		if (rb.velocity.magnitude > SpeedCap) {
			rb.velocity = rb.velocity.normalized * SpeedCap;
		}
	}
	
	private void ReceiveWindForce() {
		rb.AddForce(Wind.Force * Wind.Direction.normalized / 10f, ForceMode.Force);
	}

//	private void OnTriggerEnter(Collider other) {
//		if (other.CompareTag("Water Surface")) {
//			//if (Random.Range(0f, 1f) > 0.8f) {
//				GameObject newSplash = Instantiate(Splash, transform.position, new Quaternion());
//				newSplash.transform.localScale *= 0.3f;
//				Destroy(newSplash, 2f);
//				Destroy(this.gameObject);
//			//}
//		}
//	}
}
