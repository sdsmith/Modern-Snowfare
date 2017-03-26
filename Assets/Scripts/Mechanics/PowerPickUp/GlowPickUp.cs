using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowPickUp : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);

        if (collision.gameObject.name != "Terrain" && collision.gameObject.name != "Snowball(Clone)" &&
             collision.gameObject.name != "Torch_Blue" && collision.gameObject.name != "Torch_Red")
        {
            int viewID = collision.gameObject.GetPhotonView().viewID;
            GetComponent<PhotonView>().RPC("RemoveGlow", PhotonTargets.AllBuffered);
        }


    }
    [PunRPC]
    void RemoveGlow()
    {
        Destroy(this.gameObject);
    }
}
