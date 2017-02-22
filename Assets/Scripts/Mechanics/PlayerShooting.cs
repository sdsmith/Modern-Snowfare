using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour {

	public float fireRate = 0.5f;
	float coolDown = 0;
	public float damage = 25f;
	FXManager fxManager;

	void Start(){
		fxManager = GameObject.FindObjectOfType<FXManager> ();

		if (fxManager == null) {
			Debug.LogError ("No fxManager");
		}
	}

	// Update is called once per frame
	void Update () {

		coolDown -= Time.deltaTime;

		if(Input.GetButton("Fire1")){
			Fire ();
		}
	}

	void Fire(){
		if (coolDown > 0) {
			return;
		}

		// Debug.Log ("firing gun");

		Ray ray = new Ray (Camera.main.transform.position, Camera.main.transform.forward);
		Transform hitTransform;
		Vector3 hitPoint;

		hitTransform = FindClosestHitObject (ray, out hitPoint);

		// If we hit something, lets resolve the hit
		if (hitTransform != null) {
			Debug.Log ("HIT: " + hitTransform.name);
			Health h = hitTransform.GetComponent<Health> ();

			// check if the things parent has a Health object
			while (h == null && hitTransform.parent) {
				hitTransform = hitTransform.parent;
				h = hitTransform.GetComponent<Health> ();
			}

			if (h != null) {
				// TODO h.resolve(damage);
				// h.TakeDamage (damage);
				// next line equivalent of calling h.takeDamage except over network
				PhotonView pv = h.GetComponent<PhotonView> ();
				if (pv == null) {
					Debug.Log ("PlayerShooting: PhotonView is null");
				} else {

					//this code fetches the PhotonPlayer system and assigns it to the variable "target"
					PhotonPlayer target = pv.owner;
//					PhotonView rootView = hit.rigidbody.gameObject.transform.root.GetComponent<PhotonView>();
//					int targetID = rootView.owner.ID;
//					PhotonPlayer target = PhotonPlayer.Find(targetID);[/code2]

					if (PhotonNetwork.player.GetTeam () != target.GetTeam ()) {
						pv.RPC ("TakeDamage", PhotonTargets.AllBuffered, damage);
						Debug.Log ("Teams don't match, take damage");
					} else {
						Debug.Log ("FRIENDLY FIRE STAHP IT");
					}

					// If other doesn't have a team, or I don't have a team, or the teams are different, take damage
//					if (tm == null || tm.teamID == 0 || myTM == null || myTM.teamID == 0 || tm.teamID != myTM.teamID) {
//						pv.RPC ("TakeDamage", PhotonTargets.AllBuffered, damage);
//					}
				}
			}

			if (fxManager != null) {
				fxManager.GetComponent<PhotonView> ().RPC ("SnowballFX", PhotonTargets.All, Camera.main.transform.position, hitPoint);
			}

		} 
		else {
			// Didn't hit anything (maybe empty space) but we want a visual fx anyway
			if (fxManager != null) {
				hitPoint = Camera.main.transform.position + (Camera.main.transform.forward * 100f);
				fxManager.GetComponent<PhotonView> ().RPC ("SnowballFX", PhotonTargets.All, Camera.main.transform.position, hitPoint);
			}
		}

		coolDown = fireRate;
		
	}

	Transform FindClosestHitObject(Ray ray, out Vector3 hitPoint){

		RaycastHit[] hits = Physics.RaycastAll (ray);

		Transform closestHit = null;
		float distance = 0;
		hitPoint = Vector3.zero;

		foreach (RaycastHit hit in hits) {
			// if we don't hit ourselves & its the closest thing to us
			if(hit.transform != this.transform && (closestHit == null || hit.distance < distance)){
				closestHit = hit.transform;
				distance = hit.distance;
				hitPoint = hit.point;
			}
		}

		//return the closest hit object or null if nothing was hit
		return closestHit;
	}
}
