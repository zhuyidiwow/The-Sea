using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : MonoBehaviour {
	
	public GameObject RainDrop;
	public float interval;
	public int RainDropPerInterval;
	public Vector2 size;
	private Player player;
	
	void Start () {
		
		player = GetterUtility.GetPlayer();
		StartCoroutine(RainCoroutine());
	}

	private IEnumerator RainCoroutine() {
		float x;
		float y;
		float z;
		while (true) {
			for (int i = 0; i < RainDropPerInterval; i++) {
				x = player.transform.position.x - 20f + size.x * Random.Range(-1f, 1f);
				y = transform.position.y;
				z = player.transform.position.z + size.y * Random.Range(-1f, 1f);
				Instantiate(RainDrop, new Vector3(x, y, z), new Quaternion());
			}
			yield return new WaitForSeconds(interval);
		}	
	}
}
