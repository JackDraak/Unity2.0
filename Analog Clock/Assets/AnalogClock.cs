using UnityEngine;
using UnityEngine.UI;

public class AnalogClock : MonoBehaviour 
{
    [SerializeField] Transform hours;
    [SerializeField] Transform minutes;
    [SerializeField] Transform seconds;
    [SerializeField] Text left, right;
    [SerializeField] AudioClip[] secondHandFX;

    private AudioSource audioSource;
    private bool stopwatch = false;
    private float intervalGUIUpdate, updateGUITime, stopTime; 
    private float minuteRotation = 0.1f;
    private float twelveHourRotation = (0.1f / 12);
    private float updateInterval = 1;
    private float updateTime;
    private int hourRotation = 30;
    private int secondRotation = 6;
    private System.DateTime startTime;
    private Vector3 clockwise = new Vector3(0, 1, 0);

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        intervalGUIUpdate = updateInterval / 10;
        startTime = System.DateTime.Now;
        updateTime = updateInterval;
        SetClock();
    }

    private void Update()
    {
        if (Time.time >= updateTime) UpdateClock();
        if (Time.time >= updateGUITime) UpdateGUI();

        if (stopwatch) stopTime += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            stopwatch = !stopwatch;
            if (stopwatch) stopTime = 0;
        }
    }

    private void SetClock()
    {
        hours.Rotate(clockwise * startTime.Hour * hourRotation);
        hours.Rotate(clockwise * startTime.Minute * hourRotation * twelveHourRotation * 2); // TODO: make this cleaner
        minutes.Rotate(clockwise * startTime.Minute * secondRotation);
        minutes.Rotate(clockwise * startTime.Second * minuteRotation);
        seconds.Rotate(clockwise * startTime.Second * secondRotation);
    }

    private void UpdateGUI()
    {
        updateGUITime += intervalGUIUpdate;
        left.text = ("Begun: " + startTime.ToString() + "\nElapsed: " + Time.time.ToString());
        right.text = ((Time.time % 60).ToString() + " :Laptime\n" + stopTime.ToString() + " :Stopwatch");
    }

    private void UpdateClock()
    {
        updateTime += updateInterval;
        audioSource.PlayOneShot(secondHandFX[Mathf.FloorToInt(Random.Range(0, secondHandFX.Length))]);
        seconds.Rotate(clockwise * secondRotation);
        minutes.Rotate(clockwise * minuteRotation);
        hours.Rotate(clockwise * twelveHourRotation);
    }
}
