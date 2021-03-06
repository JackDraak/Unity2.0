﻿using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    private static bool soundFX = true;

    private AudioSource audioSource;
    private float priorTimeScale;
    private KeyCode audioKey;
    private KeyValet keyValet;

    private void OnEnable()
    {
        // Singleton pattern, preferred over making the class static:
        AudioHandler[] objectCount;
        objectCount = FindObjectsOfType<AudioHandler>();
        if (objectCount.Length > 1) Destroy(gameObject);
        else DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (!(audioSource = GetComponent<AudioSource>())) Debug.Log("AudioHandler.cs: audioSource INFO, FAIL.");
        if (!(keyValet = FindObjectOfType<KeyValet>())) Debug.Log("AudioHandler.cs: keyManager INFO, FAIL.");
        audioKey = keyValet.GetKey("AudioHandler-AudioToggle");
    }

    private void Update()
    {
        if (priorTimeScale != Time.timeScale)
        {
            AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
            foreach (AudioSource audioSource in audioSources) audioSource.pitch = Time.timeScale;
            priorTimeScale = Time.timeScale;
        }
        if (Input.GetKeyDown(audioKey)) soundFX = !soundFX;
        if (soundFX && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        else if (!soundFX && audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    public bool GetFX()
    {
        return soundFX;
    }

    public void SetFX(bool torf)
    {
        soundFX = torf;
    }

    public void ToggleFX()
    {
        soundFX = !soundFX;
    }
}
