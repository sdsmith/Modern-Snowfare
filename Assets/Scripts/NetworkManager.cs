using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour {

	public GameObject standbyCamera;
	SpawnSpot[] spawnSpots;

	string prefabName = "Player_NEW";

	bool connecting = false;
	List<string> chatMessages;
	int maxChatMessages = 5;

	// Use this for initialization
	void Start () {
		spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
		PhotonNetwork.player.name = PlayerPrefs.GetString("Username", "Modern Snowfare");
		chatMessages = new List<string>();
	}

	void OnDestroy(){
		PlayerPrefs.SetString ("Username", PhotonNetwork.player.name);
	}

	public void AddChatMessage(string m){
		GetComponent<PhotonView> ().RPC ("AddChatMessage_RPC", PhotonTargets.AllBuffered, m);
	}

	[PunRPC]
	void AddChatMessage_RPC(string m) {
		while (chatMessages.Count >= maxChatMessages) {
			chatMessages.RemoveAt (0);
		}
		chatMessages.Add (m);
	}

	void Connect() {
		PhotonNetwork.ConnectUsingSettings ("MultiFPS v001");
	}

	void OnGUI() {
		GUILayout.Label( PhotonNetwork.connectionStateDetailed.ToString() );

		//connecting variable because PhotonNetwork has delay which keeps button longer than
		// they should be
		if (!PhotonNetwork.connected && !connecting) {
			// TODO Look into NGUI


			GUILayout.BeginArea (new Rect (0, 0, Screen.width, Screen.height));
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			GUILayout.BeginVertical ();
			GUILayout.FlexibleSpace ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Username: ");
			PhotonNetwork.player.name = GUILayout.TextField (PhotonNetwork.player.name);
			GUILayout.EndHorizontal ();


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

		connecting = false;
		SpawnMyPlayer();
	}

	void SpawnMyPlayer() {

		AddChatMessage ("Spawning player: " + PhotonNetwork.player.name);

		if (spawnSpots == null) {
			Debug.Log ("SpawnMyPlayer: SpawnSpots == null");
			return;
		}
		SpawnSpot mySpawnSpot = spawnSpots [Random.Range (0, spawnSpots.Length)];
		GameObject myPlayerGO = (GameObject)PhotonNetwork.Instantiate (prefabName, mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);
		standbyCamera.SetActive(false);

		myPlayerGO.GetComponent<characterController> ().enabled = true;
		((MonoBehaviour)myPlayerGO.GetComponent ("PlayerShooting")).enabled = true;
		myPlayerGO.transform.FindChild("Main Camera").gameObject.SetActive(true);
	}

}
