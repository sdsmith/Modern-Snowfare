using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(GUIText))]
[RequireComponent(typeof(GUITexture))]
public class PlayerInGameOverlay : MonoBehaviour {

    /** The transform of the game object to be tracked. */
    private Transform target;
    /** Name of the attached player. */
    private string playerName;
    /** Health component of the attached player. */
    private Health playerHealth;

    private GUIText guiTextComponent;
    private GUITexture guiTextureComponent;



	void Start () {
        // Get components
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
        guiTextComponent.text = "HELLO WORLD THIS IS A LONG THING";


        // Setup texture to be drawn
        Texture2D overlayTexture = new Texture2D(500, 50);
        overlayTexture.SetPixel(0, 0, Color.red);
        overlayTexture.Apply();

        guiTextureComponent.texture = overlayTexture;
	}
	

	void Update () {
        // Move overlay to target
        //
        // @NOTE(sdsmith): @STUDY(sdsmith): Could we lock ourselves to the target? 
        // Would this be a better option?
        Vector3 targetScreenPos = Camera.main.WorldToScreenPoint(target.position);
        DebugOverlay.AddAttr("player overlay pos (world)", target.position.ToString());
        DebugOverlay.AddAttr("player overlay pos (screen)", targetScreenPos.ToString());
        transform.position = targetScreenPos;
    }
}
