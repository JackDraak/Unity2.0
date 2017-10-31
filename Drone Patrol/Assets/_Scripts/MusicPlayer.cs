using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour {

	static MusicPlayer instance = null;

	public AudioClip[] level_;

	private AudioSource music;

	void Start () {
		if (instance != null && instance != this) { Destroy (gameObject); }
		else {
			instance = this;
			GameObject.DontDestroyOnLoad(gameObject);
			if (GameObject.Find ("MusicPlayer")) {
				music = GameObject.Find ("MusicPlayer").GetComponent<AudioSource>();
			}
			music.loop = true;
		}
	}
	
	void OnLevelWasLoaded(int level){
		if (music) {
			music.Stop();
			music.clip = level_[level]; // setup clips-quesheet in the inspector
			music.loop = true;
			music.volume = 0.86f;
			music.Play();
		}
	}
}
