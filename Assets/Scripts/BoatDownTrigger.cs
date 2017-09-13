using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatDownTrigger : MonoBehaviour {

	public BoatDownUnit BoatDownUnit;
	private bool isTriggered = false;

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {
			if (!isTriggered) {
				isTriggered = true;
				BoatDownUnit.PutBoatDown();
			}
			
		}
	}
}
