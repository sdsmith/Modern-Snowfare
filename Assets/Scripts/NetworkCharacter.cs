using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCharacter : Photon.MonoBehaviour {

	Vector3 realPosition = Vector3.zero;
	Quaternion realRotation = Quaternion.identity;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (photonView.isMine) {
			// Do nothing -- the character motor/input/etc... is moving us
		} else {
			// Smooths character over network
			transform.position = Vector3.Lerp (transform.position, realPosition, 0.1f);
			transform.rotation = Quaternion.Lerp (transform.rotation, realRotation, 0.1f);
		}

	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){

		// Debug.Log ("OnPhotonSerializeView");

		if (stream.isWriting) {
			// This is our player. Send actual position to network

			stream.SendNext (transform.position);
			stream.SendNext (transform.rotation);

		} else {
			// This is someone else's player. Recieve their position (as of a few 
			// milliseconds ago, and update our version of that player.
			realPosition = (Vector3) stream.ReceiveNext ();
			realRotation = (Quaternion)stream.ReceiveNext ();

		}
	}
}
