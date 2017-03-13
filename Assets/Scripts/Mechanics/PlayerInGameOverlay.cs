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

    /** Name of the attached player. */
    private string playerName;
    /** Health component of the attached player. */
    private Health playerHealth;

    private Canvas overlayCanvas;


	void Start () {
        overlayEnabled = false;
        screenOffset = new Vector3(0, 0, 0);
        localOffset = new Vector3(0, 0, 0);

        // Get the overlay canvas
        overlayCanvas = GetComponent<Canvas>();

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

        // Set the overlay to show the player's name
        overlayPlayerNameText = Util.AddTextToCanvas(playerName, overlayCanvas.gameObject);
        overlayPlayerNameText.alignment = TextAnchor.MiddleCenter;
        overlayPlayerNameText.fontSize = 14;
        switch (Util.GetTeam(gameObject)) {
            case PunTeams.Team.none:
                overlayPlayerNameText.color = Color.gray;
                break;
            case PunTeams.Team.red:
                overlayPlayerNameText.color = Color.red;
                break;
            case PunTeams.Team.blue:
                overlayPlayerNameText.color = Color.blue;
                break;
            default:
                Debug.Assert(false, "Panic: an object does not have a valid team type");
                break;
        }

        // @TODO(sdsmith): Add health info to the overlay
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


    public void Enable() {
        overlayCanvas.enabled = true;
        overlayEnabled = true;
    }


    public void Disable() {
        overlayCanvas.enabled = false;
        overlayEnabled = false;
    }


    public Transform GetTarget() {
        return target;
    }
}
