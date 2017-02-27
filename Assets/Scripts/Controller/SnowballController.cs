using UnityEngine;
using System.Collections;



[RequireComponent(typeof(Rigidbody))]
public class SnowballController : MonoBehaviour {

	public float speed = 150f;// m/s(?)



	void Start () {
        Rigidbody rb = GetComponent<Rigidbody>();

        // Apply initial forward velocity to the snowball at creation and let 
        // the physics engine do its work.
        rb.AddRelativeForce(Vector3.forward * speed, ForceMode.VelocityChange);
    }
	
	void Update () {
	}
	

	void OnCollisionEnter (Collision collision)
	{
		if (collision.gameObject.name == "Mountain") {
			Destroy (this.gameObject);
		}
	}
}

