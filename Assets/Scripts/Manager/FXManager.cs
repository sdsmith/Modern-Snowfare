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
        // @TODO(sdsmith): Change this to 'when the player is spawned'
        if (Camera.main) {
            DisplayInGameOverlay();
        }
    }

    void DisplayInGameOverlay() {
        // Enable overlays with proximity
        foreach (GameObject goOverlay in GameObject.FindGameObjectsWithTag("PlayerInGameOverlay")) {
            // Don't display our own overlay
            // @TODO(sdsmith): Fix
            if (false) {
                continue;
            }

            PlayerInGameOverlay overlay = goOverlay.GetComponent<PlayerInGameOverlay>();
    
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





        /*
         * @TODO(sdsmith): Doesn't seem to work right, plus the Physics.SphereCastAll destroys the framerate when you look up.
         * 
        // Check if the player has spawned
        // @TODO(sdsmith): Is this the right check? There must be a more future proof way.
        if (Camera.main != null) {
            //Vector2 reticleScreenPoint = Camera.main.WorldToScreenPoint(Camera.main.transform.forward); // @TODO(sdsmith): Should it be the camera forward, or some point in front of it?
            //                                                                                            // @PERFORMANCE(sdsmith): Additional parameters can tweak the collision zone and what layers are considered
            //inGameOverlay.collidersNearReticle = Physics2D.OverlapCircleAll(reticleScreenPoint, inGameOverlay.nearReticleRadius);

            //Vector3 terrainSize = Terrain.activeTerrain.terrainData.size;
            //float diagonalTerrainSpan = Vector2.Distance(new Vector2(0, 0), terrainSize);

            inGameOverlay.hitsNearReticle = Physics.SphereCastAll(Camera.main.transform.position, inGameOverlay.nearReticleRadius, Camera.main.transform.forward, 10f);


            foreach (RaycastHit hit in inGameOverlay.hitsNearReticle) {
                GameObject go = hit.collider.gameObject;

                if (go.tag == "Player" && go.GetComponent<PhotonView>().isMine) { // @TODO(sdsmith): Confirm isMine gets us our player on the local client
                    // Draw overlay over the player's head
                    // @TODO(sdsmith): Make it's opacity dependent on how close it is to the center of the reticle. Account for Z distance, and object size.

                    // Get the position over the player's head
                    Bounds playerBounds = go.GetComponent<Collider>().bounds;
                    Vector3 overheadPoint = playerBounds.center + new Vector3(0, playerBounds.extents.y, 0); // @NOTE(sdsmith): if we add a y offset here, the overlay's distance from the player's head will be relative to its z distance.

                    // Get that position on the screen
                    Vector3 overheadScreenPoint = Camera.main.WorldToScreenPoint(overheadPoint);

                    //// Add offset above the character's heads
                    //const float overheadOffset = 5;//px
                    //overheadScreenPoint.y += overheadOffset;
                    
                    // Set box size
                    // @PERFORMANCE(sdsmith): Do this in setup
                    Vector2 boxSize = new Vector2(500, 50); // @STUDY(sdsmith): Do these values need to be relative? (ie. Are they in pixels?)
                    Vector2 boxPosition = overheadScreenPoint;
                    //boxPosition.x -= (int)(boxSize.x / 2); // offset left
                    //boxPosition.y += (int)(boxSize.y / 2); // offset up

                    // Setup texture to be drawn
                    // @PERFORMANCE(sdsmith): Do this in setup
                    Texture2D overlayTexture = new Texture2D((int)boxSize.x, (int)boxSize.y);
                    overlayTexture.SetPixel(0, 0, Color.red);
                    overlayTexture.Apply();

                    // Draw the health bar
                    Rect healthBarRect = new Rect(overheadScreenPoint, boxSize);
                    //Texture2D oldBoxTex = GUI.skin.box.normal.background;
                    GUI.skin.box.normal.background = overlayTexture;
                    GUI.Box(healthBarRect, GUIContent.none);

                    // Reset the box texture to the old texture
                    // @PERFORMANCE(sdsmith): I do this only because I don't know if we have plans to use this later.
                    // With a better understand of our GUI design, this may not be necessary.
                    //GUI.skin.box.normal.background = oldBoxTex;
                }
            }
        }
        */
    }

	//SniperBulletFx
	[PunRPC]
	void SnowballFX(Vector3 startPos, Quaternion rotation){
        Instantiate(SnowballPrefab, startPos, rotation);
	}
}
