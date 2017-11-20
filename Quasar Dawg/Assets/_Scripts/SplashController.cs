using UnityEngine;

public class SplashController : MonoBehaviour
{
    [SerializeField] GameObject subtitle;
    [SerializeField] float blinkSpeed = .333f;
    [SerializeField] float loadDelay = 3;

    private bool aglow = false;
    private float blinkTime = 0;

    LevelValet levelValet;

    private void Start()            { levelValet = FindObjectOfType<LevelValet>(); }
    private void OnEnable()         { Invoke("LoadNextLevel", loadDelay); }
    private void Update()           { if (subtitle) Blink(subtitle); }
    private void LoadNextLevel()    { levelValet.LoadNextLevel(); }

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
