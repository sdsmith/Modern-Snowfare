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
	void SnowballFX(Vector3 startPos, Quaternion rotation, float damage){
		SnowballPrefab.GetComponent<SnowballController> ().SetSnowballDamage (damage);
        Instantiate(SnowballPrefab, startPos, rotation);
	}

    /**
     * Displays the status bars of all players oriented toward the camera of
     * the local player.
     */
    void DisplayPlayerStatusBars() {
        // @TODO(sdsmith): see .LookAt(Transform) and the TextMesh component of
        // each Player.
    }
}
