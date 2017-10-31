using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class OptionsController : MonoBehaviour {
	
	public Slider autoplaySlider, easySlider, fireballsSlider, speedSlider, trailsSlider;

	private LevelManager levelManager;
	
	public void Save () {
		PlayerPrefsManager.SetSpeed (speedSlider.value);
		if (autoplaySlider.value == 1) PlayerPrefsManager.SetAutoplay(true); else PlayerPrefsManager.SetAutoplay(false);
		if (easySlider.value == 1) PlayerPrefsManager.SetEasy(true); else PlayerPrefsManager.SetEasy(false);
		if (fireballsSlider.value == 1) PlayerPrefsManager.SetFireBalls(true); else PlayerPrefsManager.SetFireBalls(false);
		if (trailsSlider.value == 1) PlayerPrefsManager.SetTrails(true); else PlayerPrefsManager.SetTrails(false);
	}
	
	public void SaveAndExit () {
		Save ();
		levelManager.LoadLevel ("_Start Menu");
	}
	
	public void SetDefaults () {
		autoplaySlider.value = 0;
		easySlider.value = 0;
		fireballsSlider.value = 0;
		speedSlider.value = 0.7f;
		trailsSlider.value = 1;
	}

	void Start () {
		if (SceneManager.GetActiveScene().buildIndex != 0) {
			levelManager = GameObject.FindObjectOfType<LevelManager>(); if (!levelManager) Debug.LogError (this + ": unable to attach to LevelManager");
		}
		speedSlider.value = PlayerPrefsManager.GetSpeed ();
		if (PlayerPrefsManager.GetAutoplay () == true) autoplaySlider.value = 1; else autoplaySlider.value = 0;
		if (PlayerPrefsManager.GetEasy () == true) easySlider.value = 1; else easySlider.value = 0;
		if (PlayerPrefsManager.GetFireBalls () == true) fireballsSlider.value = 1; else fireballsSlider.value = 0;
		if (PlayerPrefsManager.GetTrails () == true) trailsSlider.value = 1; else trailsSlider.value = 0;
	}
}
