using UnityEngine;

public class SplashManager : MonoBehaviour
{
    [SerializeField] GameObject subtitle;
    [SerializeField] float loadDelay = 3;
    [SerializeField] float blinkSpeed = .333f;
    private bool aglow = false;
    private float blinkTime = 0;

    LevelManager levelManager;

    private void Start()            { levelManager = FindObjectOfType<LevelManager>(); }
    private void OnEnable()         { Invoke("LoadNextLevel", loadDelay); }
    private void Update()           { if (subtitle) Blink(subtitle); }
    private void LoadNextLevel()    { levelManager.LoadNextLevel(); }

    private void Blink(GameObject GUItext)
    {
        if (Time.time > blinkTime + blinkSpeed)
        {
            GUItext.SetActive(aglow);
            aglow = !aglow;
            blinkTime = Time.time;
        }
    }
}
