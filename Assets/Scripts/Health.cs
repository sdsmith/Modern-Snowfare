using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

	public float hitPoints = 100f;
	float currentPoints;

	// Use this for initialization
	void Start () {
		currentPoints = hitPoints;
	}
	

	public void TakeDamage (float amt) {
		currentPoints -= amt;

		if (currentPoints <= 0) {
			Die ();
		}
	}

	void Die(){
		Destroy (gameObject);
	}
}
