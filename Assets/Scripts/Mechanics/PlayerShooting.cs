using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PlayerController))]
public class PlayerShooting : MonoBehaviour {

    public float fireRate = 0.5f;
    float coolDown = 0;
    FXManager fxManager;
    private static ulong firedSnowballsCount = 0;

	PlayerController pc;
	float snowballDamage;

    private AudioSource audioSource;

    void Start() {
        fxManager = GameObject.FindObjectOfType<FXManager>();
        if (fxManager == null) {
            Debug.LogError("No fxManager");
        }

		pc = GetComponent<PlayerController> ();
		if (pc == null) {
			Debug.LogError ("Player Controller is null");
		}

        audioSource = GetComponent<AudioSource>();

		snowballDamage = pc.GetDamage();
    }


    void Update() {

        coolDown -= Time.deltaTime;

        if (Input.GetButton("Fire1")) {
            Fire();
        }
    }


    void Fire() {
        if (coolDown > 0) {
            return;
        }

        // Play fire/throw sound
        AudioClip clip = AudioClips.GetRand(AudioClips.playerThrow);
        audioSource.PlayOneShot(clip);

        // Fire the snowball
        if (fxManager != null) {
            // @DEBUG(sdsmith): Update debug stats
            DebugOverlay.AddAttr("snowballs fired count", (++firedSnowballsCount).ToString());

            /*
			 * @TODO(Llewellin): change the startPos to be where the snowball launches.
			 * @NOTE(Llewellin): startPos is set forward slightly so the snowball doesn't come from the camera.
			 * The camera is inside the player, which causes the snowball to hit us and causes complications in 
			 * SnowballController:onCollisionEnter().
			 * i.e. we collide with ourselves and take damage, then die
			 */

            // @TODO(sdsmith): Set offset of the snowball spawn location to be relative to the size of the player's 
            // collider plus the width of the snowball collider and some error margin to make it change proof.
			Vector3 startPos = Camera.main.transform.position + (Camera.main.transform.forward * 1.1f);
            int playerViewID = this.gameObject.GetComponent<PhotonView>().viewID;
            fxManager.GetComponent<PhotonView>().RPC("SnowballFX", PhotonTargets.All, startPos, Camera.main.transform.rotation, snowballDamage, playerViewID);
        }

        coolDown = fireRate;
    }

    public void SetFireRate(float rate)
    {
        this.fireRate = rate;
    }
}
