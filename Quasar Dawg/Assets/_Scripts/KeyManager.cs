using UnityEngine;

public class KeyManager : MonoBehaviour
{
    // DEVNOTE: Super-simple key-manager to consolidate key-bindings to once place.
    // TODO: extend functionality to allow for re-asignments or other features.

    static KeyManager instance = null;

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
            case "MusicPlayer-MusicToggle":          handback = KeyCode.M; break;
            case "ObjectSpawner-DespawnCommand":     handback = KeyCode.P; break;
            case "ObjectSpawner-SpawnAllCommand":    handback = KeyCode.I; break;
            case "ObjectSpawner-SpawnRandomCommand": handback = KeyCode.O; break;
            case "PlayerController-ShieldCharge":    handback = KeyCode.Y; break;
            case "PlayerController-WeaponCharge":    handback = KeyCode.U; break;
            default: break;
        }
        return handback;
    }
}
