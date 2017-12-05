using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friendly : MonoBehaviour {
	// Each action is a vector
	// X is for action
		// 1f - Left Turn
		// 2f - Right Turn
	// Y is for duration
	public Vector2[] ActionList;
	public float ActionWaitTime;
	
	// public fields
	public GameObject Sail;
	public GameObject Mast;
	public GameObject RotationReference;
	public GameObject Particles;
	public ParticleSystem Flame;
	public bool CanBreakWood = false;

	public GameObject PickUp;
	
	[Tooltip("Degree per second")]
	public float SailRotationSpeed;
	
	// serialized private fileds
	[SerializeField] private Transform lowestPoint;
	[SerializeField] private float horizontalSpeedCap;
	
	[SerializeField] private float speedIncreasePerEnergy;
	[SerializeField] private float rotationSpeedIncreasePerEnergy;
	[SerializeField] private float sailScaleIncreasePerEnergy;
	private float baseSpeed;
	private float baseRotationSpeed;
	private Vector3 baseSailScale;
	public bool IsSunk = false;
	
	[SerializeField] private float loseEnergyTime;
		
	// private fields
	private Rigidbody rb;
	private const float particleMovingRadius = 2.5f;
	private ParticleSystem.ForceOverLifetimeModule forceOverLifetimeModule;
	private float energy;
	
	void Start() {
		rb = GetComponent<Rigidbody>();
		baseSpeed = horizontalSpeedCap;
		baseSailScale = Sail.transform.localScale;
		baseRotationSpeed = SailRotationSpeed;
		rb.isKinematic = true;
		Particles.SetActive(false);
	}

	private void FixedUpdate() {
		if (!IsSunk) {
			ReceiveWindForce();
			CapHorizontalSpeed();
			// ChangeWindVaneDirection();
			UpdateWave();
			UpdateFlame();
		}
	}

	public void SetSpeedCap(float speedCap) {
		horizontalSpeedCap = speedCap;
		baseSpeed = horizontalSpeedCap;
	}
	
	public void UnFreeze() {
		rb.isKinematic = false;
		Particles.SetActive(true);
		StartActionsAfterSeconds();
	}
	
	public void StartActionsAfterSeconds() {
		StartCoroutine(ActionExecutionCoroutine());
	}
	
	private IEnumerator ActionExecutionCoroutine() {
		yield return new WaitForSeconds(ActionWaitTime);
		foreach (Vector2 action in ActionList) {
			if (action.x.Equals(1f)) {
				float startTime = Time.time;
				float duration = action.y;
				while ((Time.time - startTime) < duration) {
					RotateSailBy(-SailRotationSpeed * Time.deltaTime);
					yield return new WaitForSeconds(Time.deltaTime);
				}
			} else if (action.x.Equals(2f)) {
				float startTime = Time.time;
				float duration = action.y;
				while ((Time.time - startTime) < duration) {
					RotateSailBy(SailRotationSpeed * Time.deltaTime);
					yield return new WaitForSeconds(Time.deltaTime);
				}
			}
		}
	}
	
	public Vector3 GetLowestPoint() {
		return lowestPoint.position;
	}
	
	// called by water to provide buoyancy
	public void ReceiveBuoyancy(float buoyancyMagnitude) {
		rb.AddForce(buoyancyMagnitude * Vector3.up, ForceMode.Force);	
	}
	
	// receive any force other than buoyancy
	public void ReceiveForce(Vector3 force) {
		rb.AddForce(force, ForceMode.Force);
	}

	private void OnTriggerEnter(Collider other) {
		if (!IsSunk) {
			if (other.gameObject.CompareTag("Energy")) {
				PickUp pickUp = other.transform.parent.GetComponent<PickUp>();
				if (pickUp.CanBePickedUp) {
					PickUpEnergy(pickUp.EnergyAmount);
					pickUp.GetPickedUp();
				}
			}
		}
	}

	private void OnCollisionEnter(Collision other) {
		if (other.gameObject.CompareTag("Thorn")) {
			if (!IsSunk) {
				Sink();
				other.transform.parent.GetComponent<Thorn>().Sink();
			}
		}
		
		if (other.gameObject.CompareTag("Floating Wood")) {
			if (!IsSunk) {
				if (CanBreakWood) {
					other.transform.parent.gameObject.GetComponent<FloatingWood>().Break();
					Sink();
				} else {
					Sink();
				}
			}
		}
	}

	public void Sink() {
		IsSunk = true;
		rb.velocity = rb.velocity / 3f;
		StartCoroutine(SinkCoroutine());
		Particles.SetActive(false);
		Flame.Stop();
		Destroy(gameObject, 5f);
	}

	private IEnumerator SinkCoroutine() {
		yield return new WaitForSeconds(0.5f);
		Instantiate(PickUp, new Vector3(transform.position.x, 0.31f, transform.position.z), new Quaternion());
	}

	private void PickUpEnergy(float amount) {
		energy += amount;
		rb.velocity += Vector3.one.normalized * amount;
		UpdatePerformance();
		StartCoroutine(LoseEnergyCoroutine(amount, loseEnergyTime));
	}

	private void UpdatePerformance() {
		horizontalSpeedCap = baseSpeed + speedIncreasePerEnergy * energy;
		SailRotationSpeed = baseRotationSpeed + rotationSpeedIncreasePerEnergy * energy;
		Sail.transform.localScale = (1f + sailScaleIncreasePerEnergy * energy) * baseSailScale;
	}

	private IEnumerator LoseEnergyCoroutine(float amount, float time) {
		yield return new WaitForSeconds(time);
		energy -= amount;
		UpdatePerformance();
	}

	private void CapHorizontalSpeed() {
		Vector2 horizontalVelocity = new Vector2(rb.velocity.x, rb.velocity.z);
		if (horizontalVelocity.magnitude > horizontalSpeedCap) {
			rb.velocity = new Vector3(horizontalVelocity.normalized.x * horizontalSpeedCap, rb.velocity.y, horizontalVelocity.normalized.y * horizontalSpeedCap);
		}
	}
	
	private void RotateSailBy(float degree) {
		Sail.transform.RotateAround(RotationReference.transform.position, Mast.transform.up, degree);
	}

	private void ReceiveWindForce() {
		Vector3 direction = -Sail.transform.forward;
		float angle = Vector3.Angle(Wind.Direction, -Sail.transform.forward);
		float force = Wind.Force * Mathf.Cos(Mathf.Deg2Rad * angle);
		ReceiveForce(force * direction);

		// apply torque
		rb.AddForceAtPosition(force * direction / 20f, RotationReference.transform.position, ForceMode.Force);
	}

	private void UpdateWave() {
//		if (rb.velocity.magnitude < 5f) {
//			Particles.SetActive(false);
//			return;
//		}
//		Particles.SetActive(true);
		Particles.transform.position = transform.position - rb.velocity.normalized * particleMovingRadius;
		Particles.transform.position = new Vector3(Particles.transform.position.x, transform.position.y - 0.33f, Particles.transform.position.z);
		Particles.transform.LookAt(Particles.transform.position-rb.velocity.normalized);
	}

	private void UpdateFlame() {
		forceOverLifetimeModule = Flame.forceOverLifetime;
		forceOverLifetimeModule.x = new ParticleSystem.MinMaxCurve(rb.velocity.x * -1f / 5f);
		forceOverLifetimeModule.y = new ParticleSystem.MinMaxCurve(rb.velocity.y * -1f / 5f);
		forceOverLifetimeModule.z = new ParticleSystem.MinMaxCurve(rb.velocity.z * -1f / 5f);
	}
}
