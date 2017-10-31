using UnityEngine;
using System.Collections;

public class PlayerPrefsManager : MonoBehaviour {
	
	const string AUTOPLAY_KEY = "autoplay";
	const string AWARD_KEY = "award";
	const string EASYMODE_KEY = "easymode";
	const string FIREBALLS_KEY = "fireballs";
	const string MASTER_VOL_KEY = "master_volume"; // TODO work in progress, not in options_scene
	const string SPEED_KEY = "speed";
	const string TOPSCORE_KEY = "topscore";
	const string TRAILS_KEY = "trails";

	// Autoplay
	public static bool GetAutoplay () { if (PlayerPrefs.GetInt (AUTOPLAY_KEY) == 1) return true; else return false; }
	public static void SetAutoplay (bool set) { if (set) PlayerPrefs.SetInt (AUTOPLAY_KEY, 1); else PlayerPrefs.SetInt (AUTOPLAY_KEY, 0); }
	
	// Award
	public static int GetAward () { return PlayerPrefs.GetInt (AWARD_KEY); }
	public static void SetAward (int award) { PlayerPrefs.SetInt (AWARD_KEY, award); }
	
	// Easy mode
	public static bool GetEasy () { if (PlayerPrefs.GetInt (EASYMODE_KEY) == 1) return true; else return false; }
	public static void SetEasy (bool set) { if (set) PlayerPrefs.SetInt (EASYMODE_KEY, 1); else PlayerPrefs.SetInt (EASYMODE_KEY, 0); }
	
	// Fireballs
	public static bool GetFireBalls () { if (PlayerPrefs.GetInt (FIREBALLS_KEY) == 1) return true; else return false; }
	public static void SetFireBalls (bool set) { if (set) PlayerPrefs.SetInt (FIREBALLS_KEY, 1); else PlayerPrefs.SetInt (FIREBALLS_KEY, 0); }

	// MasterVolume
	public static float GetMasterVolume () { return PlayerPrefs.GetFloat (MASTER_VOL_KEY); }
	public static void SetMasterVolume (float volume) { 
		if (volume >= 0f && volume <= 1f) { PlayerPrefs.SetFloat (MASTER_VOL_KEY, volume); } 
		else {Debug.LogError ("Master volume out of range"); }
	}

	// SpeedLimit
	public static float GetSpeed () { return PlayerPrefs.GetFloat (SPEED_KEY); }
	public static void SetSpeed (float yorn) { PlayerPrefs.SetFloat (SPEED_KEY, yorn); }
	
	// Topscore
	public static float GetTopscore () { return PlayerPrefs.GetFloat (TOPSCORE_KEY); }
	public static void SetTopscore (float yorn) { PlayerPrefs.SetFloat (TOPSCORE_KEY, yorn); }
	
	// TrippyTrails
	public static bool GetTrails () { if (PlayerPrefs.GetInt (TRAILS_KEY) == 1) return true; else return false; }
	public static void SetTrails (bool set) { if (set) PlayerPrefs.SetInt (TRAILS_KEY, 1); else PlayerPrefs.SetInt (TRAILS_KEY, 0); }
}
