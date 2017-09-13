using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour {

	public Lightening Lightening;
	private LightData[] LightsData;
	private Light light;

	private void Start() {
		LightsData = new LightData[4];
		LightsData[0] = new LightData(new Color(56f/255f, 56f/255f, 56f/255f, 1f), 1.43f);
		LightsData[1] = new LightData(new Color(254f/255f, 255f/255f, 204f/255f, 1f), 1.2f);
		LightsData[2] = new LightData(new Color(255f/255f, 255f/255f, 255f/255f, 1f), 1.6f);
		LightsData[3] = new LightData(new Color(42f/255f, 55f/255f, 66f/255f, 1f), 1.29f);
		light = GetComponent<Light>();
		light.color = LightsData[0].color;
		light.intensity = LightsData[0].intensity;
	}

	public void EnterScene2() {
		StartCoroutine(Scene2Coroutine());
	}

	private IEnumerator Scene2Coroutine() {
		float startTime = Time.time;
		float duration = 3f;
		while (Time.time - startTime < duration) {
			light.color = Color.Lerp(LightsData[0].color, LightsData[1].color, (Time.time - startTime) / duration);
			light.intensity = LightsData[0].intensity +
			                  (LightsData[1].intensity - LightsData[0].intensity) * ((Time.time - startTime) / duration);
			yield return new WaitForSeconds(Time.deltaTime / 2f);
		}
	}

	public void EnterScene3() {
		StartCoroutine(Scene3Coroutine());
	}

	private IEnumerator Scene3Coroutine() {
		float startTime = Time.time;
		float duration = 3f;
		while (Time.time - startTime < duration) {
			light.color = Color.Lerp(LightsData[1].color, LightsData[2].color, (Time.time - startTime) / duration);
			light.intensity = LightsData[1].intensity +
			                  (LightsData[2].intensity - LightsData[1].intensity) * ((Time.time - startTime) / duration);
			yield return new WaitForSeconds(Time.deltaTime / 2f);
		}
	}
	
	public void EnterScene4() {
		StartCoroutine(Scene4Coroutine());
		StartCoroutine(LighteningCoroutine());
	}

	private IEnumerator Scene4Coroutine() {
		float startTime = Time.time;
		float duration = 3f;
		while (Time.time - startTime < duration) {
			light.color = Color.Lerp(LightsData[2].color, LightsData[3].color, (Time.time - startTime) / duration);
			light.intensity = LightsData[2].intensity +
			                  (LightsData[3].intensity - LightsData[2].intensity) * ((Time.time - startTime) / duration);
			yield return new WaitForSeconds(Time.deltaTime / 2f);
		}
	}

	private IEnumerator LighteningCoroutine() {
		float minInterval = 5f;
		float maxInterval = 8f;
		float interval;
		yield return new WaitForSeconds(5f);
		while (true) {
			interval = Random.Range(minInterval, maxInterval);
			Lightening newLightening = Instantiate(Lightening, Vector3.zero, new Quaternion());
			newLightening.transform.RotateAround(transform.position, Vector3.right, 45f);
			newLightening.Lighten(0.03f, 0.05f, Random.Range(3, 5));
			
			yield return new WaitForSeconds(interval);
		}
	}
	
}

class LightData {
	public Color color;
	public float intensity;

	public LightData(Color color, float intensity) {
		this.color = color;
		this.intensity = intensity;
	}
}