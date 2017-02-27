﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NetworkManager : MonoBehaviour {

	public GameObject standbyCamera;
    public bool debug = false;
	SpawnSpot[] spawnSpots;

	// This is the name of the prefab we are going to create into the game.
	// This should be changed to match whichever character the player chooses to be
	string prefabName;

	bool connecting = false;
	List<string> chatMessages;
	int maxChatMessages = 5;

	public float respawnTimer = 0f;

	bool hasPickedTeam = false;

	// Use this for initialization
	void Start () {
        // Set the player prefab
        if (debug) {
            prefabName = "PlayerDebug";
        } else {
            prefabName = "Player";
        }

		spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
		PhotonNetwork.player.NickName = PlayerPrefs.GetString("Username", "Modern Snowfare");
		PhotonNetwork.player.SetTeam (PunTeams.Team.none);
		chatMessages = new List<string>();
	}

	// Called when the player disconnects
	void OnDestroy(){

		// set the default name to be the name the player chose last
		PlayerPrefs.SetString ("Username", PhotonNetwork.player.NickName);
	}

	// Add a chat message (currently appearing in the bottom left corner when a player spawns)
	public void AddChatMessage(string m){

		// Send all buffered messages (messages that have been sent before the player joined)
		GetComponent<PhotonView> ().RPC ("AddChatMessage_RPC", PhotonTargets.AllBuffered, m);
	}

	/*
	 * NOTE: [PunRPC] means everyone connected to the network recieves this update
	 * When a chatmessage is sent, everyone gets this message
	 * 
	 * ex. To call this function use:
	 * GetComponent<PhotonView> ().RPC ("AddChatMessage_RPC", PhotonTargets.AllBuffered, <parameter>);
	 */

	[PunRPC]
	void AddChatMessage_RPC(string m) {
		//When the max chat messages have been recieved, remove the oldest one to make room
		while (chatMessages.Count >= maxChatMessages) {
			chatMessages.RemoveAt (0);
		}
		chatMessages.Add (m);
	}

	/*
	 * Photon Network calls the Connect() function. 
	 * ConnectUsingSettings() takes a string parameter to identify the connection. 
	 * This string must be the same for everyone to join the same instance of the game
	 */
	void Connect() {
		PhotonNetwork.ConnectUsingSettings ("MultiFPS v001");
	}

	void OnGUI() {

		// Status of the connection found in the top left corner.
		GUILayout.Label( PhotonNetwork.connectionStateDetailed.ToString() );

		// connecting variable because PhotonNetwork has delay which keeps button longer than
		// they should be
		if (!PhotonNetwork.connected && !connecting) {
			// TODO Look into NGUI

			// main menu temporary GUI. 
			// currently has username, offline mode and start game
			GUILayout.BeginArea (new Rect (0, 0, Screen.width, Screen.height));
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			GUILayout.BeginVertical ();
			GUILayout.FlexibleSpace ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Username: ");
			PhotonNetwork.player.NickName = GUILayout.TextField (PhotonNetwork.player.NickName);
			GUILayout.EndHorizontal ();

			// Joins a game locally for offline testing.
			if(GUILayout.Button("Offline mode")){
				connecting = true;
				PhotonNetwork.offlineMode = true;
				OnJoinedLobby ();
			}
			if(GUILayout.Button("Start Game")){
				connecting = true;
				Connect ();
			}
			GUILayout.FlexibleSpace ();
			GUILayout.EndVertical ();
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();
			GUILayout.EndArea ();
		}

		// send the chat label at the bottom left
		if (PhotonNetwork.connected && !connecting) {

			if (hasPickedTeam) {
				GUILayout.BeginArea (new Rect (0, 0, Screen.width, Screen.height));
				GUILayout.BeginVertical ();
				GUILayout.FlexibleSpace ();

				foreach (string msg in chatMessages) {
					GUILayout.Label (msg);
				}

				GUILayout.EndVertical ();
				GUILayout.EndArea ();
			} 
			else {
				// Player has not yet selected a team

				GUILayout.BeginArea (new Rect (0, 0, Screen.width, Screen.height));
				GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				GUILayout.BeginVertical ();
				GUILayout.FlexibleSpace ();

				if(GUILayout.Button("Red Team")){
					PhotonNetwork.player.SetTeam (PunTeams.Team.red);
					SpawnMyPlayer ();
				}
				if(GUILayout.Button("Blue Team")){
					PhotonNetwork.player.SetTeam (PunTeams.Team.blue);
					SpawnMyPlayer ();
				}
				GUILayout.FlexibleSpace ();
				GUILayout.EndVertical ();
				GUILayout.FlexibleSpace ();
				GUILayout.EndHorizontal ();
				GUILayout.EndArea ();

			}
		}
	}

	/*
	 * When the network connects, Photon calls OnJoinedLobby().
	 * Here we try to join a random room. Currently we only have 
	 * one room. So if we're the first player to join, no room 
	 * has been created yet. This will call OnPhotonRandomJoinFailed().
	 * Otherwise, OnJoinedRoom() is called.
	 */
	void OnJoinedLobby() {
		Debug.Log ("OnJoinedLobby");
		PhotonNetwork.JoinRandomRoom();
	}

	/*
	 * We are the first player, so joining a random room failed
	 * because there is no room yet. Lets create a room. When
	 * we create a room, we join it. So OnJoinedRoom() is called.
	 */
	void OnPhotonRandomJoinFailed() {
		Debug.Log ("OnPhotonRandomJoinFailed");
		PhotonNetwork.CreateRoom( null );
	}

	/*
	 * We have joined a room, yay!
	 * Lets call SpawnMyPlayer() which will create our player in the room
	 * We are also no longer connecting so set the variable to false. 
	 * This will remove the 
	 */
	void OnJoinedRoom() {
		Debug.Log ("OnJoinedRoom");

		connecting = false;
		// SpawnMyPlayer();
	}

	/* 
	 * We have joined a room so lets create a player.
	 * Add a chatmessage saying the player nickname has joined
	 * get a spawn point and create a player at that point
	 */
	void SpawnMyPlayer() {
		hasPickedTeam = true;

		AddChatMessage ("Spawning player: " + PhotonNetwork.player.NickName);

		SpawnSpot mySpawnSpot = null;

		// sanity check
		if (spawnSpots == null || spawnSpots.Length != 2) {
			Debug.LogError ("Incorrect amount of spawn points");
			return;
		}

		// Set the spawn point based on the team you're on
		// @TODO(Llewellin) cleanup how we get spawnpoints.
		// (If each team can have multiple spawn points take this into consideration)
		if (spawnSpots [0].teamId == 1 && PhotonNetwork.player.GetTeam() == PunTeams.Team.red) {
			mySpawnSpot = spawnSpots [0];
		} else {
			mySpawnSpot = spawnSpots [1];
		}
			
		GameObject myPlayerGO = (GameObject)PhotonNetwork.Instantiate (prefabName, mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);
		standbyCamera.SetActive(false);

		myPlayerGO.GetComponent<PlayerController> ().enabled = true;
		((MonoBehaviour)myPlayerGO.GetComponent ("PlayerShooting")).enabled = true;

		int viewID = myPlayerGO.gameObject.GetPhotonView ().viewID;
		GetComponent<PhotonView> ().RPC ("SetTeamIcon", PhotonTargets.AllBuffered, viewID);

		myPlayerGO.transform.FindChild("Main Camera").gameObject.SetActive(true);
	}

	// NOTE: Update is called once per frame
	void Update(){
		if (respawnTimer > 0) {
			respawnTimer -= Time.deltaTime;

			// When enough time has passed, respawn the player
			if (respawnTimer <= 0) {
				SpawnMyPlayer ();
			}
		}
	}

	// Set the color of the icon on top of the player to match their team
	// Can't pass the gameObject so pass the reference ID
	[PunRPC]
	void SetTeamIcon(int viewID){
		GameObject playerObject = PhotonView.Find (viewID).gameObject;

		if (playerObject == null) {
			Debug.Log ("playerObject is null");
			return;
		}

		GameObject teamTag = playerObject.transform.FindChild ("TeamTag").gameObject;

		if (playerObject.GetPhotonView().owner.GetTeam() == PunTeams.Team.red) {
			teamTag.GetComponent<MeshRenderer> ().material.color = Color.red;
		} else {
			teamTag.GetComponent<MeshRenderer> ().material.color = Color.blue;
		}
	}
}