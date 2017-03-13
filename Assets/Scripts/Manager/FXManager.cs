using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour {

	public GameObject SnowballPrefab;
    public float SnowballSpeed = 10f;

	List<GameObject> currentSnowballs;
    //public AudioClip snowballFXAudio; 

    struct InGameOverlay {
        /** Radius of the circle that is considered 'near' the center reticle point. */
        public float nearReticleAngle;
    };
    InGameOverlay inGameOverlay;


    void Start() {
        inGameOverlay = new InGameOverlay();
        inGameOverlay.nearReticleAngle = 15f;//degrees
    }

    void OnGUI() {
        if (Util.localPlayer) {
            DisplayInGameOverlay();
        }
    }

    void DisplayInGameOverlay() {
        // Enable overlays with proximity
        foreach (GameObject goOverlay in GameObject.FindGameObjectsWithTag("PlayerInGameOverlay")) {
            PlayerInGameOverlay overlay = goOverlay.GetComponent<PlayerInGameOverlay>();

            // Don't display our own overlay
            Debug.Assert(Util.localPlayer.GetComponentInChildren<PlayerInGameOverlay>() != null, "Could not find PlayerInGameOverlay in child components");
            if (Util.localPlayer.GetComponentInChildren<PlayerInGameOverlay>() == overlay) {
                continue;
            }

            Transform playerTransform = overlay.GetTarget();

            // Calculate the distance from the player to the reticle
            Vector3 vecCamToPlayer = playerTransform.position - Camera.main.transform.position;
            
            // Enable overlay iff close to reticle
            if (Vector3.Angle(vecCamToPlayer, Camera.main.transform.forward) <= inGameOverlay.nearReticleAngle) {
                overlay.Enable();
            } else {
                overlay.Disable();
            }
        }
    }

	//SniperBulletFx
	[PunRPC]
	void SnowballFX(Vector3 startPos, Quaternion rotation, float damage){
		SnowballPrefab.GetComponent<SnowballController> ().SetSnowballDamage (damage);
        Instantiate(SnowballPrefab, startPos, rotation);
	}
}
