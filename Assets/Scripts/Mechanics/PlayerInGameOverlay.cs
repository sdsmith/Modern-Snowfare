using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(GUIText))]
[RequireComponent(typeof(GUITexture))]
public class PlayerInGameOverlay : MonoBehaviour {

    /** True if the overlay is enabled, false o/w. */
    private bool overlayEnabled;
    /** The transform of the game object to be tracked. */
    private Transform target;
    /** Name of the attached player. */
    private string playerName;
    /** Health component of the attached player. */
    private Health playerHealth;

    private GUIText guiTextComponent;
    private GUITexture guiTextureComponent;



	void Start () {
        overlayEnabled = false;

        // Get and enable components
        guiTextComponent = GetComponent<GUIText>();
        guiTextureComponent = GetComponent<GUITexture>();


        // Get the player's health component
        playerHealth = gameObject.GetComponentInParent<Health>();
        Debug.Assert(playerHealth != null, "Parent game object must have Health");

        // Get the player's name
        PhotonView targetPhotonView = gameObject.GetComponentInParent<PhotonView>();
        Debug.Assert(targetPhotonView != null, "Parent game object must have a PhotonView");
        Debug.Assert(targetPhotonView.owner != null, "Parent game object must be owner by a user, not the scene");
        string playerName = targetPhotonView.owner.NickName;

        // Set our target to the player's transform
        target = gameObject.transform.root;

        // Set the overlay to show that player's name
        guiTextComponent.text = playerName;


        // Setup texture to be drawn
        Texture2D overlayTexture = new Texture2D(5,1);
        overlayTexture.SetPixel(0, 0, Color.red);
        overlayTexture.Apply();

        guiTextureComponent.texture = overlayTexture;
	}

    /**
     * Displays the overlay for this frame.
     */
    void Update() {
        if (overlayEnabled) {
            // Move overlay to target
            // @NOTE(sdsmith): GUIText (and GUITexture) use viewport space, ie. values in 
            // range [0,1] for position.
            Vector3 targetViewportPos = Camera.main.WorldToViewportPoint(target.position);
            DebugOverlay.AddAttr("player overlay pos (world)", target.position.ToString());
            DebugOverlay.AddAttr("player overlay pos (viewport)", targetViewportPos.ToString());
            transform.position = targetViewportPos;
        }
    }


    public void Enable() {
        guiTextComponent.enabled = true;
        guiTextureComponent.enabled = true;
        overlayEnabled = true;
    }


    public void Disable() {
        guiTextComponent.enabled = false;
        guiTextureComponent.enabled = false;
        overlayEnabled = false;
    }


    public Transform GetTarget() {
        return target;
    }
}
