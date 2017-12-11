using System;
using UnityEngine;
using URandom = UnityEngine.Random;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AnalogClock : MonoBehaviour 
{
    [SerializeField] AudioClip[] secondHandFX;
    [SerializeField] Text bottomLeft, bottomRight, topLeft, topRight;
    [SerializeField] Transform cam, hourHand, minuteHand, secondHand, stopwatchHand, sweepHand;

    private AudioSource audioSource;
    private bool mute = false, overlay = true, stopwatch = false;
    private DateTime startTime, stopTime;
    private float updateElapsedTimer, updateInterval = 1, updateTimer;
    private float rFac_12Hour = (0.1f / 12), rFac_Hour, rFac_Minute = 0.1f;
    private int notch = 30, semiNotch = 6;
    private TimeSpan breakTime, runTime;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rFac_Hour = notch * rFac_12Hour * 2;
        startTime = DateTime.Now;
        topRight.text = ("0ms :Stopwatch\n(space)");
        UpdateClock();
    }

    private void Update()
    {
        transform.LookAt(cam);
        ControlMode();
        ControlMute();
        ControlStopwatch();
        ControlVisability();
        if (Time.time >= updateTimer) UpdateClock();
        if (Time.time >= updateElapsedTimer) UpdateElapsedTime();
    }

    private void ControlMode()
    {
        if (Input.GetKeyDown(KeyCode.S)) LoadNextLevel();
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
            breakTime = DateTime.Now - stopTime;
            float ms = (float)breakTime.TotalMilliseconds;
            float stopwatchHandAngle = ((ms / 1000) % 60) * semiNotch;
            float sweepHandAngle = ((ms / 1000) % 360) * 360;
            stopwatchHand.localRotation = Quaternion.Euler(0f, stopwatchHandAngle, 0f);
            sweepHand.localRotation = Quaternion.Euler(0f, sweepHandAngle, 0f);
            topRight.text = (Mathf.RoundToInt(ms) + "ms :Stopwatch\n(space)");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            stopwatch = !stopwatch;
            if (stopwatch)
            {
                stopTime = DateTime.Now;
                //stopwatchHand.rotation = Quaternion.identity;
            }
        }
    }

    private void ControlVisability()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            Color invisible = new Color(0, 0, 0, 0);
            Color visible = new Color(250, 250, 250, 250);
            overlay = !overlay;
            if (!overlay)
            {
                topLeft.color = invisible;
                topRight.color = invisible;
                bottomLeft.color = invisible;
                bottomRight.color = invisible;
            }
            else
            {
                topLeft.color = visible;
                topRight.color = visible;
                bottomLeft.color = visible;
                bottomRight.color = visible;
            }
        }
    }

    private void LoadNextLevel()
    {
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextScene > SceneManager.sceneCountInBuildSettings -1) nextScene = 0;
        SceneManager.LoadScene(nextScene);
    }

    private void UpdateClock()
    {
        updateTimer += updateInterval;
        if (!mute) audioSource.PlayOneShot(secondHandFX[Mathf.FloorToInt(URandom.Range(0, secondHandFX.Length))]);
        DateTime time = DateTime.Now;
        bottomRight.text = time.ToLongTimeString();
        secondHand.localRotation = Quaternion.Euler(0f, time.Second * semiNotch, 0f);
        minuteHand.localRotation = Quaternion.Euler(0f, (time.Minute * semiNotch) + (time.Second * rFac_Minute), 0f);
        hourHand.localRotation = Quaternion.Euler(0f, (time.Hour * notch) + (time.Minute * rFac_Hour), 0f);
    }

    private void UpdateElapsedTime()
    {
        updateElapsedTimer += updateInterval / 10;
        runTime = DateTime.Now - startTime;
        topLeft.text = ("Begun: " + startTime.ToString() 
            + "\nElapsed: " + runTime.ToString() 
            + "\nS = toggle static/dynamic"
            + "\nV = toggle overlay");
    }
}
