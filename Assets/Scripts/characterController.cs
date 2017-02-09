using UnityEngine;
using System.Collections;

public class characterController : MonoBehaviour {

	public float speed= 10.0F;
	public weaponController theWeapon;

	// NOTE: Use this for initialization
	void Start () {
        // Lock cursor to window (hides OS cursor graphic)
		Cursor.lockState = CursorLockMode.Locked;
	}	
	
	// NOTE: Update is called once per frame
	void Update () {
        // Character object translation
		float translation = Input.GetAxis ("Vertical") * speed;
		float strafe = Input.GetAxis ("Horizontal") * speed;
		strafe *= Time.deltaTime;
		translation *= Time.deltaTime;

		transform.Translate (strafe, 0, translation);
        

        // TODO(sdsmith): Refactor non-character specific input out of here.
		if (Input.GetKeyDown ("escape"))
			Cursor.lockState = CursorLockMode.None;
		
        
        // Character Input handling
        // NOTE: 0 left click
		if (Input.GetMouseButtonDown (0)) {
			theWeapon.isFiring = true;
		}
		if (Input.GetMouseButtonUp (0)) {
			theWeapon.isFiring = false;
		}
	}
}
