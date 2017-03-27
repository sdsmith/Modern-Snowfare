using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnScreenBlood : MonoBehaviour {
	public Texture blood;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	void onGUI(){
		GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),blood);	
	}
}
