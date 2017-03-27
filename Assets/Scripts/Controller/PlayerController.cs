using UnityEngine;
using System.Collections;



[RequireComponent(typeof(AudioSource))]
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

    /** True if the player is currently jumping. */
    private bool isJumping;
    /** True if the player is airborn after jumping. */
    /* 
     * @NOTE(sdsmith): When a player starts a jump, they won't necessarily be 
     * considered 'off the ground' in the next frame due to the error margin 
     * in the calculation of 'IsGrounded()'. This variable is used to determine
     * at what period the player is actually off the ground during a jump.
     */
    private bool isAirborn;

    private AudioSource audioSource;

    private new Rigidbody rigidbody;
    private new CapsuleCollider collider;
    private GUITexture healthBarGUITexture;


    protected void Start () {
        // Component references
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        audioSource = GetComponent<AudioSource>();

        jumpSpeed = 7f;
        isJumping = IsGrounded();
        isAirborn = isJumping;

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

            // Determine if the player is in the air
            // @TODO(sdsmith): Doesn't always seem to recognize when a player has hit the ground.
            if (isAirborn && isGrounded) {
                // Player was airborn, but has now hit the ground
                isAirborn = false;
                isJumping = false;

                // Play landing sound
                AudioSource.PlayClipAtPoint(AudioClips.land, transform.position);
            } else if (isJumping && !isAirborn && !isGrounded) {
                // Player is jumping, and has just left the ground
                isAirborn = true;
            }

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
                // Play jump sound
                AudioSource.PlayClipAtPoint(AudioClips.jump, transform.position);

                // Add upward velocity (jump!)
                moveVelocity.y += jumpSpeed;

                isJumping = true;
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

    public void PlayHitNotification() {
        // @NOTE(sdsmith): There is a potential case where this function could
        // be called after the player dies. Must check to avoid nullptr.
        if (audioSource) {
            audioSource.PlayOneShot(AudioClips.targetHit, 0.25f);
        }
    }

    public void PlayKillNotification() {
        // @NOTE(sdsmith): There is a potential case where this function could
        // be called after the player dies. Must check to avoid nullptr.
        if (audioSource) {
            audioSource.PlayOneShot(AudioClips.playerKill, 0.6f);
        }
    }

    public void PlayTakenDamageNotification() {
        // @NOTE(sdsmith): There is a potential case where this function could
        // be called after the player dies. Must check to avoid nullptr.
        if (audioSource) {
            AudioClip clip = AudioClips.GetRand(AudioClips.maleGrunts);
            audioSource.PlayOneShot(clip);
        }
    }

    // @Note(Llewellin): Overridden in FlashController
    public virtual float GetSpeed() {
        return speed;
    }

	// @Note(Llewellin): Overriden in SniperController
	public virtual float GetDamage() {
		return damage;
	}

	public  void SetSpeed(float new_Speed) {
		this.speed = new_Speed;
	}
		
	/// The string for the kill count custom property
	const string killCountProperty = "KillCount";
	int m_killCount = 0;

	public int killCount
	{
		get
		{
			PhotonView view = GetComponent<PhotonView> ();
			return Util.GetCustomProperty<int>( view, killCountProperty, m_killCount, 0 );
		}
		set
		{
			PhotonView view = GetComponent<PhotonView> ();
			Util.SetCustomProperty<int>( view, killCountProperty, ref m_killCount, value );
		}
	}


	/// The string for the kill count custom property
	const string deathCountProperty = "DeathCount";
	int m_DeathCount = 0;

	public int deathCount
	{
		get
		{
			PhotonView view = GetComponent<PhotonView> ();
			return Util.GetCustomProperty<int>( view, deathCountProperty, m_DeathCount, 0 );
		}
		set
		{
			PhotonView view = GetComponent<PhotonView> ();
			Util.SetCustomProperty<int>( view, deathCountProperty, ref m_DeathCount, value );
		}
	}

	const string flagCaptureCountProperty = "FlagCaptureCount";
	int m_flagCaptureCount = 0;

	public int flagCaptureCount
	{
		get
		{
			PhotonView view = GetComponent<PhotonView> ();
			return Util.GetCustomProperty<int>( view, flagCaptureCountProperty, m_flagCaptureCount, 0 );
		}
		set
		{
			PhotonView view = GetComponent<PhotonView> ();
			Util.SetCustomProperty<int>( view, flagCaptureCountProperty, ref m_flagCaptureCount, value );
		}
	}
}
