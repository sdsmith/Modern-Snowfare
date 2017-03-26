using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GamemodeGUIBase : MonoBehaviour 
{
	/// <summary>
	/// Regular Font (Jura)
	/// </summary>
	public Font Font;
	/// <summary>
	/// Bold Font (Orbitron)
	/// </summary>
	public Font BoldFont;

	Texture2D m_ButtonBackground;
	protected Texture2D ButtonBackground
	{
		get
		{
			if( m_ButtonBackground == null )
			{
				m_ButtonBackground = Resources.Load<Texture2D>( "GUI/ButtonBackground" );
			}

			return m_ButtonBackground;
		}
	}

	float m_LeaderboardsFadeIn;
	GUIStyle m_LeaderboardsBackgroundStyle;
	protected GUIStyle LeaderboardsBackgroundStyle
	{
		get
		{
			if( m_LeaderboardsBackgroundStyle == null )
			{
				m_LeaderboardsBackgroundStyle = new GUIStyle( GUI.skin.box );
				m_LeaderboardsBackgroundStyle.normal.background = ButtonBackground;
				m_LeaderboardsBackgroundStyle.padding = new RectOffset( 10, 10, 10, 10 );
			}

			return m_LeaderboardsBackgroundStyle;
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
				m_LabelStyle.font = Font;
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
				m_HeaderStyle.font = BoldFont;
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

	protected void UpdateLeaderboardsFadeIn()
	{
		float target = 0f;
		if( Input.GetKey( KeyCode.Tab ) )
		{
			target = 1f;
		}

		m_LeaderboardsFadeIn = Mathf.Lerp( m_LeaderboardsFadeIn, target, Time.deltaTime * 10f );
	}

	protected void DrawLeaderboards()
	{
		if( GamemodeManager.CurrentGamemode.IsRoundFinished() == true )
		{
			m_LeaderboardsFadeIn = 1;
		}

		if( m_LeaderboardsFadeIn == 0 )
		{
			return;
		}

		// width of all labels, plus 20 padding
		float width = 665;
		float height = 300;

		GUILayout.BeginArea( new Rect( ( Screen.width - width ) * 0.5f + ( 1 - m_LeaderboardsFadeIn ) * -Screen.width, ( Screen.height - height ) * 0.5f, width, height ), LeaderboardsBackgroundStyle );
		{
			GUILayout.BeginHorizontal();
			{
				GUILayout.Label( "Name", HeaderStyle, GUILayout.Width( 200 ) );
				GUILayout.Label( "Kills", HeaderStyleCentered, GUILayout.Width( 90 ) );
				GUILayout.Label( "Deaths", HeaderStyleCentered, GUILayout.Width( 135 ) );
				GUILayout.Label( "K/D", HeaderStyleCentered, GUILayout.Width( 80 ) );
				GUILayout.Label( "Points", HeaderStyleCentered, GUILayout.Width( 140 ) );
			}
			GUILayout.EndHorizontal();

			//Draw the list of all players and their kill counts
			List<PlayerController> sortedPlayers = GetSortedPlayers();
			for( int i = 0; i < sortedPlayers.Count; ++i )
			{
				string playerName = sortedPlayers[ i ].name;

				if( PhotonNetwork.connected == true )
				{
					playerName = sortedPlayers[ i ].GetComponent<PhotonView>().owner.NickName;
				}

				GUILayout.BeginHorizontal();
				{
					PunTeams.Team team = sortedPlayers [i].GetComponent<PhotonView> ().owner.GetTeam ();
					GUI.color = GetTeamColor( team );
					string kdrString = GetKDRatio (sortedPlayers [i]);

					GUILayout.Label( playerName, LabelStyle, GUILayout.Width( 200 ) );
					GUILayout.Label( sortedPlayers[ i ].killCount.ToString(), LabelStyleCentered, GUILayout.Width( 90 ) );
					GUILayout.Label( sortedPlayers[ i ].deathCount.ToString(), LabelStyleCentered, GUILayout.Width( 135 ) );
					GUILayout.Label( kdrString, LabelStyleCentered, GUILayout.Width( 80 ) );
					GUILayout.Label( sortedPlayers[ i ].flagCaptureCount.ToString(), LabelStyleCentered, GUILayout.Width( 140 ) );
				}
				GUILayout.EndHorizontal();
			}
		}
		GUILayout.EndArea();

		GUI.color = Color.white;
	}

	Color GetTeamColor( PunTeams.Team team )
	{
		switch( team )
		{
		case PunTeams.Team.red:
			return new Color( 1f, 0.4f, 0.4f );
		case PunTeams.Team.blue:
			return new Color( 0.4f, 0.4f, 1f );
		}

		return Color.white;
	}

	List<PlayerController> GetSortedPlayers()
	{
		GameObject[] playerObjects = GameObject.FindGameObjectsWithTag( "Player" );
		List<PlayerController> players = new List<PlayerController>();

		for( int i = 0; i < playerObjects.Length; ++i )
		{
			players.Add( playerObjects[ i ].GetComponent<PlayerController>() );
		}

		players.Sort( delegate( PlayerController p1, PlayerController p2 )
		{
			return p2.killCount.CompareTo( p1.killCount );
		} );

		return players;
	}

	/// <summary>
	/// This returns a formatted string showing the time
	/// </summary>
	/// <param name="timePassed">The time that should be formatted</param>
	/// <returns></returns>
	protected string GetTimeLeftString( double timePassed )
	{
		double timeLeft = GamemodeCaptureTheFlag.TotalRoundTime - timePassed;
		int minutesLeft = Mathf.FloorToInt( (float)timeLeft / 60 );
		int secondsLeft = Mathf.FloorToInt( (float)timeLeft ) % 60;

		return minutesLeft.ToString() + ":" + secondsLeft.ToString( "00" );
	}

	string GetKDRatio(PlayerController player) {
		float kdr = (float)player.killCount;
		// Don't divide by 0
		if (player.deathCount != 0) {
			kdr = (float)player.killCount / (float)player.deathCount;
		}
		return kdr.ToString("0.00");
	}
}
