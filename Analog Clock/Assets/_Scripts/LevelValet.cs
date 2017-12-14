using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelValet : MonoBehaviour
{
    private void OnEnable()
    {
        // Singleton pattern, preferred over making the class static:
        LevelValet[] objectCount;
        objectCount = FindObjectsOfType<LevelValet>();
        if (objectCount.Length > 1) Destroy(gameObject);
        else DontDestroyOnLoad(gameObject);
    }

    public void LoadNextLevel()
    {
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextScene > SceneManager.sceneCountInBuildSettings - 1) nextScene = 0;
        SceneManager.LoadScene(nextScene);
    }
}
