using UnityEngine;
using System.Collections;



[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(PlayerShooting))]
public class PlayerController : BaseController {
	/*
	 * @NOTE(Llewellin): These are the default stats for characters.
	 * Characters will override these stats as needed, so when using
	 * these variables don't access them directly, call their Get() method.
	 */

	protected float speed = 10.0f;
	protected float damage = 1.0f;
    public float jumpSpeed;

    private new Rigidbody rigidbody;
    private new CapsuleCollider collider;
    private GUITexture healthBarGUITexture;

    protected void Start () {
        // Component references
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();

        jumpSpeed = 7f;

		// @DEBUG(Llewellin): Add entry to debug overlay
		DebugOverlay.AddAttr("speed", GetSpeed().ToString());
		DebugOverlay.AddAttr("damage", GetDamage().ToString());
    }

    void FixedUpdate () {
        // Character movement
        {
            Vector3 moveDirection, moveVelocity;
            float forwardMove = 0;
            float sideMove = 0;
            bool isGrounded = IsGrounded();
            bool cursorLocked = Cursor.lockState == CursorLockMode.Locked;

            // Only influence movement if cursor is locked in the window.
            if (cursorLocked) {
                forwardMove = Input.GetAxis("Vertical");
                sideMove = Input.GetAxis("Horizontal");
            }

            moveDirection = new Vector3(sideMove, 0, forwardMove);

            // Add speed to each direction in proportion to what direction we are moving.
            // @NOTE(sdsmith): This ensures that speed is always the same.
            moveDirection.Normalize();


            /* TODO(sdsmith): Need to apply a force to the character when they are in the air, and an instantanous velocity when they are on the ground. This gets rid of the unexpected 'jolt' when you release a key in the air. */
            // Calculate the velocity
			moveVelocity = (isGrounded ?
							moveDirection * GetSpeed() :      // Ground speed is full
							moveDirection * GetSpeed() / 3f); // Air speed is 1/3 regluar speed

            // Transform move direction from Local to World space
            moveVelocity = transform.TransformDirection(moveVelocity);

            // Add vertical velocity
            // NOTE(sdsmith): Rigidbody records velocity in world space, and
            // does not need to be transformed.
            moveVelocity.y = rigidbody.velocity.y;

            // Jump (only influence movement is cursor is locked in the window)
            if (cursorLocked && isGrounded && Input.GetButtonDown("Jump")) {
                // Add upward velocity (jump!)
                moveVelocity.y += jumpSpeed;
            }

            // Move
            rigidbody.velocity = moveVelocity;
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

	// @Note(Llewellin): Overridden in FlashController
	public virtual float GetSpeed() {
        return speed;
    }

	// @Note(Llewellin): Overriden in SniperController
	public virtual float GetDamage() {
		return damage;
	}
}
