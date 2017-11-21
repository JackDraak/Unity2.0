using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelValet : MonoBehaviour
{
    private void OnEnable()
    {
        // Singleton pattern, preferred over making the class static:
        LevelValet[] checker;
        checker = FindObjectsOfType<LevelValet>();
        if (checker.Length > 1) Destroy(gameObject);
        else DontDestroyOnLoad(gameObject);
    }

    public void LoadLevelOne()      { SceneManager.LoadScene(1); }
    public void LoadNextLevel()     { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); }
    public void LoadSplash()        { SceneManager.LoadScene(0); }
}
