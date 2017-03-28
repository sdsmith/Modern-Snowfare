using UnityEngine;
using System.Collections;

/// <summary>
/// This class checks the winning conditions and handles round restart
/// </summary>
public class GamemodeCaptureTheFlag : GamemodeBase 
{
	/// <summary>
	/// How long is one match?
	/// </summary>
	public const float TotalRoundTime = 15 * 60;


	/// <summary>
	/// How many captures are needed till one team wins?
	/// </summary>
	public const int TotalFlagCaptures = 3;

	/// <summary>
	/// The spawn point for the red team
	/// </summary>
	public Transform RedSpawnPoint;

	/// <summary>
	/// The spawn point for the blue team
	/// </summary>
	public Transform BlueSpawnPoint;

	public override void OnSetup()
	{
		if( PhotonNetwork.isMasterClient == true )
		{
			SetRoundStartTime();
		}
	}

	public override void OnTearDown()
	{
		Destroy( PickupFlag.RedFlag.gameObject );
		Destroy( PickupFlag.BlueFlag.gameObject );
	}

	public override Gamemode GetGamemodeType()
	{
		return Gamemode.CaptureTheFlag;
	}

	/// <summary>
	/// Increases the score of a team by one
	/// </summary>
	/// <param name="team">The team.</param>
	public void IncreaseTeamScore( Team team )
	{
		//We need to know which property we have to change, blue or red
		string property = RoomProperty.BlueScore;

		if( team == Team.Red )
		{
			property = RoomProperty.RedScore;
		}

		ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable();
		//In case the property doesn't yet exist, create it with a score of 1
		newProperties.Add( property, 1 );

		if( PhotonNetwork.room.CustomProperties.ContainsKey( property ) == true )
		{
			//if the property does exist, we just add one to the old value
			newProperties[ property ] = (int)PhotonNetwork.room.CustomProperties[ property ] + 1;
		}

		PhotonNetwork.room.SetCustomProperties( newProperties );
	}

	public override bool IsRoundFinished()
	{
		int blueScore = 0;
		int redScore = 0;
		double timePassed = Time.timeSinceLevelLoad;

		if( PhotonNetwork.room != null )
		{
			if( PhotonNetwork.room.CustomProperties.ContainsKey( RoomProperty.BlueScore ) == true )
			{
				blueScore = (int)PhotonNetwork.room.CustomProperties[ RoomProperty.BlueScore ];
			}

			if( PhotonNetwork.room.CustomProperties.ContainsKey( RoomProperty.RedScore ) == true )
			{
				redScore = (int)PhotonNetwork.room.CustomProperties[ RoomProperty.RedScore ];
			}

			if( PhotonNetwork.room.CustomProperties.ContainsKey( RoomProperty.StartTime ) == true )
			{
				timePassed = PhotonNetwork.time - (double)PhotonNetwork.room.CustomProperties[ RoomProperty.StartTime ];
			}
		}

		return blueScore >= TotalFlagCaptures || redScore >= TotalFlagCaptures || timePassed >= TotalRoundTime;
	}

	public Team GetWinningTeam()
	{
		int blueScore = 0;
		int redScore = 0;
		float timePassed = Time.timeSinceLevelLoad;

		if( PhotonNetwork.room != null )
		{
			if( PhotonNetwork.room.CustomProperties.ContainsKey( RoomProperty.BlueScore ) == true )
			{
				blueScore = (int)PhotonNetwork.room.CustomProperties[ RoomProperty.BlueScore ];
			}

			if( PhotonNetwork.room.CustomProperties.ContainsKey( RoomProperty.RedScore ) == true )
			{
				redScore = (int)PhotonNetwork.room.CustomProperties[ RoomProperty.RedScore ];
			}
		}

		if( blueScore >= TotalFlagCaptures )
		{
			return Team.Blue;
		}
		else if( redScore >= TotalFlagCaptures )
		{
			return Team.Red;
		}
		else if( Mathf.Ceil( timePassed ) >= TotalRoundTime )
		{
			if( blueScore > redScore )
			{
				return Team.Blue;
			}
			else if( redScore > blueScore )
			{
				return Team.Red;
			}
		}

		return Team.None;
	}

	public override Transform GetSpawnPoint( Team team )
	{
		if( team == Team.Blue )
		{
			return BlueSpawnPoint;
		}

		return RedSpawnPoint;
	}

	public override bool IsUsingTeams()
	{
		return true;
	}
}
