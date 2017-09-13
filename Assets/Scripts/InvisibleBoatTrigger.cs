using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleBoatTrigger : MonoBehaviour {

	public BoatDownUnit[] BoatDownUnits;
	private bool isTriggered = false;

	void Awake() {
		foreach (BoatDownUnit boatDownUnit in BoatDownUnits) {
			boatDownUnit.gameObject.SetActive(false);
		}
	}
	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {
			if (!isTriggered) {
				isTriggered = true;
				foreach (BoatDownUnit boatDownUnit in BoatDownUnits) {
					boatDownUnit.gameObject.SetActive(true);
					boatDownUnit.PutBoatDown();
				}
			}
			
		}
	}
}
