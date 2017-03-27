using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchSpawnCollision : MonoBehaviour {

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
