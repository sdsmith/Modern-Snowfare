using UnityEngine;
using System.Collections;



[RequireComponent(typeof(Rigidbody))]
public class SnowballController : MonoBehaviour {

	public float speed;// m/s(?)
    /** @DEBUG(sdsmith): For collision stats. */
    private static ulong nonTerrainCollisionCount = 0;

	//@BUG:(Llewellin): Removing the public status from this variable causes
	// the players to take 0 damage...why?
	public float damage;

    /** PhotonView ID of the player that threw the snowball. */
    private int throwerViewID;

    // Use this for initialization
    void Start () {
        Rigidbody rb = GetComponent<Rigidbody>();

        // Apply initial forward velocity to the snowball at creation and let 
        // the physics engine do its work.
        rb.AddRelativeForce(Vector3.forward * speed, ForceMode.VelocityChange);
    }
	

	void Update () {
	}


    void OnCollisionEnter(Collision collision) {

        // Play snowball impact sound
        //
        // @NOTE(sdsmith): Must use AudioSource.PlayClipAtpoint so the clip 
        // persists after the destruction of the object. If we ere using an 
        // AudioSource component, the clip would stop playing once that 
        // component was destroyed.
        AudioClip impactClip = AudioClips.GetRand(AudioClips.snowballImpacts);
        AudioSource.PlayClipAtPoint(impactClip, transform.position);


        if (collision.gameObject.name == "Mountain") {
            // @NOTE(sdsmith): @PERFORMANCE(sdsmith): Note that 'Destroy' 
            // delayed at least until the end of the frame update cycle. If we
            // hit the terrain, we know we don't need to do any additional 
            // collision calculations.
            //
            // @TODO(sdsmith): This breaks Network manager when we take the advice 
            // of the above comment and end the function here. Who knows.
            //                                                        - 2017-02-27
            Destroy(this.gameObject);
        }

        // @DEBUG(sdsmith): Update debug overlay
        DebugOverlay.AddAttr("snowball collision count", (++nonTerrainCollisionCount).ToString());

        Transform hitTransform = collision.transform;

        // If we hit something, lets resolve the hit
        if (hitTransform != null) {
            // Debug.Log ("HIT: " + collision.gameObject.name);
            Health h = hitTransform.GetComponent<Health>();

            // check if the things parent has a Health object
            while (h == null && hitTransform.parent) {
                hitTransform = hitTransform.parent;
                h = hitTransform.GetComponent<Health>();
            }

            // The thing we hit has a health component
            // Tell the thing we hit to take damage
            if (h != null) {
                PhotonView pv = h.GetComponent<PhotonView>();
                if (pv == null) {
                    Debug.Log("PlayerShooting: PhotonView is null");
                } else {

                    //get the thing that was hit
                    PhotonPlayer target = pv.owner;

                    // Get teams
                    PunTeams.Team ourTeam = PhotonNetwork.player.GetTeam();
                    PunTeams.Team theirTeam = PunTeams.Team.none; // default objects to no team

                    // Check if the target object has an owner
                    // @NOTE(sdsmith): PhotonView.owner is null for scene objects
                    // https://doc-api.photonengine.com/en/pun/current/class_photon_view.html#ad696cb93fb9835d633b9def970650edc
                    if (target != null) {
                        // Target has an owner (not a scene object), set its team
                        theirTeam = target.GetTeam();
                    }

                    if (ourTeam != theirTeam) {
                        // Not targeting same team
                        pv.RPC("TakeDamage", PhotonTargets.AllBuffered, damage, throwerViewID);

                        // Play hit sound if our local client threw the snowball
                        PhotonView throwerPV = PhotonView.Find(throwerViewID);
                        if (throwerPV && Util.localPlayer == throwerPV.gameObject) {
                            throwerPV.gameObject.GetComponent<PlayerController>().PlayHitNotification();
                        }
                    } else {
                        // Targeting same team
                    }
                }
            }
            Destroy(this.gameObject);
        }
    }

	public void SetSnowballDamage(float damage) {
		this.damage = damage;
	}

    public void SetThrower(int playerViewID) {
        throwerViewID = playerViewID;
    }
}

