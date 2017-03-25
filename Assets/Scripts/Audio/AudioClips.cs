using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClips : MonoBehaviour {

    public static List<AudioClip> playerThrow;
    public static List<AudioClip> snowballImpacts;
    public static AudioClip targetHit;
    public static AudioClip playerKill;
    public static List<AudioClip> playerDeath;
    public static AudioClip playerDeathScreen;

    public static List<AudioClip> footsteps;
    public static AudioClip jump;
    public static AudioClip land;


    void Awake() {
        playerThrow = new List<AudioClip>();
        playerThrow.Add((AudioClip)Resources.Load("SnowBallThrow1"));
        playerThrow.Add((AudioClip)Resources.Load("SnowBallThrow2"));
        playerThrow.Add((AudioClip)Resources.Load("SnowBallThrow3"));

        snowballImpacts = new List<AudioClip>();
        snowballImpacts.Add((AudioClip)Resources.Load("SnowballHit1"));
        snowballImpacts.Add((AudioClip)Resources.Load("SnowballHit2"));
        snowballImpacts.Add((AudioClip)Resources.Load("SnowballHit3"));

        targetHit = (AudioClip)Resources.Load("TargetHit");

        playerKill = (AudioClip)Resources.Load("KillConfirmed");

        playerDeath = new List<AudioClip>();
        playerDeath.Add((AudioClip)Resources.Load("DeathScream1"));
        playerDeath.Add((AudioClip)Resources.Load("DeathScream2"));
        playerDeath.Add((AudioClip)Resources.Load("DeathScream3"));
        playerDeath.Add((AudioClip)Resources.Load("DeathScream_-_Wilhelm"));

        playerDeathScreen = (AudioClip)Resources.Load("YouDied");

        footsteps = new List<AudioClip>();
        footsteps.Add((AudioClip)Resources.Load("Footstep01"));
        footsteps.Add((AudioClip)Resources.Load("Footstep02"));
        footsteps.Add((AudioClip)Resources.Load("Footstep03"));
        footsteps.Add((AudioClip)Resources.Load("Footstep04"));

        jump = (AudioClip)Resources.Load("Jump");

        land = (AudioClip)Resources.Load("Land");
    }

    public static AudioClip GetRand(List<AudioClip> clips) {
        return clips[Random.Range(0, clips.Count - 1)];
    }

    void OnDestroy() {
    }
}

