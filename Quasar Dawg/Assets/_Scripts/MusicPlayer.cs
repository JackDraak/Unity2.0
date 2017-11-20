using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    static MusicPlayer instance = null;

    private bool music = true;
    private AudioSource audioSource;

    private void OnEnable()
    {
        if (instance != null && instance != this) { Destroy(gameObject); }
        else { instance = this; GameObject.DontDestroyOnLoad(gameObject); }

        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) music = !music; // TODO: KeyManager.cs -- one key to rule them all.
        if (music && !audioSource.isPlaying) audioSource.Play();
        else if (!music && audioSource.isPlaying) audioSource.Pause();
    }
}
