using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelValet : MonoBehaviour
{
    static LevelValet instance = null;

    private void Start()
    {
        if (instance != null && instance != this) { Destroy(gameObject); }
        else { instance = this; DontDestroyOnLoad(gameObject); }
    }

    public void LoadLevelOne()      { SceneManager.LoadScene(1); }
    public void LoadNextLevel()     { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); }
    public void LoadSplash()        { SceneManager.LoadScene(0); }
}
