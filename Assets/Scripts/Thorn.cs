using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thorn : MonoBehaviour {

	public float Damage;
	public float HurtPlayerCoolDown;
	public bool CanHurtPlayer = true;
	public bool IsSunk = false;

	private Rigidbody rb;

	private void Start() {
		rb = GetComponent<Rigidbody>();
	}
	
	public void ReceiveBuoyancy(float buoyancyMagnitude) {
		rb.AddForce(buoyancyMagnitude * Vector3.up, ForceMode.Force);	
	}

	public void StartCoolDown() {
		CanHurtPlayer = false;
		StartCoroutine(CoolDownCoroutine());
	}

	private IEnumerator CoolDownCoroutine() {
		yield return new WaitForSeconds(HurtPlayerCoolDown);
	}

	public void Sink() {
		CanHurtPlayer = false;
		foreach (MeshCollider meshCollider in transform.GetComponentsInChildren<MeshCollider>()) {
			meshCollider.isTrigger = true;
		}
		StartCoroutine(SinkCoroutine());
	}

	private IEnumerator SinkCoroutine() {
		float startTime = Time.time;
		float duration = 3f;

		while (Time.time - startTime < duration) {
			transform.Translate(Vector3.down * Time.deltaTime * 10f, Space.World);
			yield return new WaitForSeconds(Time.deltaTime / 2f);
		}
	}
}
