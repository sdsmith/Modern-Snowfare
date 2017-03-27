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

    Texture2D sniperIcon;
    Texture2D juggyIcon;
    Texture2D healerIcon;
    Texture2D flashIcon;
    GUIContent sniperContent;
    GUIContent juggyContent;
    GUIContent healerContent;
    GUIContent flashContent;


    // Use this for initialization
    void Start () {
		currentStatus = Status.pickingTeam;

        sniperIcon = Resources.Load<Texture2D>("GUI/sniper");
        juggyIcon = Resources.Load<Texture2D>("GUI/juggy");
        healerIcon = Resources.Load<Texture2D>("GUI/healer");
        flashIcon = Resources.Load<Texture2D>("GUI/flash");

        sniperContent = new GUIContent();
        sniperContent.image = sniperIcon;

        juggyContent = new GUIContent();
        juggyContent.image = juggyIcon;

        healerContent = new GUIContent();
        healerContent.image = healerIcon;

        flashContent = new GUIContent();
        flashContent.image = flashIcon;
    }
	
	// Update is called once per frame
	void Update () {
		if (currentStatus == Status.inGame) {
			// KeyCode.BackQuote is ` key
			// When user presses shift and `, leave game
			if (Input.GetKey (KeyCode.BackQuote) && 
				(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) 
			{
				PhotonNetwork.LeaveRoom ();
			}
		}
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
						if (GUILayout.Button(GetButtonLabel(PunTeams.Team.blue), m_PickButtonStyle, GUILayout.Width(Screen.width * 0.5f - 20), GUILayout.Height(Screen.height - 20)))
						{
							PhotonNetwork.player.SetTeam(PunTeams.Team.blue);
							currentStatus = Status.pickingCharacter;
						}

						GUILayout.FlexibleSpace();

						if (GUILayout.Button(GetButtonLabel(PunTeams.Team.red), m_PickButtonStyle, GUILayout.Width(Screen.width * 0.5f - 20), GUILayout.Height(Screen.height - 20)))
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
						if (GUILayout.Button(healerContent, m_PickButtonStyle, GUILayout.Width(Screen.width * 0.25f - 20), GUILayout.Height(Screen.height - 20)))
						{
							currentStatus = Status.inGame;
							GetComponent<PlayerSpawner> ().SpawnMyPlayer ("Healer");
						}

						GUILayout.FlexibleSpace();

						if (GUILayout.Button(flashContent, m_PickButtonStyle, GUILayout.Width(Screen.width * 0.25f), GUILayout.Height(Screen.height - 20)))
						{
							currentStatus = Status.inGame;
							GetComponent<PlayerSpawner> ().SpawnMyPlayer ("Flash");
						}

						if (GUILayout.Button(juggyContent, m_PickButtonStyle, GUILayout.Width(Screen.width * 0.25f), GUILayout.Height(Screen.height - 20)))
						{
							currentStatus = Status.inGame;
							GetComponent<PlayerSpawner> ().SpawnMyPlayer ("Juggernaut");
						}

						if (GUILayout.Button(sniperContent, m_PickButtonStyle, GUILayout.Width(Screen.width * 0.25f), GUILayout.Height(Screen.height - 20)))
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
			// Logout description found in the top left corner
			GUILayout.Label("Logout: shift + ~");
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

	string GetButtonLabel( PunTeams.Team team )
	{
		GameObject[] playerObjects = GameObject.FindGameObjectsWithTag( "Player" );
		int playerCount = 0;

		for( int i = 0; i < playerObjects.Length; ++i )
		{
			if( playerObjects[ i ].GetPhotonView().owner.GetTeam() == team )
			{
				playerCount++;
			}
		}

		string label = team.ToString() + " team\n";
		label += playerCount.ToString();

		if( playerCount == 1 )
		{
			label += " player";
		}
		else
		{
			label += " players";
		}

		return label;
	}
}
