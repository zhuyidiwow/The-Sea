using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSurface : MonoBehaviour {

	public float animPeriod = 1f;
	public float MaxZ;

	private float elapsedTime;
	private float normalZScale;
	private float w;
	
	void Start() {
		normalZScale = transform.localScale.z;
		w = Mathf.PI * 2 / animPeriod;
		StartCoroutine(WaveCoroutine());
	}
	
	IEnumerator WaveCoroutine() {
		elapsedTime = 0f;
		while (true) {
			float zScale = normalZScale + (1f + Mathf.Sin(w * elapsedTime)) * (MaxZ - normalZScale);
			transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, zScale);
			
			yield return new WaitForSeconds(Time.deltaTime);
			elapsedTime += Time.deltaTime;
			if (elapsedTime > animPeriod) {
				elapsedTime -= animPeriod;
			}
		}
	}
}
