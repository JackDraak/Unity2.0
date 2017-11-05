using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour {
	
	const string MUSIC_KEY = "music_toggle";
	const string MASTER_VOL_KEY = "master_volume"; // TODO work in progress, not in options_scene
	const string GRAVITY_KEY = "gravity";
	const string TOPSCORE_KEY = "topscore";

	// Autoplay
	public static bool GetAutoplay () { if (PlayerPrefs.GetInt (MUSIC_KEY) == 1) return true; else return false; }
	public static void SetAutoplay (bool set) { if (set) PlayerPrefs.SetInt (MUSIC_KEY, 1); else PlayerPrefs.SetInt (MUSIC_KEY, 0); }
	
	// MasterVolume
	public static float GetMasterVolume () { return PlayerPrefs.GetFloat (MASTER_VOL_KEY); }
	public static void SetMasterVolume (float volume) { 
		if (volume >= 0f && volume <= 1f) { PlayerPrefs.SetFloat (MASTER_VOL_KEY, volume); } 
		else {Debug.LogError ("Master volume out of range"); }
	}

	// Gravity
	public static float GetSpeed () { return PlayerPrefs.GetFloat (GRAVITY_KEY); }
	public static void SetSpeed (float yorn) { PlayerPrefs.SetFloat (GRAVITY_KEY, yorn); }
	
	// Topscore
	public static float GetTopscore () { return PlayerPrefs.GetFloat (TOPSCORE_KEY); }
	public static void SetTopscore (float yorn) { PlayerPrefs.SetFloat (TOPSCORE_KEY, yorn); }
}
