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

    /**
     * @TODO(sdsmith): doc
     */
    void DisplayInGameOverlay() {
        // Enable overlays with proximity
        foreach (GameObject playerGO in GameObject.FindGameObjectsWithTag("Player")) {
            GameObject overlayGO;

            // Check if the player has an overlay
            Transform overlayTransform = Util.FindChildByTag(playerGO, "PlayerInGameOverlay");
            if (overlayTransform == null) {
                // Player does not have attached overlay, create one
                overlayGO = (GameObject)Instantiate(Resources.Load("Overlay/PlayerInGameOverlay"), playerGO.transform, false);
            } else {
                // Get the existing overlay
                overlayGO = overlayTransform.gameObject;
            }

            PlayerInGameOverlay overlay = overlayGO.GetComponent<PlayerInGameOverlay>();

            // Don't display our own overlay
            Debug.Assert(Util.localPlayer != null, "Local player game object not set");
            if (Util.localPlayer == playerGO) {
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
