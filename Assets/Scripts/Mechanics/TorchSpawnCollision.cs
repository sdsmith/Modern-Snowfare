using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchSpawnCollision : MonoBehaviour {

    private TextManager textManager;


	void Start () {
        textManager = GameObject.FindObjectOfType<TextManager>();
        if (textManager == null) {
            Debug.LogError("Text Manager is null");
        }
    }

	void OnTriggerEnter (Collider col)
	{
		if (col.gameObject.GetComponent<GrabAndDrop>() != null){
			if (gameObject.name == "BlueTorchSpawn" && col.gameObject.GetComponent<GrabAndDrop> ().GetGrabbedObjectName() == "Torch_Red") {
				col.gameObject.GetComponent<GrabAndDrop> ().CaptureFlag ();
                textManager.AddFlagCaptureMessage(PunTeams.Team.blue);

			} else if (gameObject.name == "RedTorchSpawn" && col.gameObject.GetComponent<GrabAndDrop> ().GetGrabbedObjectName() == "Torch_Blue") {
				col.gameObject.GetComponent<GrabAndDrop> ().CaptureFlag ();
                textManager.AddFlagCaptureMessage(PunTeams.Team.red);
            }
		} 
	}
}
