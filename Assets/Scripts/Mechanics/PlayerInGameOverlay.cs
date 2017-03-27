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
    /** Reference to the player's name text in the overlay. */
    private Text overlayPlayerNameText;
    /** Reference to the overlay background image. */
    private Image overlayBackgroundImage;
    /** Maximum number of characters to display in the overlay for a player's name. */
    private int maxPlayerNameDisplayLength = 8;
    /** The color of the player's team that the overlay is attached to. */
    private Color teamColor;

    /** Name of the attached player. */
    private string playerName;
    /** Health component of the attached player. */
    private Health playerHealth;

    /** Reference to the overlay halth bar slider. */
    private Slider overlayHealthBarSlider;
    /** Reference to the overlay canvas component. */
    private Canvas overlayCanvas;


	void Awake () {
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
        GameObject canvasGameObject = overlayCanvas.gameObject;

        // Get the player's health component
        playerHealth = gameObject.GetComponentInParent<Health>();
        Debug.Assert(playerHealth != null, "Parent game object must have Health");

        // Get the player's name
        PhotonView targetPhotonView = gameObject.GetComponentInParent<PhotonView>();
        Debug.Assert(targetPhotonView != null, "Parent game object must have a PhotonView");
        Debug.Assert(targetPhotonView.owner != null, "Parent game object must be owner by a user, not the scene");
        playerName = targetPhotonView.owner.NickName.ToUpper();
        // Adjust the player name to max display length
        if (playerName.Length > maxPlayerNameDisplayLength) {
            playerName = playerName.Substring(0, maxPlayerNameDisplayLength - 1) + "~";
        }

        // Set our target to the player's transform
        target = gameObject.transform.root;

        // @NOTE(sdsmith): Can only have one graphic component per game object. Therefore, each 
        // graphic element of the overlay must be its own game object.

        // Get the background image component
        Transform overlayBackgroundTransform = canvasGameObject.transform.Find("Background");
        Debug.Assert(overlayBackgroundTransform != null, "PlayerInGameOverlay must have a child 'Background' game object");
        overlayBackgroundImage = canvasGameObject.transform.Find("Background").gameObject.GetComponent<Image>();
        Debug.Assert(overlayBackgroundImage != null, "Background game object must have an 'Image' component");

        // Get the health bar game object and slider component
        Transform overlayHealthBarTransform = canvasGameObject.transform.Find("HealthBar");
        Debug.Assert(overlayHealthBarTransform != null, "PlayerInGameOverlay must have a child 'HealthBar' game object");
        overlayHealthBarSlider = overlayHealthBarTransform.gameObject.GetComponent<Slider>();
        Debug.Assert(overlayHealthBarSlider != null, "HealthBar game object must have a 'Slider' component");
        RectTransform healthBarRectTransform = overlayHealthBarTransform.gameObject.GetComponent<RectTransform>();

        // Get the player name text component
        Transform overlayPlayerNameTransform = canvasGameObject.transform.Find("PlayerName");
        Debug.Assert(overlayPlayerNameTransform != null, "PlayerInGameOverlay must have a child 'PlayerName' game object");
        overlayPlayerNameText = overlayPlayerNameTransform.gameObject.GetComponent<Text>();
        Debug.Assert(overlayPlayerNameText != null, "PlayerName game object must have a 'Text' component");
        RectTransform playerNameRectTransform = overlayPlayerNameTransform.gameObject.GetComponent<RectTransform>();

        float playerNameHeight;
        float healthBarHeight = healthBarRectTransform.sizeDelta.y;

        // Add the player's name to the overlay
        overlayPlayerNameText.text = playerName;
        overlayPlayerNameText.color = teamColor;
        playerNameHeight = overlayPlayerNameText.preferredHeight;
        overlayPlayerNameText.transform.position += new Vector3(0, (playerNameHeight / 4.0f) / 10f, 0); // Shift up in overlay
        //overlayPlayerNameText.FontTextureChanged();

        // Add a background to the overlay
        overlayBackgroundImage.color = new Color(teamColor.r / 3, teamColor.g, teamColor.b, 0.3f);
        // Adjust background size to fit overlay content
        const float padding = 2f;
        float overlayWidth = overlayPlayerNameText.fontSize * maxPlayerNameDisplayLength + padding * 2.0f;
        float overlayHeight = playerNameHeight + healthBarHeight + padding;
        overlayBackgroundImage.rectTransform.sizeDelta = new Vector2(overlayWidth, overlayHeight);

        // Set the player name text to be with width of the overlay (minus the padding)
        playerNameRectTransform.sizeDelta = new Vector2(overlayWidth - padding * 2f, overlayPlayerNameText.preferredHeight);

        // Add health bar to the overlay
        overlayHealthBarSlider.minValue = 0;
        overlayHealthBarSlider.maxValue = playerHealth.GetMaxHitPoints();
        overlayHealthBarSlider.value = playerHealth.GetCurrentHitPoints();
        healthBarRectTransform.sizeDelta = new Vector2(overlayWidth - padding * 2.0f, healthBarRectTransform.sizeDelta.y);
        overlayHealthBarSlider.transform.position -= new Vector3(0, (healthBarHeight / 4.0f) / 10f, 0);

        // Turn off the overlay by default
        Disable();
    }


    /**
     * Display the overlay for this frame.
     */
    void LateUpdate() {
        if (overlayEnabled && Camera.main != null) {
            // Force the canvas to face the player
            Vector3 v = Camera.main.transform.position - transform.position;
            v.x = v.z = 0.0f;
            overlayCanvas.transform.LookAt(Camera.main.transform.position - v);
            overlayCanvas.transform.Rotate(0, 180, 0);

            // Update the health bar with the player's current health
            overlayHealthBarSlider.value = playerHealth.GetCurrentHitPoints();
        }
    }


    /**
     * Enable the canvas component to display the overlay.
     */
    public void Enable() {
        // NOTE(sdsmith): Could do overlayCanvas.enabled = false, however, read the STUDY.
        // Doing it this way acts like disabling its child objects.
        // STUDY(sdsmith): Without disbaling the PlayerName game object, the text will not display 
        // by default even while all the settings were correct. Any modification to the Text 
        // component in the inspector would trigger the text to display. This is simply a workaround.
        overlayCanvas.gameObject.SetActive(true);
        overlayEnabled = true;
    }


    /**
     * Disable the canvas component to hide the overlay.
     */
    public void Disable() {
        // NOTE(sdsmith): Could do overlayCanvas.enabled = false, however, read the STUDY.
        // Doing it this way acts like disabling its child objects.
        // STUDY(sdsmith): Without disbaling the PlayerName game object, the text will not display 
        // by default even while all the settings were correct. Any modification to the Text 
        // component in the inspector would trigger the text to display. This is simply a workaround.
        overlayCanvas.gameObject.SetActive(false);
        overlayEnabled = false;

    }


    /**
     * Return the transform that the overlay is following.
     */
    public Transform GetTarget() {
        return target;
    }
}
