using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    static LevelManager instance = null;

    void Start()
    {
        if (instance != null && instance != this) { Destroy(gameObject); }
        else { instance = this; GameObject.DontDestroyOnLoad(gameObject); }
    }

    private void OnEnable()
    {
        Invoke("LoadNextLevel", 5f);
    }

    void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
