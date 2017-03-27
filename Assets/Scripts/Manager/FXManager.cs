using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour {

	public GameObject SnowballPrefab;
    public float SnowballSpeed = 10f;
	public string[] PowerUps;

	int RandPowerUp;

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
		StartCoroutine(Spawner());
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

            // Our player should not have an overlay
            Debug.Assert(Util.localPlayer != null, "Local player game object not set");
            if (Util.localPlayer == playerGO) {
                continue;
            }

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
	void SnowballFX(Vector3 startPos, Quaternion rotation, float damage, int throwingPlayerViewID){
		SnowballPrefab.GetComponent<SnowballController>().SetSnowballDamage(damage);
        GameObject snowballGO = Instantiate(SnowballPrefab, startPos, rotation);
        snowballGO.GetComponent<SnowballController>().SetThrower(throwingPlayerViewID);
	}

    [PunRPC]
    IEnumerator Spawner(){	
		while (true) {
			int randomTimer =  Random.Range (0, 120);
			yield return new WaitForSeconds (randomTimer);
			RandPowerUp = Random.Range (0, 4);
			Vector3 SpawnPosition = new Vector3 (Random.Range (20, 150), 7, Random.Range (20, 150));
            GameObject temp = PhotonNetwork.Instantiate (PowerUps[RandPowerUp], SpawnPosition, gameObject.transform.rotation,0);
            GameObject indicator = PhotonNetwork.Instantiate("PowerUpGlow", SpawnPosition += new Vector3(0, -0.85f, 0), Quaternion.Euler(-90, 0, 0), 0);
            indicator.transform.parent = temp.transform;
        }
	}

    /**
     * Play a flag capture notification sound for the local player.
     */
    public void PlayFlagCaptureNotification() {
        Util.localPlayer.GetComponent<AudioSource>().PlayOneShot(AudioClips.flagCapture);
    }
}
