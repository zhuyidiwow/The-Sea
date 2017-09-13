using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockThrowerTrigger : MonoBehaviour {

	public Rock Rock;
	private bool isTriggered = false;

	private void Start() {
		Rock.gameObject.SetActive(false);
	}

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {
			if (!isTriggered) {
				isTriggered = true;
				Rock.gameObject.SetActive(true);
				Rock.GetThorwnOut();
			}
			
		}
	}
}
