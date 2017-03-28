using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointController : MonoBehaviour {
    
	void Start () {
        // Rotate spawn point toward the center of the map
        {
            // Get the terrain center point
            Terrain terrain = GameObject.FindGameObjectWithTag("Terrain").GetComponent<Terrain>();
            Vector3 terrainSize = terrain.terrainData.size;
            Vector3 centerPoint = terrain.transform.position + terrainSize / 2f;

            // Create rotation
            Vector3 toCenter = centerPoint - transform.position;
            toCenter.y = 0; // y doesn't matter
            Quaternion rotToCenter = Quaternion.FromToRotation(transform.forward, toCenter);

            // Rotate
            transform.rotation = rotToCenter * transform.rotation;
        }
    }
}
