using UnityEngine;
using System.Collections;

public class snowballController : MonoBehaviour {

	public float speed = 10f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (Vector3.forward * speed * Time.deltaTime);
	}
	

	void OnCollisionEnter (Collision collision)
	{
		if (collision.gameObject.name == "Mountain") {
			Destroy (this.gameObject);
		}
	}
}

