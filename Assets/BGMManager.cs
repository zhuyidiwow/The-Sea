using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour {

	public AudioClip BaseClip;

	public AudioClip HappyClip;
	public AudioClip SadClip;

	private AudioSource audioSource;
	
	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
	}

	public void PlayHappyClip() {
		StartCoroutine(ChangeToHappyCoroutine());
	}

	IEnumerator ChangeToHappyCoroutine() {
		float startTime = Time.time;
		float duration = 1f;
		float originalVolume = 1f;
		
		while (Time.time - startTime < duration) {
			audioSource.volume = originalVolume + (0f - originalVolume) * ((Time.time - startTime) / duration);
			yield return new WaitForSeconds(Time.deltaTime/2f);
		}

		audioSource.clip = HappyClip;
		audioSource.Play();
		startTime = Time.time;
		
		while (Time.time - startTime < duration) {
			audioSource.volume = 0f + (originalVolume - 0f) * (Time.time - startTime) / duration;
			yield return new WaitForSeconds(Time.deltaTime/2f);
		}
	}
	
	public void PlaySadClip() {
		StartCoroutine(ChangeToSadCoroutine());
	}

	IEnumerator ChangeToSadCoroutine() {
		float startTime = Time.time;
		float duration = 1f;
		float originalVolume = audioSource.volume;
		while (Time.time - startTime < duration) {
			audioSource.volume = originalVolume + (0f - originalVolume) * (Time.time - startTime) / duration;
			yield return new WaitForSeconds(Time.deltaTime/2f);
		}

		audioSource.clip = SadClip;
		audioSource.Play();
		audioSource.loop = false;
		startTime = Time.time;
		
		while (Time.time - startTime < duration) {
			audioSource.volume = 0f + (originalVolume - 0f) * (Time.time - startTime) / duration;
			yield return new WaitForSeconds(Time.deltaTime/2f);
		}
	}
}
