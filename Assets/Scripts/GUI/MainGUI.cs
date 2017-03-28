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

	float controlsFadeIn;
	GUIStyle controlsBackgroundStyle;
	protected GUIStyle ControlsBackgroundStyle
	{
		get
		{
			if( controlsBackgroundStyle == null )
			{
				controlsBackgroundStyle = new GUIStyle( GUI.skin.box );
				controlsBackgroundStyle.normal.background = ButtonBackground;
				controlsBackgroundStyle.padding = new RectOffset( 10, 10, 10, 10 );
			}

			return controlsBackgroundStyle;
		}
	}

	GUIStyle m_LabelStyle;
	protected GUIStyle LabelStyle
	{
		get
		{
			if( m_LabelStyle == null )
			{
				m_LabelStyle = new GUIStyle( GUI.skin.label );
				// m_LabelStyle.font = Font;
				m_LabelStyle.fontSize = 30;
				m_LabelStyle.alignment = TextAnchor.UpperLeft;
			}
			return m_LabelStyle;
		}
	}

	GUIStyle m_LabelStyleCentered;
	protected GUIStyle LabelStyleCentered
	{
		get
		{
			if( m_LabelStyleCentered == null )
			{
				m_LabelStyleCentered = new GUIStyle( LabelStyle );
				m_LabelStyleCentered.alignment = TextAnchor.UpperCenter;
			}
			return m_LabelStyleCentered;
		}
	}

	GUIStyle m_HeaderStyle;
	protected GUIStyle HeaderStyle
	{
		get
		{
			if( m_HeaderStyle == null )
			{
				m_HeaderStyle = new GUIStyle( LabelStyle );
				m_HeaderStyle.fontStyle = FontStyle.Bold;
			}

			return m_HeaderStyle;
		}
	}

	GUIStyle m_HeaderStyleCentered;
	protected GUIStyle HeaderStyleCentered
	{
		get
		{
			if( m_HeaderStyleCentered == null )
			{
				m_HeaderStyleCentered = new GUIStyle( HeaderStyle );
				m_HeaderStyleCentered.alignment = TextAnchor.UpperCenter;
			}

			return m_HeaderStyleCentered;
		}
	}

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

		UpdateControlsFadeIn();
	}

	void UpdateControlsFadeIn()
	{
		float target = 0f;
		if( Input.GetKey( KeyCode.C ) )
		{
			target = 1f;
		}

		controlsFadeIn = Mathf.Lerp( controlsFadeIn, target, Time.deltaTime * 10f );
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
			// top left corner
			GUILayout.Label("Controls: C");
			GUILayout.Label("Scoreboard: TAB");
			GUILayout.Label("Logout: SHIFT + ~");

			DrawControls();
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

	protected void DrawControls()
	{
		if( controlsFadeIn == 0 )
		{
			return;
		}
			
		float width = 680;
		float height = 325;

		GUILayout.BeginArea( new Rect( ( Screen.width - width ) * 0.5f + ( 1 - controlsFadeIn ) * -Screen.width, ( Screen.height - height ) * 0.5f, width, height ), ControlsBackgroundStyle );
		{
			GUILayout.BeginHorizontal();
			{
				GUILayout.Label("Controls", HeaderStyleCentered );
			}
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal();
			{
				GUILayout.Label("Keyboard", HeaderStyleCentered, GUILayout.Width( 300 ) );
				GUILayout.Label("Mouse", HeaderStyleCentered, GUILayout.Width( 400 ) );
			}
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal();
			{
				GUILayout.Label ("W - Move Forward", LabelStyle, GUILayout.Width(300));
				GUILayout.Label ("Mouse Movement - Aim", LabelStyle, GUILayout.Width(400));
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			{
				GUILayout.Label ("S - Move Backward", LabelStyle, GUILayout.Width(300));
				GUILayout.Label ("Left Click - Fire", LabelStyle, GUILayout.Width(400));
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			{
				GUILayout.Label ("A - Move Left", LabelStyle, GUILayout.Width(300));
				GUILayout.Label ("Right Click - Drop Object", LabelStyle, GUILayout.Width(400));
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginVertical ();
			{
				GUILayout.Label ("D - Move Right", LabelStyle, GUILayout.Width(300));
				GUILayout.Label ("Spacebar - Jump", LabelStyle, GUILayout.Width(400));
			}
			GUILayout.EndVertical ();
		}
		GUILayout.EndArea();

		GUI.color = Color.white;
	}
}
