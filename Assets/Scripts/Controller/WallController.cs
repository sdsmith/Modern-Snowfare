using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : BaseController {
	float maxHealth = 100f;

	public override float GetHealth (){
		return maxHealth;
	}
}
