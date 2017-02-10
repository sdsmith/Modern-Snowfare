﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour {

	SpawnSpot[] spawnSpots;

	public bool offlineMode = false;

	string prefabName = "Player_NEW";

	// Use this for initialization
	void Start () {
		Debug.Log ("Start");
		spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
		Connect ();
	}

	void Connect() {
		if (offlineMode) {
			PhotonNetwork.offlineMode = true;
			OnJoinedLobby ();
		} else {
			PhotonNetwork.ConnectUsingSettings ("MultiFPS v001");
		}
	}

	void OnGUI() {
		GUILayout.Label( PhotonNetwork.connectionStateDetailed.ToString() );
	}

	void OnJoinedLobby() {
		Debug.Log ("OnJoinedLobby");
		PhotonNetwork.JoinRandomRoom();
	}

	void OnPhotonRandomJoinFailed() {
		Debug.Log ("OnPhotonRandomJoinFailed");
		PhotonNetwork.CreateRoom( null );
	}

	void OnJoinedRoom() {
		Debug.Log ("OnJoinedRoom");

		SpawnMyPlayer();
	}

	void SpawnMyPlayer() {
		if (spawnSpots == null) {
			Debug.Log ("SpawnMyPlayer: SpawnSpots == null");
			return;
		}
		SpawnSpot mySpawnSpot = spawnSpots [Random.Range (0, spawnSpots.Length)];
		GameObject myPlayerGO = (GameObject)PhotonNetwork.Instantiate (prefabName, mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);
		myPlayerGO.GetComponent<characterController> ().enabled = true;
		((MonoBehaviour)myPlayerGO.GetComponent ("PlayerShooting")).enabled = true;
		myPlayerGO.transform.FindChild("Main Camera").gameObject.SetActive(true);
	}

}
