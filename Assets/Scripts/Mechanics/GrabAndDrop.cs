using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabAndDrop : MonoBehaviour {

	GameObject grabbedObject;
	float grabbedObjectSize;
	public GameObject BlueTorchSpawn;
	public GameObject RedTorchSpawn;
	public GameObject PlayerTracker;
	public PunTeams.Team ourTeam;
	// Use this for initialization
	void Start () {
		BlueTorchSpawn = GameObject.Find ("BlueTorchSpawn");
		RedTorchSpawn = GameObject.Find ("RedTorchSpawn");
		ourTeam = PhotonNetwork.player.GetTeam();
	}
	/*
	GameObject GetMouseHoverObject(float range)
	{
		Vector3 position = this.gameObject.transform.position;
		RaycastHit raycastHit;
		Vector3 target = position + Camera.main.transform.forward * range; 
		if (Physics.Linecast(position, target, out raycastHit))
		{
			return raycastHit.collider.gameObject;
			
		}
		return null; 
	}*/

	void TryGrabObject(GameObject grabObject)
		{
			if(grabObject == null)
			{
				return;
			}

			grabbedObject = grabObject;
			grabbedObjectSize = grabObject.GetComponent<Renderer>().bounds.size.magnitude;

		}

	void DropObject()
		{
			if (grabbedObject == null)
			{
				return;
			}
			grabbedObject = null;
		}

	void OnCollisionEnter (Collision col)
	{
		if ((col.gameObject.name == "Torch_Red" && ourTeam == PunTeams.Team.blue) || 
			(col.gameObject.name == "Torch_Blue" && ourTeam == PunTeams.Team.red))  {
			TryGrabObject (col.gameObject);
		} 

		else if (col.gameObject.name == "RedTorchSpawn" && ourTeam == PunTeams.Team.red) 
			{
			grabbedObject.transform.position = BlueTorchSpawn.transform.position;
			DropObject ();

		}
		else if (col.gameObject.name == "BlueTorchSpawn" && ourTeam == PunTeams.Team.blue)
		{
			grabbedObject.transform.position = RedTorchSpawn.transform.position;
			DropObject ();
			}

	}

	// Update is called once per frame
	void Update () {
		//Debug.Log (GetMouseHoverObject (5)); 

		/*
		if (Input.GetMouseButtonDown(1))
			{
				if(grabbedObject == null)
				{
					TryGrabObject(GetMouseHoverObject(10));
				}
				
				else
				{
					DropObject(); 
				}	
			}

		*/

		if (grabbedObject != null)
			{
			 Vector3 newPosition = gameObject.transform.position+Camera.main.transform.forward*grabbedObjectSize;
			 grabbedObject.transform.position = newPosition;
			}
	}
}
