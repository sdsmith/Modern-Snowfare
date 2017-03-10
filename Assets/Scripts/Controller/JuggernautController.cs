using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuggernautController : PlayerController {

	public override float GetHealth (){
		return base.GetHealth () * 2;
	}
}
