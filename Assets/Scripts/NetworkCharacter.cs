using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCharacter : Photon.MonoBehaviour {

	Vector3 realPosition;
	Quaternion realRotation;

	bool gotFirstUpdate = false;

	// Use this for initialization
	void Start () {
		realPosition = transform.position;
		realRotation = transform.rotation;
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

	/*
	 * FROM https://doc-api.photonengine.com/en/pun/current/group__public_api.html
	 * 
	 * Implement to customize the data a PhotonView regularly synchronizes. 
	 * Called every 'network-update' when observed by Photon View.
	 * Implementing this method, you can customize which data a PhotonView regularly synchronizes. 
	 * Your code defines what is being sent (content) and how your data is used by receiving clients.
	 * OnPhotonSerializeView only gets called when it is assigned to a PhotonView as PhotonView.observed script.
	 * It will be in "writing" mode" on the client that controls a PhotonView (PhotonStream.isWriting == true) 
	 * and in "reading mode" on the remote clients that just receive that the controlling client sends.
	 * 
	 * For example. When our player moves, we want to send the characters position and rotation to the network (isWriting)
	 * When someone else moves we want to recieve their position and rotation (isReading)
	 */ 

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

			// If we've never gotten an update, avoid the slide from (0,0,0) to real position
			// Instantly teleport to correct position
			if (!gotFirstUpdate) {
				transform.position = realPosition;
				transform.rotation = realRotation;
				gotFirstUpdate = true;
			}

		}
	}
}
