using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
/// <summary>Singleton class for controlling pause functions.</summary>
public class SumPause : MonoBehaviour {

    private float lerpTimeScale; // JDraak
    private bool lerping = false;
    float beginLerpTime = 0;
    float ohToOne = 0;

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
        // if (!lerping) Status = !Status; // Flip current status
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
            instance.lerping = true;
            instance.beginLerpTime = Time.time;
            //instance.StartCoroutine(instance.SetTimeScale(0f));
            //instance.StartCoroutine(instance.SetTime(0));
            Time.timeScale = 0; // Set game speed to 0
        }
        else {
            // What to do when unpaused
            instance.lerping = true;
            instance.beginLerpTime = Time.time;
            //instance.StartCoroutine(instance.SetTimeScale(1f));
            //instance.StartCoroutine(instance.SetTime(1));
            Time.timeScale = 1; // Resume normal game speed
        }
    }

    private IEnumerator SetTime(int scale)
    {
        float duration = Time.time - instance.beginLerpTime;
        if (scale == 0)
        {
            if (duration <= 1)
            {
                Time.timeScale = Mathf.SmoothStep(1, 0, duration);
                yield return new WaitForSeconds(Time.deltaTime);
                instance.StartCoroutine(SetTime(scale));
                yield return 0;
            }
            else
            {
                Time.timeScale = 0;
                instance.lerping = false;
                yield return 0;
            }
        }
        else if (scale == 1)
        {
            if (duration <= 1)
            {
                Time.timeScale = Mathf.SmoothStep(0, 1, duration);
                yield return new WaitForSeconds(Time.deltaTime);
                instance.StartCoroutine(SetTime(scale));
                yield return 0;
            }
            else
            {
                Time.timeScale = 1;
                instance.lerping = false;
                yield return 0;
            }
        }
    }
}
