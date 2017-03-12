using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour {

	static GameObject[] redSpawns;
	static GameObject[] blueSpawns;

	public static Vector3 defaultRedFlag;
	public static Vector3 defaultBlueFlag;

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
}
