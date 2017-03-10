using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperController : PlayerController {

	public override float GetDamage () {
		return base.GetDamage() * 2;
	}
}