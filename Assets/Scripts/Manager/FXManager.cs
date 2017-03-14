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
        public float nearReticleRadius;
    };
    InGameOverlay inGameOverlay;


    void Start() {
        inGameOverlay = new InGameOverlay();
        inGameOverlay.nearReticleRadius = 1000f; // @TODO(sdsmith): Adjust value
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
    
            // Vector from camera to player
            Vector3 toPlayer = overlay.GetTarget().position - Camera.main.transform.position;

            // Calculate distance to reticle
            Vector3 forward = Camera.main.transform.forward * toPlayer.magnitude;
            float projectOntoForward = toPlayer.magnitude * Mathf.Cos(Vector3.Angle(forward, toPlayer));
            float distanceToReticle = Mathf.Sqrt(Mathf.Pow(toPlayer.magnitude, 2) - Mathf.Pow(projectOntoForward, 2));

            // Enable overlay iff close to reticle
            if (distanceToReticle <= inGameOverlay.nearReticleRadius) {
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
