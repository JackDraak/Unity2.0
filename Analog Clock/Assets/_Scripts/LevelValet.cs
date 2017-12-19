using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelValet : MonoBehaviour
{
    private static bool overlay = true;
    private static int fontsize = 18;

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

    public int GetFontsize()            { return fontsize; }
    public bool GetOverlay()            { return overlay; }
    public void SetFontsize(int size)   { fontsize = size; }
    public void SetOverlay(bool torf)   { overlay = torf; }
    public void ToggleOverlay()         { overlay = !overlay; }
    public void FontUp()                { fontsize++; }
    public void FontDown()              { fontsize--; }
}
