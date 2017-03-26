using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WallController))]
public class WallHealth : Health {

	Vector3 shiftOffset;

	/* @NOTE(Llewellin);
	 * Ratio will be the amount to drop the wall whenever it gets hit. This is calculated by its height over health.
	 * The height (scale) of the wall, at time of writing, is 20 (half over, half under the map). So we're gonna use 10.
	 * I'm gonna hard code the number 10 because what is time?
	 */
	float ratio;

	void Start() {
		base.Start ();
		ratio = 10 / GetComponent<WallController> ().GetHealth ();
	}

	[PunRPC] // can be called indirectly
	// all players recieve notification of something taking damage
	public override void TakeDamage (float amt, int attackerViewID) {
		// Shift the wall down
		shiftOffset = new Vector3(0, amt * ratio, 0);
		//ShiftDown();
		gameObject.transform.position -= shiftOffset;
		base.TakeDamage (amt, attackerViewID);
	}

	void ShiftDown() {
		gameObject.transform.position -= shiftOffset;
	}
}
