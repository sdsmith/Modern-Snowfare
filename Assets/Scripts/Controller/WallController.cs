using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : BaseController {
	float maxHealth = 100f;

	// team set in inspector
	public PunTeams.Team team;

	public override float GetHealth (){
		return maxHealth;
	}
}
