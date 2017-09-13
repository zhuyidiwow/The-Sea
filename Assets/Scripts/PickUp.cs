﻿using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PickUp : MonoBehaviour {

	public Transform LowestPoint;
	public GameObject Energy;
	public float EnergyAmount;
	public bool CanBePickedUp = true;
	
	private Rigidbody rb;
	private AudioSource audioSource;

	void Start() {
		rb = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
	}
	
	public void ReceiveBuoyancy(float buoyancyMagnitude) {
		rb.AddForce(buoyancyMagnitude * Vector3.up, ForceMode.Force);	
	}

	public Vector3 GetLowestPoint() {
		return LowestPoint.position;
	}

	public void GetPickedUp() {
		audioSource.Play();
		CanBePickedUp = false;
		Destroy(Energy);
		foreach (MeshCollider mesh in transform.GetComponentsInChildren<MeshCollider>()) {
			mesh.enabled = false;
		}
	}
}
