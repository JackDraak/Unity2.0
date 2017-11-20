using UnityEngine;

public class KeyManager : MonoBehaviour
{
    static KeyManager instance = null;

    private void OnEnable()
    {
        if (instance != null && instance != this) { Destroy(gameObject); }
        else { instance = this; GameObject.DontDestroyOnLoad(gameObject); }
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
            case "PlayerController-ShieldCharge":    handback = KeyCode.U; break;
            case "PlayerController-WeaponCharge":    handback = KeyCode.Y; break;
            default: break;
        }
        return handback;
    }
}
