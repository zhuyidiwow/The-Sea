using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallBackTrigger : MonoBehaviour {

	public GameObject Rain;
	public int CallBackIndex;
	private bool isTriggered = false;

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {
			if (!isTriggered) {
				isTriggered = true;
				switch (CallBackIndex) {
					case 0:
						EnterScene2();
						break;
					case 1:
						EnterScene3();
						break;
					case 2:
						EnterScene4();
						break;
					case 3:
						SetPlayerHealth();
						break;
					case 4:
						GetterUtility.GetCameraManager().ChangePosition(30f, 15f, 70f);
						break;
					case 5:
						StartFinale();
						break;
					default:
						break;
				}
			}
			
		}
	}

	public void EnterScene2() {
		GetterUtility.GetLightManager().EnterScene2();
	}

	public void EnterScene3() {
		GetterUtility.GetLightManager().EnterScene3();
		GetterUtility.GetCameraManager().ChangePosition(50f, 56f, 0f);
	}

	public void EnterScene4() {
		GetterUtility.GetLightManager().EnterScene4();
		GetterUtility.GetCameraManager().ChangePosition(60f, 40f, 0f);
		GetterUtility.GetPlayer().InRain();
		GetterUtility.GetPlayer().Health = 100f;
		Rain.SetActive(true);
	}

	private void SetPlayerHealth() {
		Player player = GetterUtility.GetPlayer();
		if (player.Health >= 20f) {
			player.Health = 15f;
		}
	}

	private void StartFinale() {
		GetterUtility.GetCameraManager().StartFinale();
	}
}
