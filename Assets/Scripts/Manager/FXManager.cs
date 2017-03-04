using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour {

	public GameObject SnowballPrefab;
    public float SnowballSpeed = 10f;

	List<GameObject> currentSnowballs;
    //public AudioClip snowballFXAudio; 

    struct InGameOverlay {
        /** @PERFORMANCE(sdsmith): Allocate fixed sized buffer for speed. (see Physics2D.OverlapCircleNonAlloc) */
        public Collider2D[] collidersNearReticle;
        /** Radius of the circle that is considered 'near' the center reticle point. */
        public float nearReticleRadius;
    };
    InGameOverlay inGameOverlay;


    void Start() {
        inGameOverlay = new InGameOverlay();
        inGameOverlay.nearReticleRadius = 1000f;
    }

    void Update() {
        CreateInGameOverlay();
    }

    void CreateInGameOverlay() {

        // Check if the player has spawned
        // @TODO(sdsmith): Is this the right check? There must be a more future proof way.
        if (Camera.main != null) {
            Vector2 reticleScreenPoint = Camera.main.WorldToScreenPoint(Camera.main.transform.forward); // @TODO(sdsmith): Should it be the camera forward, or some point in front of it?
                                                                                                        // @PERFORMANCE(sdsmith): Additional parameters can tweak the collision zone and what layers are considered
            inGameOverlay.collidersNearReticle = Physics2D.OverlapCircleAll(reticleScreenPoint, inGameOverlay.nearReticleRadius);


            foreach (Collider2D collider in inGameOverlay.collidersNearReticle) {
                GameObject go = collider.gameObject;

                if (go.tag == "Player") {
                    // Draw overlay over the player's head
                    // @TODO(sdsmith): Make it's opacity dependent on how close it is to the center of the reticle. Account for Z distance, and object size.

                    // Get the position over the player's head
                    Bounds playerBounds = go.GetComponent<Collider>().bounds;
                    Vector3 overheadPoint = playerBounds.center + new Vector3(0, playerBounds.extents.y, 0); // @NOTE(sdsmith): if we add a y offset here, the overlay's distance from the player's head will be relative to its z distance.

                    // Get that position on the screen
                    Vector3 overheadScreenPoint = Camera.main.WorldToScreenPoint(overheadPoint);

                    // Add offset above the character's heads
                    const float overheadOffset = 5;//px
                    overheadScreenPoint.y += overheadOffset;

                    // Set box size
                    // @PERFORMANCE(sdsmith): Do this in setup
                    Vector2 boxSize = new Vector2(50, 10); // @STUDY(sdsmith): Do these values need to be relative? (ie. Are they in pixels?)
                    Vector2 boxPosition = overheadScreenPoint;
                    boxPosition.x -= (int)(boxSize.x / 2); // offset left
                    boxPosition.y += (int)(boxSize.y / 2); // offset up

                    // Setup texture to be drawn
                    // @PERFORMANCE(sdsmith): Do this in setup
                    Texture2D overlayTexture = new Texture2D((int)boxSize.x, (int)boxSize.y);
                    overlayTexture.SetPixel(0, 0, Color.black);
                    overlayTexture.Apply();

                    // Draw the health bar
                    Rect healthBarRect = new Rect(overheadScreenPoint, boxSize);
                    Texture2D oldBoxTex = GUI.skin.box.normal.background;
                    GUI.skin.box.normal.background = overlayTexture;
                    GUI.Box(healthBarRect, GUIContent.none);

                    // Reset the box texture to the old texture
                    // @PERFORMANCE(sdsmith): I do this only because I don't know if we have plans to use this later.
                    // With a better understand of our GUI design, this may not be necessary.
                    GUI.skin.box.normal.background = oldBoxTex;
                }
            }
        }
    }

	//SniperBulletFx
	[PunRPC]
	void SnowballFX(Vector3 startPos, Quaternion rotation){
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
