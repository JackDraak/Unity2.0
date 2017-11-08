using UnityEngine;

public class MusicPlayer : MonoBehaviour {
     static MusicPlayer instance = null;

    void OnEnable()
    {
        if (instance != null && instance != this) { Destroy(gameObject); }
        else { instance = this; GameObject.DontDestroyOnLoad(gameObject); }
    }
}
