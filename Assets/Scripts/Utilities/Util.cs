using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Util : MonoBehaviour {

	static GameObject[] redSpawns;
	static GameObject[] blueSpawns;

    public static Vector3 defaultRedFlag;
    public static Vector3 defaultBlueFlag;

    /** Contains reference to the local player, or null if it doesn't exist. */
    public static GameObject localPlayer = null;

	public static GameObject redFortEntrance;
	public static GameObject blueFortEntrance;

	public static bool redTorchLit;
	public static bool blueTorchLit;

	public static GameObject redTorchSpawn;
	public static GameObject blueTorchSpawn;

	void Start() {
		redSpawns = GameObject.FindGameObjectsWithTag ("RedSpawn");
		blueSpawns = GameObject.FindGameObjectsWithTag ("BlueSpawn");


		redTorchSpawn = GameObject.Find ("RedTorchSpawn");
		blueTorchSpawn = GameObject.Find ("BlueTorchSpawn");

		redTorchLit = true;
		blueTorchLit = true;

		defaultRedFlag = GameObject.Find ("Torch_Red").transform.position;
		defaultBlueFlag = GameObject.Find ("Torch_Blue").transform.position;

		redFortEntrance = GameObject.Find("RedFortEntrance");
		blueFortEntrance = GameObject.Find("BlueFortEntrance");

	}


	// Get spawn point based on your team
	public static GameObject GetSpawnPoint(PunTeams.Team team) {
		if (team == PunTeams.Team.red) {
			return redSpawns [Random.Range (0, redSpawns.Length)];
		} 
		else {
			return blueSpawns [Random.Range (0, blueSpawns.Length)];
		}
	}


    /**
     * Add UnityEngine.UI.Text component to the given Canvas game object with the given text.
     * Returns a reference to the added Text component.
     * 
     * @deprecated
     */
    public static Text AddTextToCanvas(string text, GameObject canvasGameObject) {
        Text t = canvasGameObject.AddComponent<Text>();
        t.text = text;

        Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        t.font = ArialFont;
        t.material = ArialFont.material;

        return t;
    }


    /**
     * Return the team associated with the given game object.
     */
    public static PunTeams.Team GetTeam(GameObject go) {
        PunTeams.Team team = PunTeams.Team.none;

        // Check the game object first
        PhotonView pv = go.GetComponent<PhotonView>();
        
        // If the PhotonView was not found, try checking the parent
        if (pv == null) {
            pv = go.GetComponentInParent<PhotonView>();
        }

        if (pv != null) {
            PhotonPlayer pplayer = pv.owner;
            if (pplayer != null) {
                team = pplayer.GetTeam();
            }
        }

        return team;
    }


    /**
     * Return true if the two given game objects are on the same team.
     */
    public static bool IsSameTeam(GameObject go1, GameObject go2) {
        return GetTeam(go1) == GetTeam(go2);
    }

	/**
     * Return true if the given game object is on the red team.
     */
	public static bool IsRedTeam(GameObject go) {
		return GetTeam (go) == PunTeams.Team.red;
	}

	public static bool IsFortSameTeam(GameObject fortObject, GameObject playerObject) {
		WallController wc = fortObject.GetComponent<WallController> ();

		if (wc == null) {
			Debug.LogError ("Wall Controller is null");
			return false;
		}

		return wc.team == GetTeam (playerObject);
	}

    /**
     * Return the first child transform of the given game object with the given tag,
     * or null if no matching tags are found.
     */
    public static Transform FindChildByTag(GameObject go, string tag) {
        foreach (Transform child in go.transform) {
            if (child.tag == tag) {
                return child;
            }
        }
        return null;
    }

	/*
	 * @NOTE(Llewellin): GetCustomProperty() and SetCustomProperty() taken from Helper.cs
	 */
	public static T GetCustomProperty<T>( PhotonView view, string property, T offlineValue, T defaultValue )
	{
		//If in offline mode, return the value from the local variable
		if( PhotonNetwork.offlineMode == true )
		{
			return offlineValue;
		}
		//In online mode, use the players custom properties. This enables
		//other players to see this stat as well
		else
		{
			//Check if the KillCount property already exist
			if( view != null && 
				view.owner != null && 
				view.owner.CustomProperties.ContainsKey( property ) == true )
			{
				return (T)view.owner.CustomProperties[ property ];
			}

			//If not, no kills have been registered yet, return 0
			return defaultValue;
		}
	}

	public static void SetCustomProperty<T>( PhotonView view, string property, ref T offlineVariable, T value )
	{
		//If in offline mode, store the value in a local variable
		if( PhotonNetwork.offlineMode == true )
		{
			offlineVariable = value;
		}
		else
		{
			//Photon has it's own Hashtable class in order to ensure that the data
			//can be synchronized between all platforms
			ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
			properties.Add( property, value );

			//Use the SetCustomProperties function to set new values and update existing ones
			//This function saves the data locally and sends synchronize operations so that every
			//client receives the update as well. 
			//Don't set PhotonView.owner.customProperties directly as it wouldn't be synchronized
			view.owner.SetCustomProperties( properties );
		}
	}
}
