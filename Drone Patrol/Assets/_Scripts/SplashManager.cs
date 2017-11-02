using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashManager : MonoBehaviour {

	public float autoLoadNextLevelDelay = 4;
    private string mainMenu = "_StartMenu_DP";

	//private OptionsController optionsController;
	
	void Start () {
		Cursor.visible = false;
	//	optionsController = GameObject.FindObjectOfType<OptionsController>(); if (!optionsController) Debug.LogError (this + ":ERROR: unable to attach to OptionsController to reset options to defaults");
	//	optionsController.SetDefaults();
	//	optionsController.Save ();
		Invoke("LoadNextLevel", autoLoadNextLevelDelay);
	}

    void Update()
    {
        bool key = Input.anyKeyDown;
        if (key) SceneManager.LoadScene(mainMenu);
    }

    public void LoadNextLevel() {
		Cursor.visible = true;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
	}
}
