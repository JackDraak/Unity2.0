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
        KeyValet[] checker;
        checker = FindObjectsOfType<KeyValet>();
        if (checker.Length > 1) Destroy(gameObject);
        else DontDestroyOnLoad(gameObject);
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
            case "PlayerController-ToggleEnergyMax":    handback = KeyCode.K; break;
            case "PlayerController-ToggleInvulnerable": handback = KeyCode.L; break;
            case "PlayerController-WeaponCharge":       handback = KeyCode.U; break;
            default: break;
        }
        return handback;
    }
}
