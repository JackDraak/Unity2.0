using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour {
//    [SerializeField] GameObject sumPause;
	static MusicPlayer instance = null;
	public AudioClip[] level_;

	private AudioSource music;
    private int cue = 1;
    private bool wasPlaying = false;
    private bool activated = true;

  /*  public void OnApplicationPause(bool pause)
    {
        if (pause && music.isPlaying)
        {
            music.Pause();
            wasPlaying = true;
        }
        else if (!pause && !music.isPlaying && wasPlaying)
        {
            music.Play();
            wasPlaying = true;
        }
        else if (pause && !music.isPlaying)
        {
            wasPlaying = false;
        }
    } */

    void Start ()
    {
		if (instance != null && instance != this) { Destroy (gameObject); }
		else
        {
			instance = this;
			GameObject.DontDestroyOnLoad(gameObject);
			if (GameObject.Find ("MusicPlayer"))
				music = GameObject.Find ("MusicPlayer").GetComponent<AudioSource>();
			if (music) music.loop = true;
		}
	}

    void FixedUpdate()
    {
        bool key_m = Input.GetKeyDown(KeyCode.M);
        if (key_m) ToggleMusic();
    }

    private void ToggleMusic()
    {
        activated = !activated;
        if (activated) music.Play();
        else music.Pause();
    }

    void OnEnable()
    {
        SumPause.pauseEvent += OnPause;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SumPause.pauseEvent -= OnPause;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (music)
        {
            music.Stop();
            music.clip = level_[scene.buildIndex]; // setup clips-quesheet in the inspector
            music.loop = true;
            music.volume = 0.15f;
            music.Play();
            wasPlaying = true;
        }
    }

    /// <summary>What to do when the pause button is pressed.</summary>
    /// <param name="paused">New pause state</param>
    void OnPause(bool paused)
    {
        if (paused)
        {
            // Code to execute when the game is paused
            Debug.Log("Pause");
            if (paused && music.isPlaying)
            {
                music.Pause();
                wasPlaying = true;
            }
        }
        else
        {
            music.volume = 0.15f;
            music.Play();
            wasPlaying = true;
            // Code to execute when the game is resumed
            Debug.Log("Resume");
        }
    }

    /*  private void OnLevelWasLoaded(int level)
      { 
          if (music)
          {
              music.Stop();
              music.clip = level_[level]; // setup clips-quesheet in the inspector
              music.loop = true;
              music.volume = 0.15f;
              music.Play();
              wasPlaying = true;
          }
      } */
}
