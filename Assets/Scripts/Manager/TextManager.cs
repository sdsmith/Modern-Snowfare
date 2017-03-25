using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManager : MonoBehaviour {

	List<string> textMessages;
	int maxChatMessages = 5;

	// Use this for initialization
	void Start () {
		textMessages = new List<string>();
	}

	void OnGUI() {

		// Status of the connection found in the top left corner.
		GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

		GUILayout.BeginArea (new Rect (0, 0, Screen.width, Screen.height));
		GUILayout.BeginVertical ();
		GUILayout.FlexibleSpace ();

		foreach (string msg in textMessages) {
			GUILayout.Label (msg);
		}

		GUILayout.EndVertical ();
		GUILayout.EndArea ();
	}

	// Add a chat message (currently appearing in the bottom left corner when a player spawns)
	public void AddChatMessage(string m) {

		// Send all buffered messages (messages that have been sent before the player joined)
		GetComponent<PhotonView>().RPC("AddChatMessage_RPC", PhotonTargets.AllBuffered, m);
	}

	[PunRPC]
	void AddChatMessage_RPC(string m) {
		//When the max chat messages have been recieved, remove the oldest one to make room
		while (textMessages.Count >= maxChatMessages) {
			textMessages.RemoveAt(0);
		}
		textMessages.Add(m);
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

}
