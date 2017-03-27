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

	/* @NOTE(Llewellin):
	 * This is the max amount a healer can repair the fort. When the forts were built, they have an initial height of 3.14,
	 */
	float maxRepair = 3.14f;

	new void Start() {
		base.Start ();
		ratio = 10 / GetComponent<WallController> ().GetHealth ();
	}

	[PunRPC] // can be called indirectly
	// all players recieve notification of something taking damage
	public override void TakeDamage (float amt, int attackerViewID) {

		// Shift the wall up/down by offset
		shiftOffset = new Vector3(0, amt * ratio, 0);

		if(ShouldRepairFort(attackerViewID)) {
			if(CanRepairFort()) {
				ShiftUp ();
			}
		}
		else {
			ShiftDown ();
		}
	}

	void ShiftDown() {
		gameObject.transform.position -= shiftOffset;
	}

	void ShiftUp() {
		gameObject.transform.position += shiftOffset;
	}

	// Check to see if its a healer and they're on the same team
	bool ShouldRepairFort(int attackerViewID) {
		PhotonView pv = PhotonView.Find(attackerViewID);

		if (pv) {
			PlayerController pc = pv.gameObject.GetComponent<PlayerController> ();
			if (pc) {
				return pc is HealerController && Util.IsFortSameTeam (this.gameObject, pc.gameObject);
			}
		}
		return false;
	}

	bool CanRepairFort() {
		return gameObject.transform.position.y < maxRepair;
	}
}
