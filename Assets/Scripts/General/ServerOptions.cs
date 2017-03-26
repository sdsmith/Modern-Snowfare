using UnityEngine;
using System.Collections;

public class ServerOptions 
{
	public static string[] AvailableMaps = new string[] 
	{ 
		"LeChamp",
		"Healer",
		"Flash",
		"main"
	};

	public static Gamemode[] AvailableModes = new Gamemode[] 
	{ 
		Gamemode.CaptureTheFlag, 
		Gamemode.Deathmatch,
		Gamemode.TeamDeathmatch
	};

	public static void CreateRoom( string name, string mapQueueString, int skillLevel = 5 )
	{
		MapQueueEntry firstMap = MapQueue.GetSingleEntryInMapQueue( mapQueueString, 0 );

		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = 8;

		roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
		roomOptions.CustomRoomProperties.Add( RoomProperty.MapQueue, mapQueueString );
		roomOptions.CustomRoomProperties.Add( RoomProperty.MapIndex, 0 );
		roomOptions.CustomRoomProperties.Add( RoomProperty.RedScore, 0 );
		roomOptions.CustomRoomProperties.Add( RoomProperty.BlueScore, 0 );
		roomOptions.CustomRoomProperties.Add( RoomProperty.Map, firstMap.Name );
		roomOptions.CustomRoomProperties.Add( RoomProperty.Mode, (int)firstMap.Mode );
		Debug.Log (firstMap.Mode);
		roomOptions.CustomRoomProperties.Add( RoomProperty.SkillLevel, skillLevel );

		roomOptions.CustomRoomPropertiesForLobby = new string[] {
			RoomProperty.Map,
			RoomProperty.Mode,
			RoomProperty.SkillLevel,
		};

		PhotonNetwork.JoinOrCreateRoom( name, roomOptions, MultiplayerConnector.Lobby );
	}
}
