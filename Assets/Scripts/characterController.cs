using UnityEngine;
using System.Collections;



[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class characterController : MonoBehaviour {

    public float speed = 10.0F;
    public float jumpSpeed = 15.0F;
    public weaponController theWeapon;
    public bool useOldWeapon = false;

    private Rigidbody rigidbody;
    private CapsuleCollider collider;


        
    // NOTE: Use this for initialization
    void Start () {
        // Component references
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();        
        
        // Lock cursor to window (hides OS cursor graphic)
        Cursor.lockState = CursorLockMode.Locked;        
    }

    
    // NOTE: Update is called once per frame
    void Update () {
        // Character movement 
        Vector3 moveDirection = Vector3.zero;

        if (IsGrounded()) {
            float forwardMove = Input.GetAxis("Vertical");
            float sideMove = Input.GetAxis("Horizontal");

            moveDirection = new Vector3(sideMove, 0, forwardMove);
            moveDirection *= speed;

            
            // Jump
            if (Input.GetButtonDown("Jump")) {
                // Add upward velocity (jump!)
                moveDirection.y = jumpSpeed;            
            } else {
                // Continue with existing velocity
                moveDirection.y = rigidbody.velocity.y;
            }

            // Transform from Local to World space
            moveDirection = transform.TransformDirection(moveDirection);
        
            // Move
            rigidbody.velocity = moveDirection;
        }                
        

        // TODO(sdsmith): Refactor non-character specific input out of here.
        if (Input.GetKeyDown("escape")) 
            Cursor.lockState = CursorLockMode.None;		
        
        // Character Input handling
		if (useOldWeapon) {
			// NOTE: 0 left click
			if (Input.GetMouseButtonDown (0)) {
				theWeapon.isFiring = true;
			}
			if (Input.GetMouseButtonUp (0)) {
				theWeapon.isFiring = false;
			}
		}
	}


    private bool IsGrounded() {
        const float errorMargin = 0.1F;
        return Physics.Raycast(transform.position, -Vector3.up, collider.bounds.extents.y + errorMargin);
    }
}
