using UnityEngine;
using System.Collections;

public class SnowballController : MonoBehaviour {

	public float speed = 10f;

	// @TODO(Llewellin): Determine how much damage should be taken from a snowball
	public float damage = 25f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (Vector3.forward * speed * Time.deltaTime);
	}
	

	void OnCollisionEnter (Collision collision)
	{
		if (collision.gameObject.name == "Mountain") {
			Destroy (this.gameObject);
		}

		// Transform hitTransform = FindClosestHitObject(ray, out hitPoint);
			
		Transform hitTransform = collision.transform;

		// If we hit something, lets resolve the hit
		if (hitTransform != null) {
			Debug.Log ("HIT: " + collision.gameObject.name);
			Health h = hitTransform.GetComponent<Health> ();

			// check if the things parent has a Health object
			while (h == null && hitTransform.parent) {
				hitTransform = hitTransform.parent;
				h = hitTransform.GetComponent<Health> ();
			}

			// The thing we hit has a health component
			// Tell the thing we hit to take damage
			if (h != null) {
				// TODO h.resolve(damage);
				// h.TakeDamage (damage);
				// next line equivalent of calling h.takeDamage except over network
				PhotonView pv = h.GetComponent<PhotonView> ();
				if (pv == null) {
					Debug.Log ("PlayerShooting: PhotonView is null");
				} 
				else {

					//get the thing that was hit
//					int targetID = pv.owner.ID;
//					PhotonPlayer target = PhotonPlayer.Find (targetID);

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
						Debug.Log ("FRIENDLY FIRE STAHP IT");
					}
				}
			}
		} 
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

