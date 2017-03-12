using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchSpawnCollision : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter (Collider col)
	{
		if (col.gameObject.GetComponent<GrabAndDrop>() != null){
			if (gameObject.name == "BlueTorchSpawn" && col.gameObject.GetComponent<GrabAndDrop> ().GetGrabbedObjectName() == "Torch_Red") {
				col.gameObject.GetComponent<GrabAndDrop> ().CaptureFlag ();
			} else if (gameObject.name == "RedTorchSpawn" && col.gameObject.GetComponent<GrabAndDrop> ().GetGrabbedObjectName() == "Torch_Blue") {
				col.gameObject.GetComponent<GrabAndDrop> ().CaptureFlag ();
			}
		} 
	}
}
