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

        // Debug.Log ("firing gun");

        // Didn't hit anything (maybe empty space) but we want a visual fx anyway
        if (fxManager != null) {
            Vector3 hitPoint = Camera.main.transform.position + (Camera.main.transform.forward * 100f);

			// @TODO(Llewellin): change the startPos to be where the snowball launches.
			// This is set forward slightly so the snowball doesn't come from the camera,
			// which inturn hits us and causes complications in onCollisionEnter()
			Vector3 startPos = Camera.main.transform.position + (Camera.main.transform.forward * 5f);
			fxManager.GetComponent<PhotonView>().RPC("SnowballFX", PhotonTargets.All, startPos, hitPoint);
        }

        coolDown = fireRate;

    }

    Transform FindClosestHitObject(Ray ray, out Vector3 hitPoint) {

        RaycastHit[] hits = Physics.RaycastAll(ray);

        Transform closestHit = null;
        float distance = 0;
        hitPoint = Vector3.zero;

        foreach (RaycastHit hit in hits) {
            // if we don't hit ourselves & its the closest thing to us
            if (hit.transform != this.transform && (closestHit == null || hit.distance < distance)) {
                closestHit = hit.transform;
                distance = hit.distance;
                hitPoint = hit.point;
            }
        }

        //return the closest hit object or null if nothing was hit
        return closestHit;
    }
}
