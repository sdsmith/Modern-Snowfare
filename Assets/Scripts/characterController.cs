using UnityEngine;
using System.Collections;

public class characterController : MonoBehaviour {

	public float speed= 10.0F;
	public weaponController theWeapon;

	// Use this for initialization
	void Start () {
		Cursor.lockState = CursorLockMode.Locked;
	}	
	
	// Update is called once per frame
	void Update () {
		float translation = Input.GetAxis ("Vertical") * speed;
		float straffe = Input.GetAxis ("Horizontal") * speed;
		straffe *= Time.deltaTime;
		translation *= Time.deltaTime;

		transform.Translate (straffe, 0, translation);

		if (Input.GetKeyDown ("escape"))
			Cursor.lockState = CursorLockMode.None;
		//0 left click
		if (Input.GetMouseButtonDown (0)) {
			theWeapon.isFiring = true;
		}
		if (Input.GetMouseButtonUp (0)) {
			theWeapon.isFiring = false;
		}
	}
}
