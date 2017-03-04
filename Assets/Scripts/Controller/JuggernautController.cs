using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuggernautController : PlayerController {
	/*
	 * @NOTE(Llewellin): Character controllers inherit from PlayerController.
	 * If you're overriding a parent method, check to see if you should call
	 * the parent function first with base.<function>(); Ensure the function
	 * is public/protected, Unity defaults to private.
	 */

	// Use this for initialization
	void Start () {
		base.Start ();

		hitPoints *= 2; 
	}
}
