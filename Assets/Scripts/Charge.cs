using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Charge : MonoBehaviour {

	public Transform LowestPoint;
	public GameObject Arrow;
	public Transform[] Points;
	public Vector3 ChargeDirection;
	public float ChargeDistance = 50f;
	public float ChargeSpeed = 200f;
	public bool CanBePickedUp = true;
	
	private Rigidbody rb;

	void Start() {
		rb = GetComponent<Rigidbody>();
		ChargeDirection = (Points[1].position - Points[0].position).normalized;
	}
	
	public void ReceiveBuoyancy(float buoyancyMagnitude) {
		rb.AddForce(buoyancyMagnitude * Vector3.up, ForceMode.Force);	
	}

	public Vector3 GetLowestPoint() {
		return LowestPoint.position;
	}

	public void GetPickedUp() {
		CanBePickedUp = false;
		Destroy(Arrow);
		foreach (MeshCollider mesh in transform.GetComponentsInChildren<MeshCollider>()) {
			mesh.enabled = false;
		}
	}
}
