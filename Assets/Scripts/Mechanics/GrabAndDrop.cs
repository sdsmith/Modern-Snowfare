using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabAndDrop : MonoBehaviour {

	GameObject toLight;
	GameObject grabbedObject;
	public PunTeams.Team ourTeam;
	public Vector3 offset = new Vector3(0,0,0);
	public Vector3 dropOffset = new Vector3(7,0,0);

    private Vector3 originalGrabbedObjectScale;

	// Use this for initialization
	void Start () {
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

	void Update()
	{
		if (Input.GetMouseButtonDown (1)) {
			Debug.Log ("dropping object");

			DropObject ();

		}
	}

	void TryGrabObject(GameObject grabObject)
	{
		if(grabObject == null)
		{
			return;
		}
		
		int objViewID = grabObject.GetComponent<PhotonView>().viewID;
		GetComponent<PhotonView>().RPC("GrabbingObject", PhotonTargets.AllBuffered, objViewID);
	}

	[PunRPC]
	public void GrabbingObject(int viewID) {

		grabbedObject = PhotonView.Find(viewID).gameObject;

		grabbedObject.GetComponent<CapsuleCollider> ().enabled = false;

		grabbedObject.transform.SetParent (gameObject.transform, false);
		Vector3 offset = Quaternion.AngleAxis(-45, gameObject.transform.right) * gameObject.transform.forward * 4;
		grabbedObject.transform.position = gameObject.transform.position + offset;

        // Adjust the scale of the object to the scale of the new parent so it retains its size.
        {
            Vector3 parentScale = gameObject.transform.localScale;
            originalGrabbedObjectScale = grabbedObject.transform.localScale;

            Vector3 adjustedScale = originalGrabbedObjectScale;
            adjustedScale.x /= parentScale.x;
            adjustedScale.y /= parentScale.y;
            adjustedScale.z /= parentScale.z;

            grabbedObject.transform.localScale = adjustedScale;
        }
	}

	public void DropObject()
	{
		GetComponent<PhotonView>().RPC("DroppingObject", PhotonTargets.AllBuffered);
	}

	[PunRPC]
	public void DroppingObject(){
		if (grabbedObject == null)
		{
			return;
		}

		grabbedObject.transform.parent = null;
		grabbedObject.GetComponent<CapsuleCollider> ().enabled = true;
		//grabbedObject.transform.position = gameObject.transform.position + dropOffset;
        // Adjust the scale of the grabbed object back to normal
        {
            grabbedObject.transform.localScale = originalGrabbedObjectScale;
        }
        grabbedObject = null;
	}

	[PunRPC]
	public void ResetFlag(PunTeams.Team team) {

		if (team == PunTeams.Team.red) {
			GameObject.Find ("Torch_Red").transform.position = Util.defaultRedFlag;
		} else {
			GameObject.Find ("Torch_Blue").transform.position = Util.defaultBlueFlag;
		}
	}

	void OnCollisionEnter (Collision col)
	{
		//if blue team colliding with red torch
		if (col.gameObject.name == "Torch_Red" && ourTeam == PunTeams.Team.blue) {
			//if red torch is lit
			if (Util.redTorchLit == true) {
				//grab it
				if (grabbedObject == null)
				{
				TryGrabObject (col.gameObject);
				}
			} 
			//red torch not lit
			else {
				//if holding lighter
				if (grabbedObject.name == "Lighter") {
					toLight = col.gameObject;
					//light the torch
					GetComponent<PhotonView> ().RPC ("LightTorch", PhotonTargets.AllBuffered, "red");
					//LightTorch ("red");
					//col.gameObject.GetComponent<Torchelight> ().IntensityLight = 1;
					//col.gameObject.GetComponent<Torchelight> ().MaxLightIntensity = 3;
				}
			}
		} else if (col.gameObject.name == "Torch_Blue" && ourTeam == PunTeams.Team.red) {
			if (Util.blueTorchLit == true) {
				if (grabbedObject == null)
				{
				TryGrabObject (col.gameObject);
				}
			} else {
				
				if (grabbedObject.name == "Lighter") {
					toLight = col.gameObject;
					GetComponent<PhotonView> ().RPC ("LightTorch", PhotonTargets.AllBuffered, "blue");
					//LightTorch ("blue");
					//col.gameObject.GetComponent<Torchelight> ().IntensityLight = 1;
					//col.gameObject.GetComponent<Torchelight> ().MaxLightIntensity = 3;
				}
			}
		} else if ((col.gameObject.name == "Torch_Red" && ourTeam == PunTeams.Team.red) ||
		         (col.gameObject.name == "Torch_Blue" && ourTeam == PunTeams.Team.blue)) {
			// col.gameObject.transform.position = RedTorchSpawn.transform.position;
			GetComponent<PhotonView> ().RPC ("ResetFlag", PhotonTargets.AllBuffered, ourTeam);
			// Debug.Log ("Red reclaiming red torch");

		}
		// If we collide with our own flag (red team)
		else if (col.gameObject.name == "Torch_Red" && ourTeam == PunTeams.Team.red) 
		{
			// Reset the flag only if its not at the base
			if (col.gameObject.transform.position != Util.defaultRedFlag) {
				GetComponent<PhotonView> ().RPC ("ResetFlag", PhotonTargets.AllBuffered, ourTeam);
			}
		} 
		// If we collide with our own flag (blue team)
		else if(col.gameObject.name == "Torch_Blue" && ourTeam == PunTeams.Team.blue) 
		{
			// Reset the flag only if its not at the base
			if (col.gameObject.transform.position != Util.defaultBlueFlag) {
				GetComponent<PhotonView> ().RPC ("ResetFlag", PhotonTargets.AllBuffered, ourTeam);
			}
		} 

		else if (col.gameObject.name == "Lighter") 
		{
			if (grabbedObject == null)
			{
			TryGrabObject (col.gameObject);
			}
		}
	}

	public string GetGrabbedObjectName()
	{
		if (grabbedObject == null) {
			return "";
		}
		return grabbedObject.name;
	}

	public GameObject GetGrabbedObject()
	{
		return grabbedObject;
	}
	public void CaptureFlag()
	{
		if (grabbedObject != null) {
			DropObject ();

			IncreaseTeamScore (ourTeam);
			IncreasePlayerScore ();
			//If we're red team, reset the blue flag
			if (ourTeam == PunTeams.Team.red) {
				GetComponent<PhotonView> ().RPC ("ResetFlag", PhotonTargets.AllBuffered, PunTeams.Team.blue);
			} 
			else {
				GetComponent<PhotonView> ().RPC ("ResetFlag", PhotonTargets.AllBuffered, PunTeams.Team.red);
			}

		}
	}
		
	public void flameOff()
	{
		GetComponent<PhotonView> ().RPC ("RPCFlameOff", PhotonTargets.AllBuffered);
	}

	[PunRPC]
	public void RPCFlameOff()
	{
		if (GetGrabbedObjectName () == "Torch_Red") {
			Util.redTorchLit = false; 
			grabbedObject.GetComponent<Torchelight> ().IntensityLight = 0;
			grabbedObject.GetComponent<Torchelight> ().MaxLightIntensity = 0;

		} 
		else if (GetGrabbedObjectName() == "Torch_Blue"){
			Util.blueTorchLit = false;
			grabbedObject.GetComponent<Torchelight> ().IntensityLight = 0;
			grabbedObject.GetComponent<Torchelight> ().MaxLightIntensity = 0;
		}
	}

	[PunRPC]
	public void SetToLight()
	{
		
	}
	[PunRPC]
	public void RPCFlameOn()
	{
		Debug.Log ("setting torch light intensity");
		toLight.GetComponent<Torchelight> ().IntensityLight = 1;
		toLight.GetComponent<Torchelight> ().MaxLightIntensity = 3;
		Debug.Log ("Torch light intensity set");
	}

	[PunRPC]
	public void LightTorch(string colour)
	{
		//GetComponent<PhotonView> ().RPC ("RPCFlameOn", PhotonTargets.AllBuffered);
		if (colour == "red") 
		{
			Util.redTorchLit = true;
			GameObject.Find ("Torch_Red").GetComponent<Torchelight> ().IntensityLight = 1;
			GameObject.Find ("Torch_Red").GetComponent<Torchelight> ().MaxLightIntensity = 1;
		}

		else if (colour == "blue") 
		{
			Util.blueTorchLit = true;
			GameObject.Find ("Torch_Blue").GetComponent<Torchelight> ().IntensityLight = 1;
			GameObject.Find ("Torch_Blue").GetComponent<Torchelight> ().MaxLightIntensity = 1;

		}

		toLight = null;

	}


	public void IncreaseTeamScore( PunTeams.Team team )
	{
		//We need to know which property we have to change, blue or red
		string property = RoomProperty.BlueScore;

		if( team == PunTeams.Team.red )
		{
			property = RoomProperty.RedScore;
		}

		ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable();
		//In case the property doesn't yet exist, create it with a score of 1
		newProperties.Add( property, 1 );

		if( PhotonNetwork.room.CustomProperties.ContainsKey( property ) == true )
		{
			//if the property does exist, we just add one to the old value
			newProperties[ property ] = (int)PhotonNetwork.room.CustomProperties[ property ] + 1;
		}

		PhotonNetwork.room.SetCustomProperties( newProperties );
	}

	void IncreasePlayerScore () {
		GetComponent<PlayerController> ().flagCaptureCount++;
	}
}
