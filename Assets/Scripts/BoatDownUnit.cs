using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatDownUnit : MonoBehaviour {

	public Friendly Boat;
	public GameObject Text;
	public Transform[] Positions;
	public float Duration;
	public float BoatSpeedCap = 20f;

	public void PutBoatDown() {
		StartCoroutine(PutBoatDownCoroutine());
	}

	private IEnumerator PutBoatDownCoroutine() {
		float startTime = Time.time;
		while (Time.time - startTime < Duration) {
			Boat.transform.position = Vector3.Lerp(Positions[0].position, Positions[1].position, (Time.time - startTime) / Duration);
			Boat.transform.rotation = Quaternion.Lerp(Positions[0].rotation, Positions[1].rotation, (Time.time - startTime) / Duration);
			yield return new WaitForSeconds(Time.deltaTime/2f);
		}
		Boat.UnFreeze();
		Boat.SetSpeedCap(BoatSpeedCap);
		if (Text != null) {
			Text.SetActive(true);
		}
	}

}
