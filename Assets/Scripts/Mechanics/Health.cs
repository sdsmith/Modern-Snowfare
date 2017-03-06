using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Health : MonoBehaviour {

	float hitPoints;
	float currentPoints;

	private PlayerController pc;


	// Use this for initialization
	void Start () {

        pc = GetComponent<PlayerController> ();
        if (pc == null) {
            Debug.LogError ("Player Controller is null");
        }

        hitPoints = pc.GetHealth ();
        currentPoints = hitPoints;

        // @DEBUG(sdsmith): Add entry to debug overlay
        DebugOverlay.AddAttr("Max health", hitPoints.ToString());
        DebugOverlay.AddAttr("Current health", currentPoints.ToString());
    }
	

	[PunRPC] // can be called indirectly
	// all players recieve notification of something taking damage
	public void TakeDamage (float amt) {
        currentPoints -= amt;

        // @DEBUG(sdsmith): Update debug stats
        // @TODO(sdsmith): @PERFORMANCE(sdsmith): Should have these calls 
        // wrapped under a debug flag to reduce performance impact of the 
        // calls when the overlay is not enabled.
        //if (this.gameObject.GetComponent<PhotonView>().isMine) {
        DebugOverlay.AddAttr("Current health", currentPoints.ToString());
        //}

        if (currentPoints <= 0) {
			Die ();
		}
	}

	void Die(){
		GameObject Indicator = GameObject.Find("IndicatorLogic");
		Indicator.SetActive(false);

		// game objects created locally (crate)
		if (GetComponent<PhotonView> ().instantiationId == 0) {
			Destroy (gameObject);
		} 

		//game objects instantiated over the network (players)
		else {
			// Only the owner of the object destroys the game object
			if (GetComponent<PhotonView> ().isMine) {

				// Check to see if this is MY player object. If it's mine, respawn my character
				// Note: make sure character prefab has the tag set to player
				if (gameObject.tag == "Player") {
					// show the standby camera. Optional for now
					if (GetComponent <GrabAndDrop> () != null) {
						GetComponent<GrabAndDrop> ().DropObject ();
					}

					NetworkManager nm = GameObject.FindObjectOfType<NetworkManager> ();
					nm.standbyCamera.SetActive (true);
					nm.respawnTimer = 2f;
				}
				GetComponent<PhotonView> ().RPC ("DeathAnimation", PhotonTargets.All);
				// DeathAnimation ();
				PhotonNetwork.Destroy (gameObject);
			}
		}
	}

	[PunRPC]
	void DeathAnimation() {
		ToonCharacterController toonCC = GetComponent<ToonCharacterController> ();
		if (toonCC != null) {
			 toonCC.Decapitate (true, 0, Vector3.zero);
		}
	}

	/*
	 * DEBUGGING PURPOSES 
	*/
	void OnGUI(){
		// If this is my player, kill myself to test respawning
		if (GetComponent<PhotonView> ().isMine && gameObject.tag == "Player") {
			if (GUI.Button (new Rect (Screen.width - 225, 0, 225, 30), "I don't wanna be here anymore!")) {
				Die ();
			}
		}
	}
}
