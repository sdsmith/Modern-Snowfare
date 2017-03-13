using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class PlayerInGameOverlay : MonoBehaviour {

    /** True if the overlay is enabled, false o/w. */
    private bool overlayEnabled;
    /** The transform of the game object to be tracked. */
    private Transform target;
    /** Offset from the target (local delta). */
    private Vector3 localOffset;
    /** Set the offset of the canvas on the screen (ie. shift from (0,0) screen space). */
    private Vector3 screenOffset;
    /** Reference to the player's name text in the overlay. */
    private Text overlayPlayerNameText;
    /** Reference to the overlay background image. */
    private Image overlayBackgroundImage;

    private Color teamColor;

    /** Name of the attached player. */
    private string playerName;
    /** Health component of the attached player. */
    private Health playerHealth;

    private Canvas overlayCanvas;


	void Awake () {
        overlayEnabled = false;
        screenOffset = new Vector3(0, 0, 0);
        localOffset = new Vector3(0, 0, 0);

        // Set the team color
        switch (Util.GetTeam(gameObject)) {
            case PunTeams.Team.none:
                teamColor = Color.gray;
                break;
            case PunTeams.Team.red:
                teamColor = Color.red;
                break;
            case PunTeams.Team.blue:
                teamColor = Color.blue;
                break;
            default:
                Debug.Assert(false, "Panic: an object does not have a valid team type");
                break;
        }

        // Get the overlay canvas
        overlayCanvas = GetComponent<Canvas>();

        // Get the player's health component
        playerHealth = gameObject.GetComponentInParent<Health>();
        Debug.Assert(playerHealth != null, "Parent game object must have Health");

        // Get the player's name
        PhotonView targetPhotonView = gameObject.GetComponentInParent<PhotonView>();
        Debug.Assert(targetPhotonView != null, "Parent game object must have a PhotonView");
        Debug.Assert(targetPhotonView.owner != null, "Parent game object must be owner by a user, not the scene");
        playerName = targetPhotonView.owner.NickName;

        // Set our target to the player's transform
        target = gameObject.transform.root;

        GameObject canvasGameObject = overlayCanvas.gameObject;

        // @NOTE(sdsmith): Can only have one graphic component per game object. Therefore, each 
        // graphic element of the overlay must be its own game object.

        // Get the background image component
        Transform overlayBackgroundTransform = canvasGameObject.transform.Find("Background");
        Debug.Assert(overlayBackgroundTransform != null, "PlayerInGameOverlay must have a child 'Background' game object");
        overlayBackgroundImage = canvasGameObject.transform.Find("Background").gameObject.GetComponent<Image>();
        Debug.Assert(overlayBackgroundImage != null, "Background game object must have an 'Image' component");

        // Get the player name text component
        Transform overlayPlayerNameTransform = canvasGameObject.transform.Find("PlayerName");
        Debug.Assert(overlayPlayerNameTransform != null, "PlayerInGameOverlay must have a child 'PlayerName' game object");
        overlayPlayerNameText = overlayPlayerNameTransform.gameObject.GetComponent<Text>();
        Debug.Assert(overlayPlayerNameText != null, "PlayerName game object must have a 'Text' component");

        // Get the health bar game obeject and slider component
        Transform overlayHealthBarTransform = canvasGameObject.transform.Find("HealthBar");
        Debug.Assert(overlayHealthBarTransform != null, "PlayerInGameOverlay must have a child 'HealthBar' game object");
        Slider overlayHealthBarSlider = overlayHealthBarTransform.gameObject.GetComponent<Slider>();
        Debug.Assert(overlayHealthBarSlider != null, "HealthBar game object must have a 'Slider' component");

        // Add a background colour to the overlay
        overlayBackgroundImage.color = new Color(teamColor.r / 3, teamColor.g, teamColor.b, 0.3f);

        // Add the player's name to the overlay
        overlayPlayerNameText.text = playerName;
        Font arialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        overlayPlayerNameText.font = arialFont;
        overlayPlayerNameText.material = arialFont.material;
        overlayPlayerNameText.alignment = TextAnchor.MiddleCenter;
        overlayPlayerNameText.fontSize = 14;
        overlayPlayerNameText.horizontalOverflow = HorizontalWrapMode.Overflow;
        overlayPlayerNameText.color = teamColor;

        // Add health info to the overlay
        


        // Adjust background size to fit overlay content
        const float backgroundPadding = 5f;
        overlayBackgroundImage.rectTransform.sizeDelta = new Vector2(overlayPlayerNameText.preferredWidth + backgroundPadding, overlayPlayerNameText.preferredHeight);

        // Turn off the overlay by default
        Disable();
    }


    /**
     * Display the overlay for this frame.
     */
    void LateUpdate() {
        if (overlayEnabled) {
            // Force the canvas to face the player
            Vector3 v = Camera.main.transform.position - transform.position;
            v.x = v.z = 0.0f;
            overlayCanvas.transform.LookAt(Camera.main.transform.position - v);
            overlayCanvas.transform.Rotate(0, 180, 0);
        }
    }


    /**
     * Enable the canvas component to display the overlay.
     */
    public void Enable() {
        overlayCanvas.enabled = true;
        overlayEnabled = true;
    }


    /**
     * Disabled the canvas component to hide the overlay.
     */
    public void Disable() {
        overlayCanvas.enabled = false;
        overlayEnabled = false;
    }


    /**
     * Return the transform that the overlay is following.
     */
    public Transform GetTarget() {
        return target;
    }
}
