using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NetworkManager : MonoBehaviour {

	public GameObject standbyCamera;
	SpawnSpot[] spawnSpots;

	// This is the name of the prefab we are going to create into the game.
	// This should be changed to match whichever character the player chooses to be
	string prefabName = "Player";

	bool connecting = false;
	List<string> chatMessages;
	int maxChatMessages = 5;

	public float respawnTimer = 0f;

	// Use this for initialization
	void Start () {
		spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
		PhotonNetwork.player.NickName = PlayerPrefs.GetString("Username", "Modern Snowfare");
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
			GUILayout.BeginArea (new Rect (0, 0, Screen.width, Screen.height));
			GUILayout.BeginVertical ();
			GUILayout.FlexibleSpace ();

			foreach(string msg in chatMessages){
				GUILayout.Label (msg);
			}

			GUILayout.EndVertical ();
			GUILayout.EndArea ();
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
		SpawnMyPlayer();
	}

	/* 
	 * We have joined a room so lets create a player.
	 * Add a chatmessage saying the player nickname has joined
	 * get a spawn point and create a player at that point
	 */
	void SpawnMyPlayer() {

		AddChatMessage ("Spawning player: " + PhotonNetwork.player.NickName);

		if (spawnSpots == null) {
			Debug.Log ("SpawnMyPlayer: SpawnSpots == null");
			return;
		}

		// @TODO(Llewellin): change random spawn point to join a team
		SpawnSpot mySpawnSpot = spawnSpots [Random.Range (0, spawnSpots.Length)];
		GameObject myPlayerGO = (GameObject)PhotonNetwork.Instantiate (prefabName, mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);
		standbyCamera.SetActive(false);

		myPlayerGO.GetComponent<PlayerController> ().enabled = true;
		((MonoBehaviour)myPlayerGO.GetComponent ("PlayerShooting")).enabled = true;
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
}