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
        float buttonWidth = 600;
        float buttonHeight = 40;

        if (GUI.Button(new Rect((Screen.width - buttonWidth) * 0.5f, (Screen.height - buttonHeight) * 0.5f, buttonWidth, buttonHeight), "Youtube Tutorial", Styles.Button))
        {
            Application.OpenURL("https://www.youtube.com/watch?v=rxz4S-w7KgY");
        }
    }
}
