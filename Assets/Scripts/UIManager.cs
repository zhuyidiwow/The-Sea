using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

	public GameObject Title;
	public GameObject PressKey;

	public void StartGame() {
		Title.SetActive(false);
		PressKey.SetActive(false);
	}
}
