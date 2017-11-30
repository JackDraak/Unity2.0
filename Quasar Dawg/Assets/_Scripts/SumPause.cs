using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
/// <summary>Singleton class for controlling pause functions.</summary>
public class SumPause : MonoBehaviour {
    private float priorTimeScale = 0; // JDraak

    // Event managers
    public delegate void PauseAction(bool paused);
    public static event PauseAction PauseEvent;

    // Variables set via inspector
    [SerializeField]
    bool useEvent = false, detectEscapeKey = true;
    [SerializeField]
    Sprite pausedSprite, playingSprite;

    // Link to button's image
    Image image;

    static bool status = false;
    /// <summary>
    /// Sets/Returns current pause state (true for paused, false for normal)
    /// </summary>
    public static bool Status {
        get { return status; }
        set {
            status = value;
            Debug.Log("Pause status set to " + status.ToString());

            OnChange();

            // Change image to the proper sprite if everything is set
            if (CheckLinks())
                instance.image.sprite = status ? instance.pausedSprite : instance.playingSprite;
            else
                Debug.LogError("Links missing on SumPause component. Please check the sumPauseButton object for missing references.");

            // Notify other objects of change
            if (instance.useEvent && PauseEvent != null)
                PauseEvent(status);
        }
    }

    // Instance used for singleton
    public static SumPause instance;

    void Awake () {
        image = GetComponent<Image>();
    }

    void Start () {
        // Ensure singleton
        if (SumPause.instance == null)
            SumPause.instance = this;
        else
            Destroy(this);
    }

    void Update() {
        // Listen for escape key and pause if needed
        if (detectEscapeKey && Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    /// <summary>
    /// Flips the current pause status. Called from the attached button in 
    /// the inspector.
    /// </summary>
    public void TogglePause () {
        Status = !Status;
    }

    /// <summary>Checks if all links are properly connected</summary>
    /// <returns>False means links are missing</returns>
    static bool CheckLinks () {
        return (instance.image != null && instance.playingSprite != null && instance.pausedSprite != null);
    }

    /// <summary>This is what we want to do when the game is paused or unpaused.</summary>
    static void OnChange() {
        if(status) {
            // What to do when paused
            instance.priorTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }
        else {
            // What to do when unpaused
            Time.timeScale = instance.priorTimeScale;
        }
    }
}
