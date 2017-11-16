using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    static LevelManager instance = null;

    private void Start()
    {
        if (instance != null && instance != this) { Destroy(gameObject); }
        else { instance = this; GameObject.DontDestroyOnLoad(gameObject); }
    }

    public void LoadSplash() { SceneManager.LoadScene(0); }
    public void LoadLevelOne() { SceneManager.LoadScene(1); }
    public void LoadNextLevel() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); }
}
