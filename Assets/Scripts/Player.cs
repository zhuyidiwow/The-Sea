using System.Collections;
using System.Security.Cryptography;
using System.Security.Principal;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public class Player : MonoBehaviour {
	// public fields
	public float Health = 100f;
	public GameObject Sail;
	public GameObject Mast;
	public GameObject RotationReference;
	public GameObject Particles;
	public ParticleSystem Wave;
	public ParticleSystem Flame;
	
	// Scene 0
	public Transform[] StartPositions;
	
	[Tooltip("Degree per second")]
	public float SailRotationSpeed;
	public bool ShouldReceiveInput = true;
	public bool CanDestroyThings = false; 
	
	// serialized private fileds
	[SerializeField] private Transform lowestPoint;
	[SerializeField] private float horizontalSpeedCap;
	
	[SerializeField] private float speedIncreasePerEnergy;
	[SerializeField] private float rotationSpeedIncreasePerEnergy;
	[SerializeField] private float sailScaleIncreasePerEnergy;
	[SerializeField] private float flameScaleIcnerasePerEnergy;
	private float baseSpeed;
	private float baseRotationSpeed;
	private Vector3 baseSailScale;
	private Vector3 baseFlameScale;
	
	[SerializeField] private float loseEnergyTime;

	// flame things
	private readonly Color flameNormalColor = new Color(255f/255f, 246f/255f, 118f/255f, 1f);
	private readonly Color flameDangerColor = Color.red;
	private readonly float flameNormalLifeTime = 2f;
	private readonly float flameDangerLifeTime = 0.5f;
	private readonly float flameNormalEmissionSize = 30f;
	private readonly float flameDangerEmissionSize = 1f;
	private readonly float baseLifeTime = 1f;
			
	// private fields
	private Rigidbody rb;
	// private float sailRotationDegree = 0f; // clockwise from original position, which is perpendicular to the ship
	private const float particleMovingRadius = 2.5f;
	private float energy;
	private bool isCharging = false;

	private float lastChargeStartTime;
	private float lastChargeDuration;
	private Vector3 lastChargeDirection;
	private float lastChargeDistance;
	private float lastChargeSpeed;

	void Start() {
		rb = GetComponent<Rigidbody>();
		baseSpeed = horizontalSpeedCap;
		baseSailScale = Sail.transform.localScale;
		baseRotationSpeed = SailRotationSpeed;
		baseFlameScale = Flame.transform.localScale;
		Wave = Particles.GetComponent<ParticleSystem>();
		Wind.SetForce(5f);
		Wind.SetDirection(new Vector3(1f, 0f, 0f));
		Freeze();
	}

	public void Freeze() {
		rb.isKinematic = true;
		ShouldReceiveInput = false;
	}

	public void Scene0() {
		StartCoroutine(Scene0Coroutine());
	}

	IEnumerator Scene0Coroutine() {
		float startTime = Time.time;
		float duration = 2f;
		while (Time.time - startTime < duration) {
			transform.position = Vector3.Lerp(StartPositions[0].position, StartPositions[1].position, (Time.time - startTime) / duration);
			transform.rotation = Quaternion.Lerp(StartPositions[0].rotation, StartPositions[1].rotation, (Time.time - startTime) / duration);
			yield return new WaitForSeconds(Time.deltaTime/2f);
		}
		Unfreeze();
	}
	
	public void Unfreeze() {
		rb.isKinematic = false;
		ShouldReceiveInput = true;
	}

	private void FixedUpdate() {
		if (ShouldReceiveInput) {
			ReceiveInput();
		}
		
		ReceiveWindForce();
		
		if (isCharging) {
			rb.velocity = lastChargeDirection.normalized * lastChargeSpeed;
			if (Time.time - lastChargeStartTime > lastChargeDuration) {
				isCharging = false;
				Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Stone Panel"), false);
			}
		}
		
		if (!isCharging) {
			CapHorizontalSpeed();
			UpdateWave();
		}
		
		UpdateFlame();

		if (Mathf.Abs(transform.position.y - -0.24f) > 0.5f) {
			rb.drag = 1f;
		} else {
			rb.drag = 0f;
		}
	}

	public void Charge(Vector3 direction, float distance, float speed) {
		isCharging = true;
		lastChargeStartTime = Time.time;
		lastChargeDirection = direction;
		lastChargeDistance = distance;
		lastChargeSpeed = speed;
		lastChargeDuration = distance / speed;
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Stone Panel"), true);
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

	public void TakeDamage(float amount) {
		Health -= amount;
		UpdateFlameWithHealth();
	}

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag("Energy")) {
			PickUp pickUp = other.transform.parent.GetComponent<PickUp>();
			if (pickUp.CanBePickedUp) {
				PickUpEnergy(pickUp.EnergyAmount);
				pickUp.GetPickedUp();
			}
		}
		
		if (other.gameObject.CompareTag("Charge")) {
			Charge charge = other.transform.parent.GetComponent<Charge>();
			if (charge.CanBePickedUp) {
				Charge(charge.ChargeDirection, charge.ChargeDistance, charge.ChargeSpeed);
				charge.GetPickedUp();
			}
		}
	}

	private void OnCollisionEnter(Collision other) {
		if (other.gameObject.CompareTag("Thorn")) {
			Thorn thorn = other.transform.parent.GetComponent<Thorn>();
			if (thorn.CanHurtPlayer) {
				TakeDamage(thorn.Damage);
				thorn.StartCoolDown();
				thorn.Sink();
			}
		}

		if (other.gameObject.CompareTag("Floating Wood")) {
			if (Health > 20f) {
				FloatingWood floatingWood = other.transform.parent.GetComponent<FloatingWood>();
				if (!floatingWood.IsBroken) {
					TakeDamage(30f);
					floatingWood.Break();
				}
				
			}

		}
	}

	private void PickUpEnergy(float amount) {
		energy += amount;
		Health += amount * 10f;
		if (Health > 100f) Health = 100f;
		rb.velocity += Vector3.one.normalized * amount;
		UpdatePerformance();
		StartCoroutine(LoseEnergyCoroutine(amount, loseEnergyTime));
	}

	private void UpdatePerformance() {
		horizontalSpeedCap = baseSpeed + speedIncreasePerEnergy * energy;
		SailRotationSpeed = baseRotationSpeed + rotationSpeedIncreasePerEnergy * energy;
		Sail.transform.localScale = (1f + sailScaleIncreasePerEnergy * energy) * baseSailScale;
		Flame.transform.localScale = (1f + flameScaleIcnerasePerEnergy * energy) * baseFlameScale;
	}

	private IEnumerator LoseEnergyCoroutine(float amount, float time) {
		yield return new WaitForSeconds(time);
		energy -= amount;
		UpdatePerformance();
	}

	private void CapHorizontalSpeed() {
		horizontalSpeedCap = (0.5f + Health / 200f) * baseSpeed;
		Vector2 horizontalVelocity = new Vector2(rb.velocity.x, rb.velocity.z);
		if (horizontalVelocity.magnitude > horizontalSpeedCap) {
			rb.velocity = new Vector3(horizontalVelocity.normalized.x * horizontalSpeedCap, rb.velocity.y, horizontalVelocity.normalized.y * horizontalSpeedCap);
		}
	}

	private void ReceiveInput() {
		if (Input.GetKey(KeyCode.A)) {
			RotateSailBy(-SailRotationSpeed * Time.deltaTime);
		}

		if (Input.GetKey(KeyCode.D)) {
			RotateSailBy(SailRotationSpeed * Time.deltaTime);
		}

		//TODO: debug only code
		if (Input.GetKey(KeyCode.Keypad9)) {
			Wind.SetDirection(new Vector3(1f, 0f, 1f).normalized);
		}

		if (Input.GetKey(KeyCode.Keypad6)) {
			Wind.SetDirection(new Vector3(1f, 0f, 0f));
		}

		if (Input.GetKey(KeyCode.Keypad3)) {
			Wind.SetDirection(new Vector3(1f, 0f, -1f).normalized);
		}

		if (Input.GetKey(KeyCode.UpArrow)) {
			Wind.Force += Time.deltaTime * 2f;
		}

		if (Input.GetKey(KeyCode.DownArrow)) {
			Wind.Force -= Time.deltaTime * 2f;
		}

		if (Input.GetKeyDown(KeyCode.T)) {
			Charge(Vector3.right, 50f, 100f);
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

		var mainModule = Wave.main;
		mainModule.startLifetime = baseLifeTime * rb.velocity.magnitude / 15f;
	}

	private void UpdateFlame() {
		var forceOverLifetimeModule = Flame.forceOverLifetime;
		forceOverLifetimeModule.x = new ParticleSystem.MinMaxCurve(rb.velocity.x * -1f / 5f);
		forceOverLifetimeModule.y = new ParticleSystem.MinMaxCurve(rb.velocity.y * -1f / 5f);
		forceOverLifetimeModule.z = new ParticleSystem.MinMaxCurve(rb.velocity.z * -1f / 5f);
	}

	private void UpdateFlameWithHealth() {
		var mainModule = Flame.main;
		mainModule.startColor = new ParticleSystem.MinMaxGradient(Color.Lerp(flameNormalColor, flameDangerColor, 1f- Health/100f));
		mainModule.startLifetime = flameNormalLifeTime + (flameDangerLifeTime - flameNormalLifeTime) * (1f - Health / 100f);

		var emissionModule = Flame.emission;
		emissionModule.rateOverTime = flameNormalEmissionSize + (flameDangerEmissionSize - flameNormalEmissionSize) * (1f - Health / 100f);
	}

	public void InRain() {
		StartCoroutine(InRainCoroutine());
	}

	private IEnumerator InRainCoroutine() {
		yield return new WaitForSeconds(5f);
		float healthDropPerSecond = 2f;
		while (true) {
			if (Health > 5f) {
				TakeDamage(healthDropPerSecond * Time.deltaTime);
			}
			yield return new WaitForSeconds(Time.deltaTime);
		}
	}
}
