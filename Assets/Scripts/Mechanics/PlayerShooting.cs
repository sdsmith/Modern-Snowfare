using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour {

    public float fireRate = 0.5f;
    float coolDown = 0;
    public float damage = 25f;
    FXManager fxManager;

    void Start() {
        fxManager = GameObject.FindObjectOfType<FXManager>();

        if (fxManager == null) {
            Debug.LogError("No fxManager");
        }
    }

    // Update is called once per frame
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

        // Fire the snowball
        if (fxManager != null) {
            Vector3 hitPoint = Camera.main.transform.position + (Camera.main.transform.forward * 2f);

			/*
			 * @TODO(Llewellin): change the startPos to be where the snowball launches.
			 * @NOTE(Llewellin): startPos is set forward slightly so the snowball doesn't come from the camera.
			 * The camera is inside the player, which causes the snowball to hit us and causes complications in 
			 * SnowballController:onCollisionEnter().
			 * i.e we collide with ourselves and take damage, then die
			 */

			Vector3 startPos = Camera.main.transform.position + (Camera.main.transform.forward * 1.1f);
			fxManager.GetComponent<PhotonView>().RPC("SnowballFX", PhotonTargets.All, startPos, hitPoint);
        }

        coolDown = fireRate;

    }
}
