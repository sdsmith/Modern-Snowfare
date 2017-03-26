using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGUI : MonoBehaviour {

 	enum Status
	{
		pickingTeam, 
		pickingCharacter,
		inGame
	}

	Status currentStatus;

	GUIStyle m_PickButtonStyle;
	public Font ButtonFont;
	public Texture2D ButtonBackground;

	// Use this for initialization
	void Start () {
		currentStatus = Status.pickingTeam;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI() {

		switch (currentStatus) {
		case Status.pickingTeam:
			{
				// Player has not yet selected a team
				LoadStyles();
				GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20));
				{
					GUILayout.BeginHorizontal();
					{
						if (GUILayout.Button("blue", m_PickButtonStyle, GUILayout.Width(Screen.width * 0.5f - 20), GUILayout.Height(Screen.height - 140)))
						{
							PhotonNetwork.player.SetTeam(PunTeams.Team.blue);
							currentStatus = Status.pickingCharacter;
						}

						GUILayout.FlexibleSpace();

						if (GUILayout.Button("red", m_PickButtonStyle, GUILayout.Width(Screen.width * 0.5f - 20), GUILayout.Height(Screen.height - 140)))
						{
							PhotonNetwork.player.SetTeam(PunTeams.Team.red);
							currentStatus = Status.pickingCharacter;
						}
					}
					GUILayout.EndHorizontal();
				}
				GUILayout.EndArea();
			}
			break;
		case Status.pickingCharacter:
			{
				LoadStyles();
				GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20));
				{
					GUILayout.BeginHorizontal();
					{
						if (GUILayout.Button("healer", m_PickButtonStyle, GUILayout.Width(Screen.width * 0.25f - 20), GUILayout.Height(Screen.height - 20)))
						{
							currentStatus = Status.inGame;
							GetComponent<PlayerSpawner> ().SpawnMyPlayer ("Healer");
						}

						GUILayout.FlexibleSpace();

						if (GUILayout.Button("flash", m_PickButtonStyle, GUILayout.Width(Screen.width * 0.25f), GUILayout.Height(Screen.height - 20)))
						{
							currentStatus = Status.inGame;
							GetComponent<PlayerSpawner> ().SpawnMyPlayer ("Flash");
						}

						if (GUILayout.Button("tank", m_PickButtonStyle, GUILayout.Width(Screen.width * 0.25f), GUILayout.Height(Screen.height - 20)))
						{
							currentStatus = Status.inGame;
							GetComponent<PlayerSpawner> ().SpawnMyPlayer ("Juggernaut");
						}

						if (GUILayout.Button("sniper", m_PickButtonStyle, GUILayout.Width(Screen.width * 0.25f), GUILayout.Height(Screen.height - 20)))
						{
							currentStatus = Status.inGame;
							GetComponent<PlayerSpawner> ().SpawnMyPlayer ("Sniper");
						}
					}
					GUILayout.EndHorizontal();
				}
				GUILayout.EndArea();
			}
			break;
		case Status.inGame:
			break;
		}
	}

	void LoadStyles()
	{
		if (m_PickButtonStyle == null)
		{
			m_PickButtonStyle = new GUIStyle(Styles.Button);
			m_PickButtonStyle.fontSize = 60;
		}
	}
}
