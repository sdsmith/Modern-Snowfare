using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGUI : MonoBehaviour {

 	enum Status
	{
		pickingTeam, 
		pickingCharacter,
		ready,
		inGame
	}

	Status currentStatus;

	GUIStyle m_PickButtonStyle;
	public Font ButtonFont;
	public Texture2D ButtonBackground;

	List<string> chatMessages;
	int maxChatMessages = 5;

	// Use this for initialization
	void Start () {
		currentStatus = Status.pickingTeam;
		chatMessages = new List<string>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI() {

		// Status of the connection found in the top left corner.
		GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

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
							currentStatus = Status.ready;
							GetComponent<PlayerSpawner> ().SpawnMyPlayer ("Healer");
						}

						GUILayout.FlexibleSpace();

						if (GUILayout.Button("flash", m_PickButtonStyle, GUILayout.Width(Screen.width * 0.25f), GUILayout.Height(Screen.height - 20)))
						{
							currentStatus = Status.ready;
							GetComponent<PlayerSpawner> ().SpawnMyPlayer ("Flash");
						}

						if (GUILayout.Button("tank", m_PickButtonStyle, GUILayout.Width(Screen.width * 0.25f), GUILayout.Height(Screen.height - 20)))
						{
							currentStatus = Status.ready;
							GetComponent<PlayerSpawner> ().SpawnMyPlayer ("Juggernaut");
						}

						if (GUILayout.Button("sniper", m_PickButtonStyle, GUILayout.Width(Screen.width * 0.25f), GUILayout.Height(Screen.height - 20)))
						{
							currentStatus = Status.ready;
							GetComponent<PlayerSpawner> ().SpawnMyPlayer ("Sniper");
						}
					}
					GUILayout.EndHorizontal();
				}
				GUILayout.EndArea();
			}
			break;
		case Status.ready:
			{
				GUILayout.BeginArea (new Rect (0, 0, Screen.width, Screen.height));
				GUILayout.BeginVertical ();
				GUILayout.FlexibleSpace ();

				foreach (string msg in chatMessages) {
					GUILayout.Label (msg);
				}

				GUILayout.EndVertical ();
				GUILayout.EndArea ();
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

	// Add a chat message (currently appearing in the bottom left corner when a player spawns)
	public void AddChatMessage(string m) {

		// Send all buffered messages (messages that have been sent before the player joined)
		GetComponent<PhotonView>().RPC("AddChatMessage_RPC", PhotonTargets.AllBuffered, m);
	}

	[PunRPC]
	void AddChatMessage_RPC(string m) {
		//When the max chat messages have been recieved, remove the oldest one to make room
		while (chatMessages.Count >= maxChatMessages) {
			chatMessages.RemoveAt(0);
		}
		chatMessages.Add(m);
	}

}
