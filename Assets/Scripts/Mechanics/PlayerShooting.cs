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

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        Transform hitTransform;
        Vector3 hitPoint;

        hitTransform = FindClosestHitObject(ray, out hitPoint);

        // If we hit something, lets resolve the hit
        if (hitTransform != null) {
            Debug.Log("HIT: " + hitTransform.name);
            Health h = hitTransform.GetComponent<Health>();

            // check if the things parent has a Health object
            while (h == null && hitTransform.parent) {
                hitTransform = hitTransform.parent;
                h = hitTransform.GetComponent<Health>();
            }

            if (h != null) {
                // TODO h.resolve(damage);
                // h.TakeDamage (damage);
                // next line equivalent of calling h.takeDamage except over network
                PhotonView pv = h.GetComponent<PhotonView>();
                if (pv == null) {
                    Debug.Log("PlayerShooting: PhotonView is null");
                } else {

                    // get the object that was hit
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
                        pv.RPC("TakeDamage", PhotonTargets.AllBuffered, damage);
                        // Debug.Log ("Teams don't match, take damage");
                    } else {
                        // Targeting same team
                        // Debug.Log ("FRIENDLY FIRE STAHP IT");
                    }
                }
            }

            if (fxManager != null) {
                fxManager.GetComponent<PhotonView>().RPC("SnowballFX", PhotonTargets.All, Camera.main.transform.position, hitPoint);
            }

        } else {
            // Didn't hit anything (maybe empty space) but we want a visual fx anyway
            if (fxManager != null) {
                hitPoint = Camera.main.transform.position + (Camera.main.transform.forward * 100f);
                fxManager.GetComponent<PhotonView>().RPC("SnowballFX", PhotonTargets.All, Camera.main.transform.position, hitPoint);
            }
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
