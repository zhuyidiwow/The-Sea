using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingWood : MonoBehaviour {
	public int Health = 1;
	public bool IsBroken = false;
	private AudioSource audioSource;

	private void Start() {
		audioSource = GetComponent<AudioSource>();
	}

	public void Break() {
		Health -= 1;
		if (Health <= 0) {
			if (!IsBroken) {
				audioSource.Play();
				foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>()) {
					rb.isKinematic = false;
					Destroy(rb.gameObject, 2f);
				}
				IsBroken = true;
			}
		}
	}
	
	public void BreakWithForce() {
		if (!IsBroken) {
			foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>()) {
				rb.isKinematic = false;
				rb.AddForce(Vector3.up * 2000f);
			}

			foreach (MeshCollider meshCollider in GetComponentsInChildren<MeshCollider>()) {
				meshCollider.enabled = false;
			}
			IsBroken = true;
		}
	}
}
