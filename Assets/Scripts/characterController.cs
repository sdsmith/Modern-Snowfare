using UnityEngine;
using System.Collections;



[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class characterController : MonoBehaviour {

    public float speed = 10.0F;
    public float jumpSpeed = 15.0F;
    public weaponController theWeapon;
    private new Rigidbody rigidbody;
    private new CapsuleCollider collider;

        
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
        // NOTE: LEFT_CLICK = 0
        if (Input.GetMouseButtonDown(0)) {
            theWeapon.isFiring = true;
        }
        if (Input.GetMouseButtonUp(0)) {
            theWeapon.isFiring = false;
        }
    }


    /**
     * Return true if the character is on the ground, false otherwise.
     */
    private bool IsGrounded() {
        /*
         * NOTE(sdsmith): There was issues with steep slopes when one ray was
         * shot downward from the center. The fix is shooting one ray every 90
         * degrees of the circumference of the collider. 
         *                                                         - 2017-02-13
         * 
         * @Performance(sdsmith): Do we want to unroll this, short circuit it, 
         * keep it the same? I perfer consistent timing for an operation this 
         * common rather than fluctuating performance. However, it could be
         * argued that the general case is being on mostly level ground, which 
         * would short ciruit it on the first of second ray. The downside is 
         * you get a potential 4x slowdown on steep slopes.
         *                                                         - 2017-02-13
         */
        const float errorMargin = 0.1F;
        float colliderRadius = collider.radius;

        bool hit = false;

        for (float deltaX = -colliderRadius; deltaX <= colliderRadius; deltaX += 2*colliderRadius) {
            for (float deltaY = -colliderRadius; deltaY <= colliderRadius; deltaY += 2 * colliderRadius) {
                Vector3 delta = new Vector3(deltaX, deltaY, 0);
                hit = hit || Physics.Raycast(transform.position + delta, -Vector3.up, collider.bounds.extents.y + errorMargin);
            }
        }

        //return Physics.Raycast(transform.position, -Vector3.up, collider.bounds.extents.y + errorMargin);
        return hit;
    }
}
