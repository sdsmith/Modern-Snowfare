using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour {

	public float fireRate = 0.5f;
	float coolDown = 0;
	public float damage = 25f;

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

		Debug.Log ("firing gun");

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
				}
				else {
					pv.RPC("TakeDamage", PhotonTargets.AllBuffered, damage);
				}
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
