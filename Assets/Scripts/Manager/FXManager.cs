using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour {

	public GameObject SnowballPrefab;
	public float SnowballSpeed = 10f;

	List<GameObject> currentSnowballs;
	//public AudioClip snowballFXAudio; 

	//SniperBulletFx
	[PunRPC]
	void SnowballFX(Vector3 startPos, Quaternion rotation){
        Instantiate(SnowballPrefab, startPos, rotation);
	}
}
