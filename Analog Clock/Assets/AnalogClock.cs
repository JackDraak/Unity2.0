using UnityEngine;
using UnityEngine.UI;
using System;
using URandom = UnityEngine.Random;

public class AnalogClock : MonoBehaviour 
{
    [SerializeField] AudioClip[] secondHandFX;
    [SerializeField] Text bottomLeft, bottomRight, topLeft, topRight;
    [SerializeField] Transform hourHand;
    [SerializeField] Transform minuteHand;
    [SerializeField] Transform secondHand;
    [SerializeField] Transform stopwatchHand;

    private AudioSource audioSource;
    private bool mute = false, stopwatch = false;
    private float intervalGUIUpdate, stopTime, updateGUITime;
    private float hourMinuteRotation;
    private float minuteRotation = 0.1f;
    private float twelveHourRotation = (0.1f / 12);
    private float updateInterval = 1;
    private float updateTime;
    private int hourRotation = 30;
    private int secondRotation = 6;
    private DateTime currentTime, startTime;
    private Vector3 clockwise = new Vector3(0, 1, 0);

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        hourMinuteRotation = hourRotation * twelveHourRotation * 2;
        intervalGUIUpdate = updateInterval / 10;
        startTime = DateTime.Now;
        updateTime = updateInterval;
        InitClock();
    }

    private void Update()
    {
        ControlMute();
        ControlStopwatch();
        if (Time.time >= updateTime) UpdateClock();
        if (Time.time >= updateGUITime) UpdateGUI();
    }

    private void ControlMute()
    {
        if (Input.GetKeyDown(KeyCode.M)) mute = !mute;
        if (mute) bottomLeft.text = "M = restore audio"; else bottomLeft.text = "M = mute audio";
    }

    private void ControlStopwatch()
    {
        if (stopwatch)
        {
            stopTime += Time.deltaTime;
            float absoluteAngle = (stopTime % 60) * secondRotation;
            stopwatchHand.rotation = Quaternion.Euler(0f, absoluteAngle, 0f);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            stopwatch = !stopwatch;
            if (stopwatch)
            {
                stopTime = 0;
                stopwatchHand.rotation = Quaternion.identity;
            }
        }
    }

    private void InitClock()
    {
        hourHand.Rotate(clockwise * startTime.Hour * hourRotation);
        hourHand.Rotate(clockwise * startTime.Minute * hourMinuteRotation);

        minuteHand.Rotate(clockwise * startTime.Minute * secondRotation);
        minuteHand.Rotate(clockwise * startTime.Second * minuteRotation);

        secondHand.Rotate(clockwise * startTime.Second * secondRotation);
    }

    private void UpdateClock()
    {
        updateTime += updateInterval;

        if (!mute) audioSource.PlayOneShot(secondHandFX[Mathf.FloorToInt(URandom.Range(0, secondHandFX.Length))]);

        secondHand.Rotate(clockwise * secondRotation);
        minuteHand.Rotate(clockwise * minuteRotation);
        hourHand.Rotate(clockwise * twelveHourRotation);

        currentTime = DateTime.Now;
        bottomRight.text = currentTime.ToLongTimeString();
    }

    private void UpdateGUI()
    {
        updateGUITime += intervalGUIUpdate;

        topLeft.text = ("Begun: " + startTime.ToString() + "\nElapsed: " + Time.time.ToString());
        topRight.text = (stopTime.ToString() + " :Stopwatch (space)\n" + (Time.time % 60).ToString() + " :Laptime");
    }
}
