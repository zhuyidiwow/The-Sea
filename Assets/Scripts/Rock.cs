using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour {

	public AudioClip[] AudioClips;
	public float maxZ;
	public float minZ;
	public GameObject Splash;
	public float InitialSpeed;
	private Vector3 InitialDirection;
	private Rigidbody rb;
	private bool hasHurtPlayer = false;
	private AudioSource audioSource;

	private void Awake() {
		rb = GetComponent<Rigidbody>();
		rb.isKinematic = true;
		audioSource = GetComponent<AudioSource>();
	}

	public void GetThorwnOut() {
		rb.isKinematic = false;
		InitialDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(1f, 1.5f), Random.Range(minZ, maxZ));
		rb.velocity = InitialSpeed * InitialDirection.normalized;
		audioSource.clip = AudioClips[0];
		audioSource.Play();
	}
	
	private void FixedUpdate() {
		if (!rb.isKinematic) {
			rb.AddForce(Vector3.down * 30f, ForceMode.Force);
		}
	}

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag("Water Surface")) {
			rb.velocity *= 0.3f;
			rb.drag = 1f;
			audioSource.clip = AudioClips[1];
			audioSource.Play();
			Instantiate(Splash, transform.position, new Quaternion());
			Destroy(gameObject, 2f);
		}
	}

	private void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.CompareTag("Player")) {
			if (!hasHurtPlayer) {
				hasHurtPlayer = true;
				GetterUtility.GetPlayer().TakeDamage(20f);
				audioSource.clip = AudioClips[2];
				audioSource.Play();
			}
		}

		if (collision.gameObject.CompareTag("Friendly")) {
			if (!hasHurtPlayer) {
				audioSource.clip = AudioClips[2];
				audioSource.Play();
				hasHurtPlayer = true;
				collision.gameObject.GetComponent<Friendly>().Sink();
			}
		}
	}
}
