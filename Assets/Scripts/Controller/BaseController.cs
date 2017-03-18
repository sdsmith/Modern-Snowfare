using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour {
	protected float health = 2.0f;

	// @NOTE(Llewellin): Overridden in JuggernautController
	public virtual float GetHealth() {
		return health;
	}
}
