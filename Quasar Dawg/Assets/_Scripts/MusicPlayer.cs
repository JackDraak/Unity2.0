using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    static MusicPlayer instance = null;

    private bool music = true;
    private AudioSource audioSource;
    private KeyManager keyManager;
    private KeyCode musicKey;

    private void OnEnable()
    {
        if (instance != null && instance != this) { Destroy(gameObject); }
        else { instance = this; GameObject.DontDestroyOnLoad(gameObject); }
    }

    private void Start()
    {
        if (!(audioSource = GetComponent<AudioSource>())) Debug.Log("MusicPlayer.cs audioSource ERROR.");

        if (!(keyManager = FindObjectOfType<KeyManager>())) Debug.Log("MusicPlayer.cs keyManager ERROR.");
        musicKey = keyManager.GetKey("MusicPlayer-MusicToggle");
    }

    private void Update()
    {
        if (Input.GetKeyDown(musicKey)) music = !music;
        if (music && !audioSource.isPlaying) audioSource.Play();
        else if (!music && audioSource.isPlaying) audioSource.Pause();
    }
}
