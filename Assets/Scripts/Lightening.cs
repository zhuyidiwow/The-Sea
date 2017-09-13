using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightening : MonoBehaviour {

	private float minIntensity = 0f;
	private float maxIntensity = 1f;

	private Light light;
	
	public void Lighten(float minDuration, float maxDuration, int times) {
		light = GetComponent<Light>();
		StartCoroutine(LightenCoroutine(minDuration, maxDuration, times));
		GetterUtility.GetThunder().PlayThunderSound();
	}

	IEnumerator LightenCoroutine(float minDuration, float maxDuration, int times) {
		float startTime = Time.time;
		float duration = Random.Range(minDuration, maxDuration);

		for (int i = 0; i < times; i++) {
			while (Time.time - startTime < duration) {
				light.intensity = minIntensity + (maxIntensity - minIntensity) * (Time.time - startTime) / duration;
				yield return new WaitForSeconds(Time.deltaTime / 2f);
			}
			light.intensity = maxIntensity;
			yield return new WaitForSeconds(Time.deltaTime);

			startTime = Time.time;
			while (Time.time - startTime < duration) {
				light.intensity = maxIntensity + (minIntensity - maxIntensity) * (Time.time - startTime) / duration;
				yield return new WaitForSeconds(Time.deltaTime / 2f);
			}
			light.intensity = minIntensity;
			yield return new WaitForSeconds(Time.deltaTime);
			duration = Random.Range(minDuration, maxDuration);
			
		}
		
		Destroy(gameObject);
	}
}
