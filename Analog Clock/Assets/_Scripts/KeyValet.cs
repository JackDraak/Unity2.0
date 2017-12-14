using UnityEngine;

public class KeyValet : MonoBehaviour
{
    // DEVNOTE: Super-simple key-manager to consolidate key-bindings to once place.
    // TODO: extend functionality to allow for re-asignments or other features.
    /*  
     * Use Case:
     * ---------  
       KeyValet keyValet;
       if (!(keyValet = FindObjectOfType<KeyValet>())) Debug.Log("Scriptname.cs keyValet not found - ERROR.");
        
       KeyCode commandKey;
       commandKey = keyValet.GetKey("Scriptname-Command");
        
       if (Input.GetKeyDown(commandKey)) IssueCommand();
     */

    private void OnEnable()
    {
        // Singleton pattern, preferred over making the class static:
        KeyValet[] objectCount;
        objectCount = FindObjectsOfType<KeyValet>();
        if (objectCount.Length > 1) Destroy(gameObject);
        else DontDestroyOnLoad(gameObject);
    }

    public KeyCode GetKey(string key)
    {
        KeyCode handback = KeyCode.None;
        switch (key)
        {
            case "AudioHandler-AudioToggle":    handback = KeyCode.M;       break;
            case "Clock-SwitchTheme":           handback = KeyCode.S;       break;
            case "Clock-ToggleClicks":          handback = KeyCode.C;       break;
            case "Clock-ToggleLamp":            handback = KeyCode.L;       break;
            case "Clock-ToggleOverlay":         handback = KeyCode.V;       break;
            case "Clock-ToggleStopwatch":       handback = KeyCode.Space;   break;
            case "Clock-Quit":                  handback = KeyCode.Q;       break;
            default: break;
        }
        return handback;
    }
}
