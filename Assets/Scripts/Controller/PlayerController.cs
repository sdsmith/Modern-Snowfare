using UnityEngine;
using System.Collections;



[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour {

    public float speed = 10.0F;
    public float jumpSpeed = 15.0F;

    private new Rigidbody rigidbody;
    private new CapsuleCollider collider;

    

    void Start () {
        // Component references
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();

        // @TODO(sdsmith): Move this out of here. Has nothing to do with the player.
        // Lock cursor to window (hides OS cursor graphic)
        Cursor.lockState = CursorLockMode.Locked;
    }
    

    void Update () {
        // Character movement 
        Vector3 moveDirection = Vector3.zero;
        // @NOTE(sdsmith): Movement force should keep the existing vertical
        // velocity.
        Vector3 moveVelocity;
        
        
        if (IsGrounded()) {
            float forwardMove = Input.GetAxis("Vertical");
            float sideMove = Input.GetAxis("Horizontal");

            moveDirection = new Vector3(sideMove, 0, forwardMove);

            // Add speed to each direction in preportion to what direction we are moving.
            // @NOTE(sdsmith): This ensures that speed is always the same.
            moveDirection.Normalize();            
            moveVelocity = moveDirection * speed;
            
            // Jump
            if (Input.GetButtonDown("Jump")) {
                // Add upward velocity (jump!)
                moveVelocity.y = jumpSpeed;
            } else {
                moveVelocity.y = rigidbody.velocity.y;
            }

            // Transform from Local to World space
            moveVelocity = transform.TransformDirection(moveVelocity);
        
            // Move
            rigidbody.velocity = moveVelocity;
        }


        // @TODO(sdsmith): Refactor non-character specific input out of here.
        if (Input.GetKeyDown("escape")) {
            Cursor.lockState = CursorLockMode.None;
        }
    }


    /**
     * Return true if the character is on the ground, false otherwise.
     */
    private bool IsGrounded() {
        /*
         * @NOTE(sdsmith): There was issues with steep slopes when one ray was
         * shot downward from the center. The fix is shooting one ray every 90
         * degrees of the circumference of the collider. 
         *                                                         - 2017-02-13
         * 
         * @PERFORMANCE(sdsmith): Do we want to unroll this, short circuit it, 
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

        return hit;
    }
}
