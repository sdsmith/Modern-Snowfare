using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseController))]
public class Health : MonoBehaviour {

	float hitPoints;
	float currentPoints;

	private BaseController bc;


	// Use this for initialization
	public void Start () {

        bc = GetComponent<BaseController> ();
        if (bc == null) {
            Debug.LogError ("Base Controller is null");
        }

        hitPoints = bc.GetHealth ();
        currentPoints = hitPoints;

        // @DEBUG(sdsmith): Add entry to debug overlay
        DebugOverlay.AddAttr("Max health", hitPoints.ToString());
        DebugOverlay.AddAttr("Current health", currentPoints.ToString());
    }
	
    public float GetMaxHitPoints() {
        return hitPoints;
    }

    public float GetCurrentHitPoints() {
        return currentPoints;
    }


	[PunRPC] // can be called indirectly
	// all players recieve notification of something taking damage
	public virtual void TakeDamage (float amt, int attackerViewID) {
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
			SetStats ();

            // Play kill sound for attacker
            PhotonView pv = PhotonView.Find(attackerViewID);
            if (pv) {
                // Player that hit this target is still alive, play the notification
                // @NOTE(sdsmith): If this is placed before 'Die()', players do not 
                // die. Who knows.
                pv.gameObject.GetComponent<PlayerController>().PlayKillNotification();
            }
        }
	}

    void Die() {
        Debug.Log("Dying");
        // game objects created locally (crate)
        if (GetComponent<PhotonView>().instantiationId == 0) {
            Destroy(gameObject);
        }
        //game objects instantiated over the network (players)
        else {
            // Only the owner of the object destroys the game object
            if (GetComponent<PhotonView>().isMine) {
                //GetComponent<PhotonView> ().transform.FindChild("IndicatorLogic(Clone)").gameObject.SetActive(false);
                // Check to see if this is MY player object. If it's mine, respawn my character
                // Note: make sure character prefab has the tag set to player
                if (gameObject.tag == "Player") {
                    // show the standby camera. Optional for now
                    if (GetComponent<GrabAndDrop>() != null) {
                        if (GetComponent<GrabAndDrop>().GetGrabbedObjectName() == "Torch_Red"
                            || GetComponent<GrabAndDrop>().GetGrabbedObjectName() == "Torch_Blue") {

                            GetComponent<GrabAndDrop>().flameOff();
                        }
                        GetComponent<GrabAndDrop>().DropObject();
                    }

                    NetworkManager nm = GameObject.FindObjectOfType<NetworkManager>();
                    nm.standbyCamera.SetActive(true);
                    nm.respawnTimer = 2f;
                }
                GetComponent<PhotonView>().RPC("DeathAnimation", PhotonTargets.All);
                //transform.DetachChildren();
                // DeathAnimation ();
                PhotonNetwork.Destroy(gameObject);

                // Update utilities
                Util.localPlayer = null;
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
//	void OnGUI(){
//		// If this is my player, kill myself to test respawning
//		if (GetComponent<PhotonView> ().isMine && gameObject.tag == "Player") {
//			if (GUI.Button (new Rect (Screen.width - 225, 0, 225, 30), "I don't wanna be here anymore!")) {
//				Die ();
//			}
//		}
//	}

	public void SetCurrentPoints(float health){
		this.currentPoints = health;
	}

	// When a player dies, increment their death count and opponents kill count
	void SetStats() {
		((PlayerController)bc).deathCount ++;
		if (Util.localPlayer) {
			Util.localPlayer.GetComponent<PlayerController> ().killCount++;
		}
	}
}
