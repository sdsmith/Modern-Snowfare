using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomMenuCreateRoom : RoomMenuBase 
{

    MovieTexture movie;

	void Start()
	{
	
	}

	public void Draw()
	{
		DrawBackground();
        DrawVideo();
	}

    void DrawVideo()
    {
        GUILayout.BeginArea(new Rect(Screen.width / 2.0f, Screen.height / 2.0f, Screen.width, Screen.height));
        {
            GUILayout.Label("Insert some youtube link", Styles.Header);
        }
        GUILayout.EndArea();
        }
}
