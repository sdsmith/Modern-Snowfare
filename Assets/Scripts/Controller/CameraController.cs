using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	Vector2 mouseLook;
	Vector2 smoothV;
	public float sensitivity = 5.0f;
	public float smoothing = 2.0f;

	public float clampMin = -90f;
	public float clampMax = 90f;

	GameObject character;

    private Vector2 lastLockedInput;


    
	void Start () {
        // Lock cursor to window (hides OS cursor graphic)
        Cursor.lockState = CursorLockMode.Locked;

        character = this.transform.parent.gameObject;
	}
	
    
	void Update () {

        // Check the whether the cursor should be locked or unlocked
        if (Input.GetKeyDown("escape")) {
            Cursor.lockState = CursorLockMode.None;
        } else if (Cursor.lockState == CursorLockMode.None) {
            // Lock cursor if fire button is pressed
            if (Input.GetButton("Fire1")) {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        // Compute rotation
        Vector2 md;

        /* 
         * @NOTE(sdsmith): This is a dirty little hack. When the cursor gets unlocked, 
         * the character continues to rotate. I have tried setting the transforms' 
         * rotation to Quaternion.identity, but they still rotate. This ensures that 
         * they stay locked to the last locked cursor position.
         *                                                               - 2017-03-04
         */    
        if (Cursor.lockState == CursorLockMode.Locked) {
            md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            lastLockedInput = md;
        } else {
            md = lastLockedInput;
        }

        md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f / smoothing);
        mouseLook += smoothV;
        mouseLook.y = Mathf.Clamp(mouseLook.y, clampMin, clampMax);

        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, character.transform.up);
    }
}
