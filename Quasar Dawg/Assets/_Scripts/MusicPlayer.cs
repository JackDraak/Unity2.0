using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    private bool music = true;
    private float priorTimeScale;
    private KeyCode musicKey;
    private KeyValet keyManager;

    private void OnEnable()
    {
        // Singleton pattern, preferred over making the class static:
        MusicPlayer[] checker;
        checker = FindObjectsOfType<MusicPlayer>();
        if (checker.Length > 1) Destroy(gameObject);
        else DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (!(audioSource = GetComponent<AudioSource>())) Debug.Log("MusicPlayer.cs: audioSource INFO, FAIL.");
        if (!(keyManager = FindObjectOfType<KeyValet>())) Debug.Log("MusicPlayer.cs: keyManager INFO, FAIL.");
        musicKey = keyManager.GetKey("MusicPlayer-MusicToggle");
    }

    private void Update()
    {
        if (priorTimeScale != Time.timeScale)
        {
            AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
            foreach (AudioSource audioSource in audioSources) audioSource.pitch = Time.timeScale;
            priorTimeScale = Time.timeScale;
        }
        if (Input.GetKeyDown(musicKey)) music = !music;
        if (music && !audioSource.isPlaying) audioSource.Play();
        else if (!music && audioSource.isPlaying) audioSource.Pause();
    }

    public void TogglePause()
    {
        music = !music;
    }
}
