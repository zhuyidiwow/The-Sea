using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTrigger : MonoBehaviour {

	public GameObject Target;
	private bool isTriggered = false;

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {
			if (!isTriggered) {
				isTriggered = true;
				Target.SetActive(!Target.activeSelf);
			}
		}
	}
}
