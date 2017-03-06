using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour {

	static GameObject[] redSpawns;
	static GameObject[] blueSpawns;

    /** Contains reference to the local player, or null if it doesn't exist. */
    public static GameObject localPlayer = null;


	void Start() {
		redSpawns = GameObject.FindGameObjectsWithTag ("RedSpawn");
		blueSpawns = GameObject.FindGameObjectsWithTag ("BlueSpawn");
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
}
