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
     */
    public static Text AddTextToCanvas(string text, GameObject canvasGameObject) {
        Text t = canvasGameObject.AddComponent<Text>();
        t.text = text;

        Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        t.font = ArialFont;
        t.material = ArialFont.material;

        return t;
    }
}
