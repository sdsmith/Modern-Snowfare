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



	void Start() {
		redSpawns = GameObject.FindGameObjectsWithTag ("RedSpawn");
		blueSpawns = GameObject.FindGameObjectsWithTag ("BlueSpawn");

		defaultRedFlag = GameObject.Find ("Torch_Red").transform.position;
		defaultBlueFlag = GameObject.Find ("Torch_Blue").transform.position;
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
}
