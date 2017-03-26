using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPickUp : MonoBehaviour {

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
			GetComponent<PhotonView> ().RPC("SpeedUnit", PhotonTargets.AllBuffered, viewID);
		}
	}	

	[PunRPC] 
	void SpeedUnit(int viewID){
		PlayerController Player = PhotonView.Find (viewID).gameObject.GetComponent<PlayerController>();
		float Speed = Player.GetSpeed();
		Player.SetSpeed ((Speed * 1.5f));
		this.GetComponent<MeshRenderer>().enabled = false;
		this.GetComponent<SphereCollider>().enabled = false;
        Vector3 SpawnPosition = new Vector3 (0,0,0);
		GameObject temp = PhotonNetwork.Instantiate ("SpeedPowerUpIndicator", SpawnPosition,gameObject.transform.rotation,0);
		temp.transform.parent = Player.transform;
		temp.transform.localPosition = Vector3.zero;
		StartCoroutine(Timer(Player, Speed));

	}

	[PunRPC] 
	IEnumerator Timer(PlayerController Player, float Speed) {
		yield return new WaitForSeconds(15);
		Player.SetSpeed (10);
		if (Player != null && Player.transform.Find("SpeedPowerUpIndicator(Clone)").gameObject!= null) {
			Destroy (Player.transform.Find ("SpeedPowerUpIndicator(Clone)").gameObject);
		}
		Destroy (this.gameObject);
	}

}
	