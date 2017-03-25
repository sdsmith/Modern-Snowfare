using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Creates the synchronized ship objects
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
	// This is the name of the prefab we are going to create into the game.
	// This should be changed to match whichever character the player chooses to be
	string prefabName;
	public bool debug = false;
	public GameObject standbyCamera;

	public float respawnTimer = 0f;
	public GameObject Indicator;

	TextManager tm;

	void Start()
	{
		//if we are not connected, than we probably pressed play in a level in editor mode.
		//In this case go back to the main menu to connect to the server first
		if( PhotonNetwork.connected == false )
		{
			SceneManager.LoadScene ( "MainMenu" );
			return;
		}

		// Set the player prefab
		// @DEBUG(sdsmith):
		if (debug) {
			prefabName = "PlayerDebug";
		} else {
			prefabName = "Flash";
		}

		PhotonNetwork.player.NickName = PlayerPrefs.GetString("Username", "Modern Snowfare");
		PhotonNetwork.player.SetTeam(PunTeams.Team.none);

		tm = GetComponent<TextManager> ();
		if (tm == null) {
			Debug.LogError ("Text Manager is null");
		}
	}

	// NOTE: Update is called once per frame
	void Update() {
		if (respawnTimer > 0) {
			respawnTimer -= Time.deltaTime;

			// When enough time has passed, respawn the player
			if (respawnTimer <= 0) {
				SpawnMyPlayer();

			}
		}
	}

	public void CreateLocalPlayer( Team team )
	{
		object[] instantiationData = new object[] { (int)team } ;

		//Notice the differences from PhotonNetwork.Instantiate to Unitys GameObject.Instantiate
		GameObject newShipObject = PhotonNetwork.Instantiate( 
			"Ship", 
			Vector3.zero, 
			Quaternion.identity, 
			0,
			instantiationData
		);

		Transform spawnPoint = GamemodeManager.CurrentGamemode.GetSpawnPoint( team );
		newShipObject.transform.position = spawnPoint.transform.position;
		newShipObject.transform.rotation = spawnPoint.transform.rotation;

		Ship newShip = newShipObject.GetComponent<Ship>();
		newShip.SetTeam( team );

		//Since this function is called on every machine to create it's one and only local player, the new ship is always the camera target
		Camera.main.GetComponent<CameraShipFollow>().SetTarget( newShip );
	}

	// When the user selectes a new character, set the prefab
	// to the selected character and spawn
	public void SpawnMyPlayer(string selectedCharacter)
	{
		prefabName = selectedCharacter;
		SpawnMyPlayer ();
	}

	/*
	* @NOTE(sdsmith): This includes both your local client's player and
	* network client players.
	*/
	public void SpawnMyPlayer() 
	{
		tm.AddSpawnMessage(PhotonNetwork.player.NickName);

		// Set the spawn point based on the team you're on
		GameObject spawnPoint = Util.GetSpawnPoint(PhotonNetwork.player.GetTeam());

		if (spawnPoint == null) {
			Debug.LogError("Spawn point is null");
		}

		GameObject myPlayerGO = (GameObject)PhotonNetwork.Instantiate(prefabName, spawnPoint.transform.position, spawnPoint.transform.rotation, 0);
		standbyCamera.SetActive(false);

		// Set referance to the local player
		// @TODO(sdsmith): Confirm that this always spawns out local player
		if (myPlayerGO.GetComponent<PhotonView>().owner.IsLocal) {
			Util.localPlayer = myPlayerGO;
		}

		// Enable player scripts
		ToggleComponents(myPlayerGO);

		int viewID = myPlayerGO.gameObject.GetPhotonView().viewID;
		GetComponent<PhotonView>().RPC("SetTeamIcon", PhotonTargets.AllBuffered, viewID);
	}

	// Enable the player controller scripts that were disabled initially
	void ToggleComponents(GameObject myPlayerGO) {

		if (myPlayerGO == null) {
			Debug.LogError("Player object is null");
			return;
		}

		myPlayerGO.GetComponent<PlayerController>().enabled = true;

		//PlayerCapsule doesn't have a BotControlScript, so check if its not null
		if (myPlayerGO.GetComponent<BotControlScript>() != null) {
			myPlayerGO.GetComponent<BotControlScript>().enabled = true;
		}

		myPlayerGO.GetComponent<GrabAndDrop>().enabled = true;
		myPlayerGO.GetComponent<PlayerShooting>().enabled = true;
		myPlayerGO.transform.FindChild("Main Camera").gameObject.SetActive(true);
		myPlayerGO.transform.FindChild("RadarCamera").gameObject.SetActive(true);

		//activates indicators
		GameObject temp = Instantiate (Indicator);
		temp.transform.parent = myPlayerGO.transform;
		myPlayerGO.transform.FindChild("IndicatorLogic(Clone)").gameObject.SetActive(true);

		// Allow a player to walk into their teams base
		CapsuleCollider playerCC = myPlayerGO.GetComponent<CapsuleCollider> ();
		if (PhotonNetwork.player.GetTeam () == PunTeams.Team.red) {
			BoxCollider fortBC = Util.redFortEntrance.GetComponent<BoxCollider> ();
			Physics.IgnoreCollision (playerCC, fortBC);
		} else {
			BoxCollider fortBC = Util.blueFortEntrance.GetComponent<BoxCollider> ();
			Physics.IgnoreCollision (playerCC, fortBC);
		}
	}

	// Set the color of the icon on top of the player to match their team
	// Can't pass the gameObject so pass the reference ID
	[PunRPC]
	void SetTeamIcon(int viewID) {
		GameObject playerObject = PhotonView.Find(viewID).gameObject;

		if (playerObject == null) {
			Debug.Log("playerObject is null");
			return;
		}
		//set an indicator object to the prefab
		//GameObject temp = Instantiate (Indicator);

		GameObject teamTag = playerObject.transform.FindChild("TeamIndicator").gameObject;
		GameObject r = playerObject.transform.FindChild("RadarPlayerPosition").gameObject;
		//adding indicatorteam as child
		//temp.transform.parent = playerObject.transform;
		//playerObject.transform.FindChild("IndicatorLogic(Clone)").gameObject.SetActive(true);


		if (playerObject.GetPhotonView().owner.GetTeam() == PunTeams.Team.red) {
			teamTag.GetComponent<SpriteRenderer>().material.color = Color.red;
			r.GetComponent<MeshRenderer>().material.color = Color.red;

		} else {
			teamTag.GetComponent<SpriteRenderer>().material.color = Color.blue;
			r.GetComponent<MeshRenderer>().material.color = Color.blue;

		}
	}
}