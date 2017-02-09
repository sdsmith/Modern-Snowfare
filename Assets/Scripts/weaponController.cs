using UnityEngine;
using System.Collections;

public class weaponController : MonoBehaviour {

	public bool isFiring;

	public snowballController snowball;
	public float snowballSpeed;

	public float timeBetweenShots;
	public float shotCounter;

	public Transform firePoint;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (isFiring) {
			shotCounter -= Time.deltaTime;
			if (shotCounter <= 0) {
				shotCounter = timeBetweenShots;
				snowballController newSnowball = Instantiate (snowball, firePoint.position, firePoint.rotation) as snowballController;
				newSnowball.speed = snowballSpeed;
			}
		} else {
			shotCounter = 0;
		}
	}
}
