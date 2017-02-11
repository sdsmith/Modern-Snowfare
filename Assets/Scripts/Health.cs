using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

	public float hitPoints = 100f;
	float currentPoints;

	// Use this for initialization
	void Start () {
		currentPoints = hitPoints;
	}
	

	[PunRPC] // can be called indirectly
	// all players recieve notification of something taking damage
	public void TakeDamage (float amt)  {
		currentPoints -= amt;

		if (currentPoints <= 0) {
			Die ();
		}
	}

	void Die(){

		// game objects created locally (crate)
		if (GetComponent<PhotonView> ().instantiationId == 0) {
			Destroy (gameObject);
		} 

		//game objects instantiated over the network (players)
		else {
			if (PhotonNetwork.isMasterClient) {
				PhotonNetwork.Destroy (gameObject);
			}
		}
	}
}
