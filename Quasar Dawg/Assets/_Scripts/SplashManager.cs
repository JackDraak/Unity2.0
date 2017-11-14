using UnityEngine;
using System.Collections;

public class SplashManager : MonoBehaviour
{
    [SerializeField] GameObject subtitle;
    private bool aglow = false;
    private float blinkDelay = .75f;
    private float blinkTime = 0;

    LevelManager levelManager;

    private void Start()            { levelManager = FindObjectOfType<LevelManager>(); }
    private void OnEnable()         { Invoke("LoadNextLevel", 5f); }
    private void Update()           { if (subtitle) Blink(subtitle); }
    private void LoadNextLevel()    { levelManager.LoadNextLevel(); }

    private void Blink(GameObject GUItext)
    {
        {
            if (Time.time > blinkTime + blinkDelay)
            {
                GUItext.SetActive(aglow);
                aglow = !aglow;
                blinkTime = Time.time;
            }
        }
    }
}
