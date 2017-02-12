using UnityEngine;
using System.Collections;

public class SelfDestruct : MonoBehaviour {

	public float selfDestructTime = 10.0f;

	void Update () {
		selfDestructTime -= Time.deltaTime;

		if(selfDestructTime <= 0) {
			Destroy ();
		}
	}

	void Destroy()
	{
		PhotonView pv = GetComponent<PhotonView>();

		if(pv != null && pv.instantiationId!=0 ) {
			PhotonNetwork.Destroy(gameObject);
		}
		else {
			Destroy(gameObject);
		}
	}
}
