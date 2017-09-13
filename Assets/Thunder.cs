using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunder : MonoBehaviour {

	public AudioClip[] AudioClips;
	private AudioSource audioSource;

	void Start() {
		audioSource = GetComponent<AudioSource>();
	}

	public void PlayThunderSound() {
		StartCoroutine(PlaySoundCoroutine());
	}

	IEnumerator PlaySoundCoroutine() {
		yield return new WaitForSeconds(1.5f);
		audioSource.clip = AudioClips[Random.Range(0, 1)];
		audioSource.Play();
	}
}
