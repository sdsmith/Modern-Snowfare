using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour {

	public bool isFiring;

	public SnowballController snowball;
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
				SnowballController newSnowball = Instantiate (snowball, firePoint.position, firePoint.rotation) as SnowballController;
				newSnowball.speed = snowballSpeed;
			}
		} else {
			shotCounter = 0;
		}
	}
}
