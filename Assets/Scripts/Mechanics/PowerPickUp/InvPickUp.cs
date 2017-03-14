using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvPickUp : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnCollisionEnter(Collision collision) {
		Debug.Log (collision.gameObject.name);

		if (collision.gameObject.name != "Terrain" && collision.gameObject.name != "Snowball(Clone)" &&
			 collision.gameObject.name != "Torch_Blue" && collision.gameObject.name != "Torch_Red") {
			int viewID = collision.gameObject.GetPhotonView ().viewID;
			GetComponent<PhotonView> ().RPC("InvUnit", PhotonTargets.AllBuffered, viewID);

		}
	}

	[PunRPC] 
	void InvUnit(int viewID){
		PlayerController Player = PhotonView.Find (viewID).gameObject.GetComponent<PlayerController>();
		SkinnedMeshRenderer Renderer = PhotonView.Find (viewID).gameObject.transform.FindChild("MicroMale").GetComponent<SkinnedMeshRenderer>();
		Renderer.enabled = false;
		this.GetComponent<MeshRenderer>().enabled = false;
		this.GetComponent<SphereCollider>().enabled = false;
		Vector3 SpawnPosition = new Vector3 (0,0,0);
		GameObject temp = PhotonNetwork.Instantiate ("InvPowerUpIndicator", SpawnPosition,gameObject.transform.rotation,0);
		temp.transform.parent = Player.transform;
		temp.transform.localPosition = Vector3.zero;
		StartCoroutine(Timer(Player, Renderer));

	}

	[PunRPC] 
	IEnumerator Timer(PlayerController Player, SkinnedMeshRenderer Renderer) {
		yield return new WaitForSeconds(15);
		Player.GetComponent<Health> ().SetCurrentPoints (Player.GetHealth());
		if (Player != null) {
			Renderer.enabled = true;
			Destroy(Player.transform.Find("InvPowerUpIndicator(Clone)").gameObject);
		}
		Destroy (this.gameObject);
	}
}
