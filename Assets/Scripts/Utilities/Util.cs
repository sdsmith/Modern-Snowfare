using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour {

	static GameObject[] redSpawns;
	static GameObject[] blueSpawns;

	public static GameObject redTorchSpawn;
	public static GameObject blueTorchSpawn;

	public static bool redTorchLit;
	public static bool blueTorchLit;

	void Start() {
		redSpawns = GameObject.FindGameObjectsWithTag ("RedSpawn");
		blueSpawns = GameObject.FindGameObjectsWithTag ("BlueSpawn");

		redTorchSpawn = GameObject.Find ("RedTorchSpawn");
		blueTorchSpawn = GameObject.Find ("BlueTorchSpawn");

		redTorchLit = true;
		blueTorchLit = true;
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
