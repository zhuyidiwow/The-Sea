using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour {
	public AudioClip WaveSound;
	public float Distance;
	[Tooltip("From South, looking from East, clockwise")]
	public float VerticalAngle;
	private float VerticalAngleInRad;
	[Tooltip("From South, looking from above, clockwise")]
	public float HorizontalAngle;
	private float HorizontalAngleInRad;

	public bool ShouldFollowPlayer = false;
	public Transform CameraStartTransform;
	public Transform CameraFinaleTransform;
	public Text ThanksText;
	public Text GameCreditText;
	public Text MusicCreditText;
	public Text ExitText;
	public GameObject FinaleAnimation;
	public Image Panel;
	private bool ShouldWaitForStartGame = true;
	private bool ShouldWaitForKeyDown = false;
	
	private Player player;
	private Vector3 playerPosition;
	private Animator animator;
	private bool canExit = false;
	
	
	void Start() {
		player = GetterUtility.GetPlayer();
		VerticalAngleInRad = VerticalAngle * Mathf.Deg2Rad;
		HorizontalAngleInRad = HorizontalAngle * Mathf.Deg2Rad;
		animator = GetComponent<Animator>();
		StartCoroutine(SceneStartCoroutine());
		Time.timeScale = 0.01f;
	}

	IEnumerator SceneStartCoroutine() {
		animator.SetBool("Start Animation", true);
		yield return new WaitForSeconds(3f);
		animator.SetBool("Start Animation", false);
		ShouldWaitForKeyDown = true;
	}
	
	void Update() {
		if (ShouldWaitForStartGame) {
			ShouldWaitForStartGame = false;
			Time.timeScale = 1f;
		}
		if (ShouldWaitForKeyDown && Input.anyKeyDown) {
			StartGame();
		}

		if (canExit && Input.anyKeyDown) {
			Application.Quit();
		}

//		if (Input.GetKeyDown(KeyCode.S)) {
//			StopCoroutine(SceneStartCoroutine());
//			StartGame();
//		}
		
		if (ShouldFollowPlayer) {
			KeepDistance();
			LookAtPlayer();
		}
	}

	public void StartGame() {
		ShouldWaitForKeyDown = false;
		StartCoroutine(StartGameCoroutine());
	}

	IEnumerator StartGameCoroutine() {
		animator.enabled = false;
		
		// move camera to the starting position
		float startTime = Time.time;
		float duration = 2f;
		Vector3 originalPosition = transform.position;
		Quaternion originalRotation = transform.rotation;
		while (Time.time - startTime < duration) {
			transform.position = Vector3.Lerp(originalPosition, CameraStartTransform.position, (Time.time - startTime) / duration);
			transform.rotation = Quaternion.Lerp(originalRotation, CameraStartTransform.rotation, (Time.time - startTime) / duration);
			yield return new WaitForSeconds(Time.deltaTime/2f);
		}
		
		ShouldFollowPlayer = true;
		player.Scene0();
		GetterUtility.GetUiManager().StartGame();
	}
	
	private void KeepDistance() {
		playerPosition = player.transform.position;
		VerticalAngleInRad = VerticalAngle * Mathf.Deg2Rad;
		HorizontalAngleInRad = HorizontalAngle * Mathf.Deg2Rad;
		float x = playerPosition.x - Distance * Mathf.Cos(VerticalAngleInRad) * Mathf.Sin(HorizontalAngleInRad);
		float y = playerPosition.y + Distance * Mathf.Sin(VerticalAngleInRad);
		float z = playerPosition.z - Distance * Mathf.Cos(VerticalAngleInRad) * Mathf.Cos(HorizontalAngleInRad);
		Vector3 newPosition = new Vector3(x, y, z);
		transform.position = newPosition;
	}
	
	private void LookAtPlayer() {
		GetComponent<Transform>().LookAt(player.transform);
	}

	public void ChangePosition(float distance, float vAngle, float hAngle) {
		StartCoroutine(ChangePositionCoroutine(distance, vAngle, hAngle));
	}

	IEnumerator ChangePositionCoroutine(float distance, float vAngle, float hAngle) {
		float oDistance = Distance;
		float oVAngle = VerticalAngle;
		float oHAngle = HorizontalAngle;
		float startTime = Time.time;
		float duration = 3f;

		while (Time.time - startTime < duration) {
			Distance = oDistance + (distance - oDistance) * ((Time.time - startTime) / duration);
			VerticalAngle = oVAngle + (vAngle - oVAngle) * ((Time.time - startTime) / duration);
			HorizontalAngle = oHAngle + (hAngle - oHAngle) * ((Time.time - startTime) / duration);
			yield return new WaitForSeconds(Time.deltaTime / 2f);
		}
	}

	public void StartFinale() {
		StartCoroutine(FinaleCoroutine());
	}

	IEnumerator FinaleCoroutine() {
		GetComponent<Camera>().backgroundColor = Color.black;
		ShouldFollowPlayer = false;
		// move camera to the starting position
		float startTime = Time.time;
		float duration = 3f;
		Vector3 originalPosition = transform.position;
		Quaternion originalRotation = transform.rotation;
		while (Time.time - startTime < duration) {
			transform.position = Vector3.Lerp(originalPosition, CameraFinaleTransform.position, (Time.time - startTime) / duration);
			transform.rotation = Quaternion.Lerp(originalRotation, CameraFinaleTransform.rotation, (Time.time - startTime) / duration);
			yield return new WaitForSeconds(Time.deltaTime/2f);
		}
		GameObject.Find("Rain").SetActive(false);
		FinaleAnimation.SetActive(true);
		yield return new WaitForSeconds(3f);
		StartCoroutine(DeemCoroutine());
	}

	IEnumerator DeemCoroutine() {
		float startTime = Time.time;
		float duration = 2f;
		Color transparent = new Color(0f, 0f, 0f, 0f);
		Color black = Color.black;
		while (Time.time - startTime < duration) {
			Panel.color = Color.Lerp(transparent, black, (Time.time - startTime) / duration);
			yield return new WaitForSeconds(Time.deltaTime/2f);
		}

		StartCoroutine(TextFadeInCoroutine(ThanksText, 0f, 1f));
		StartCoroutine(TextFadeInCoroutine(GameCreditText, 1f, 1f));
		StartCoroutine(TextFadeInCoroutine(MusicCreditText, 2f, 0.5f));
		StartCoroutine(TextFadeInCoroutine(ExitText, 2.5f, 0.5f));
		yield return new WaitForSeconds(3f);
		canExit = true;
	}

	IEnumerator TextFadeInCoroutine(Text text, float waitTime, float duration) {
		yield return new WaitForSeconds(waitTime);
		float startTime = Time.time;
		Color transparent = new Color(1f, 1f, 1f, 0f);
		Color targetColor = Color.white;
		while (Time.time - startTime < duration) {
			text.color = Color.Lerp(transparent, targetColor, (Time.time - startTime) / duration);
			yield return new WaitForSeconds(Time.deltaTime/2f);
		}
	}
}

