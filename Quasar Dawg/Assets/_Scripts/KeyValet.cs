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

    static KeyValet instance = null;

    private void OnEnable()
    {
        if (instance != null && instance != this) { Destroy(gameObject); }
        else { instance = this; DontDestroyOnLoad(gameObject); }
    }

    public KeyCode GetKey(string key)
    {
        KeyCode handback = KeyCode.None;
        switch (key)
        {
            // "Standard" keys....
            case "MusicPlayer-MusicToggle":             handback = KeyCode.M; break;

            // "Debug" keys...
            case "ObjectSpawner-DespawnCommand":        handback = KeyCode.P; break;
            case "ObjectSpawner-SpawnAllCommand":       handback = KeyCode.I; break;
            case "ObjectSpawner-SpawnRandomCommand":    handback = KeyCode.O; break;
            case "PlayerController-ShieldCharge":       handback = KeyCode.Y; break;
            case "PlayerController-WeaponCharge":       handback = KeyCode.U; break;
            case "PlayerController-ToggleInvulnerable": handback = KeyCode.L; break;
            case "PlayerController-ToggleEnergyMax":    handback = KeyCode.K; break;
            default: break;
        }
        return handback;
    }
}
