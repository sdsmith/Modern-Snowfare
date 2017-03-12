using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour {

	static GameObject[] redSpawns;
	static GameObject[] blueSpawns;

	static GameObject redTorchSpawn;
	static GameObject blueTorchSpawn;

	public static Vector3 defaultRedFlag;
	public static Vector3 defaultBlueFlag;

	void Start() {
		redSpawns = GameObject.FindGameObjectsWithTag ("RedSpawn");
		blueSpawns = GameObject.FindGameObjectsWithTag ("BlueSpawn");

		redTorchSpawn = GameObject.Find ("Torch_Red");
		blueTorchSpawn = GameObject.Find ("Torch_Blue");

		defaultRedFlag = redTorchSpawn.transform.position;
		defaultBlueFlag = blueTorchSpawn.transform.position;
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
