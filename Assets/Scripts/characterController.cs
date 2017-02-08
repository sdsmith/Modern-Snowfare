using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class characterController : MonoBehaviour {

	public float speed = 10.0F;
    public float gravity = 9.81F;
	public weaponController theWeapon;

	// NOTE: Use this for initialization
	void Start () {
        // Lock cursor to window (hides OS cursor graphic)
		Cursor.lockState = CursorLockMode.Locked;
	}	
	
	// NOTE: Update is called once per frame
	void Update () {
        {
            // Character movement
            CharacterController controller = GetComponent<CharacterController>();
            float forwardMove = Input.GetAxis("Vertical");
            float sideMove = Input.GetAxis("Horizontal");
            Vector3 moveDirection = new Vector3(sideMove, 0, forwardMove);

            // Transform from Local to World space
            moveDirection = transform.TransformDirection(moveDirection);

            moveDirection *= speed;
            moveDirection.y -= gravity * Time.deltaTime; // m/s

            // Move
            controller.Move(moveDirection * Time.deltaTime);
        }


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
