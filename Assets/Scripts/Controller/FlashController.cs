using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashController : PlayerController {

	public override float GetSpeed () {
		return base.GetSpeed() * 2;
	}
}
