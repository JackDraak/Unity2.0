using System;
using UnityEngine;
using URandom = UnityEngine.Random;
using UnityEngine.UI;

public class AnalogClock : MonoBehaviour 
{
    [SerializeField] AudioClip[] secondHandFX;
    [SerializeField] bool legacyMode; // controls: y-axis hands (legacy) vs. z-axis hands
    [SerializeField] Text bottomLeft, bottomRight, topLeft, topRight;
    [SerializeField] Transform sceneCamera, hourHand, minuteHand, secondHand, stopwatchHand, sweepHand, sweepLampTransform;

    private AudioHandler audioHandler;
    private AudioSource audioSource;
    private bool sweepLamp = true, stopwatch = false;
    private DateTime startTime, stopTime;
    private float lookDelay = 5, updateElapsedTimer = 0, updateInterval = 1, updateTimer = 0;
    private float rFac_12Hour = (0.1f / 12), rFac_Hour, rFac_Minute = 0.1f;
    private int notch = 30, semiNotch = 6;
    private KeyCode endOfLine, guiUp, guiDown, switchTheme, toggleClick, toggleLamp, toggleOverlay, toggleStopwatch;
    private KeyValet keyValet;
    private LevelValet levelValet;
    private string currentTime, elapsedTime;
    private TimeSpan breakTime, runTime;

    private void OnEnable()
    {
        startTime = DateTime.Now;
        ClockInit();
        InitGUI();
        UpdateClock();
    }

    private void Update()
    {
        ControlState();
        iTween.LookUpdate(gameObject, sceneCamera.position, lookDelay);
        ControlMute();
        ControlLamp();
        ControlStopwatch();
        ControlVisability();
        UpdateClock();
        UpdateElapsedTime();
        UpdateGUI();
    }

    private void ClockInit()
    {
        audioSource = GetComponent<AudioSource>();
        audioHandler = FindObjectOfType<AudioHandler>();
        levelValet = FindObjectOfType<LevelValet>();
        keyValet = FindObjectOfType<KeyValet>();

        endOfLine = keyValet.GetKey("Clock-Quit");
        guiUp = keyValet.GetKey("GUI-Larger");
        guiDown = keyValet.GetKey("GUI-Smaller");
        toggleClick = keyValet.GetKey("Clock-ToggleClicks");
        toggleLamp = keyValet.GetKey("Clock-ToggleLamp");
        toggleOverlay = keyValet.GetKey("Clock-ToggleOverlay");
        toggleStopwatch = keyValet.GetKey("Clock-ToggleStopwatch");
        switchTheme = keyValet.GetKey("Clock-SwitchTheme");

        rFac_Hour = notch * rFac_12Hour * 2;
    }

    private void ControlFontsize()
    {
        int fontsize = levelValet.GetFontsize();
        topLeft.fontSize = fontsize;
        topRight.fontSize = fontsize;
        bottomLeft.fontSize = fontsize;
        bottomRight.fontSize = fontsize;
    }

    private void ControlLamp()
    {
        if (Input.GetKeyDown(toggleLamp))
        {
            sweepLamp = !sweepLamp;
            if (sweepLampTransform != null) sweepLampTransform.gameObject.SetActive(sweepLamp);
        }
    }

    private void ControlMute()
    {
        if (Input.GetKeyDown(toggleClick)) audioHandler.ToggleFX();
    }

    private void ControlOverlay()
    {
        Color visible = new Color(250, 250, 250, 250);
        Color invisible = new Color(0, 0, 0, 0);
        if (!levelValet.GetOverlay())
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

    private void ControlState()
    {
        if (Input.GetKeyDown(switchTheme)) levelValet.LoadNextLevel();
        if (Input.GetKeyDown(endOfLine)) Application.Quit();
    }

    private void ControlStopwatch()
    {
        if (stopwatch)
        {
            breakTime = DateTime.Now - stopTime;
            float ms = (float)breakTime.TotalMilliseconds;
            float stopwatchHandAngle = ((ms / 1000) % 60) * semiNotch;
            float sweepHandAngle = ((ms / 1000) % 360) * 360;

            if (!legacyMode)
            {
                stopwatchHand.localRotation = Quaternion.Euler(0f, 0f, stopwatchHandAngle);
                sweepHand.localRotation = Quaternion.Euler(0f, 0f, sweepHandAngle);
            }
            else
            {
                stopwatchHand.localRotation = Quaternion.Euler(0f, stopwatchHandAngle, 0f);
                sweepHand.localRotation = Quaternion.Euler(0f, sweepHandAngle, 0f);
            }
        }

        if (Input.GetKeyDown(toggleStopwatch))
        {
            stopwatch = !stopwatch;
            if (stopwatch) stopTime = DateTime.Now;
        }
    }

    private void ControlVisability()
    {
        if (Input.GetKeyDown(toggleOverlay))
        {
            levelValet.ToggleOverlay();
            ControlOverlay();
        }
    }

    private void InitGUI()
    {
        ControlOverlay();
        ControlFontsize();
    }

    private void UpdateClock()
    {
        if (Time.time >= updateTimer)
        {
            updateTimer = Time.time + updateInterval;
            DateTime time = DateTime.Now;
            currentTime = time.ToLongTimeString();
            if (!audioHandler) ClockInit();
            if (audioHandler.GetFX())
                audioSource.PlayOneShot(secondHandFX[Mathf.FloorToInt(URandom.Range(0, secondHandFX.Length))]);

            if (!legacyMode)
            {
                secondHand.localRotation = Quaternion.Euler(0f, 0f, time.Second * semiNotch);
                minuteHand.localRotation = Quaternion.Euler(0f, 0f, (time.Minute * semiNotch) + (time.Second * rFac_Minute));
                hourHand.localRotation = Quaternion.Euler(0f, 0f, (time.Hour * notch) + (time.Minute * rFac_Hour));
            }
            else
            {
                secondHand.localRotation = Quaternion.Euler(0f, time.Second * semiNotch, 0f);
                minuteHand.localRotation = Quaternion.Euler(0f, (time.Minute * semiNotch) + (time.Second * rFac_Minute), 0f);
                hourHand.localRotation = Quaternion.Euler(0f,(time.Hour * notch) + (time.Minute * rFac_Hour), 0f);
            }
        }
    }

    private void UpdateElapsedTime()
    {
        if (Time.time >= updateElapsedTimer)
        {
            updateElapsedTimer += updateInterval / 10;
            runTime = DateTime.Now - startTime;
            elapsedTime = "Begun: " + startTime.ToString() + "\nElapsed: " + runTime.ToString();
        }
    }

    private void UpdateGUI()
    {
        if (Input.GetKeyDown(guiUp))
        {
            int fontsize = levelValet.GetFontsize();
            if (fontsize < 27)
            {
                fontsize++;
                topLeft.fontSize = fontsize;
                topRight.fontSize = fontsize;
                bottomLeft.fontSize = fontsize;
                bottomRight.fontSize = fontsize;
                levelValet.SetFontsize(fontsize);
            }
        }

        if (Input.GetKeyDown(guiDown))
        {
            int fontsize = levelValet.GetFontsize();
            if (fontsize > 6)
            {
                fontsize--;
                topLeft.fontSize = fontsize;
                topRight.fontSize = fontsize;
                bottomLeft.fontSize = fontsize;
                bottomRight.fontSize = fontsize;
                levelValet.SetFontsize(fontsize);
            }
        }

        topLeft.text = elapsedTime;

        topRight.text = (Mathf.RoundToInt((float)breakTime.TotalMilliseconds) + "ms :Stopwatch\n(space)");

        bottomRight.text = currentTime;

        bottomLeft.text = "Q = quit";

        if (!audioHandler.GetFX()) bottomLeft.text += "\nC = restore clicks";
        else bottomLeft.text += "\nC = mute clicks";

        bottomLeft.text += "\nV = toggle overlay";

        if (sweepLamp) bottomLeft.text += "\nL = deactivate sweep lamp";
        else bottomLeft.text += "\nL = activate sweep lamp";

        bottomLeft.text += "\nS = switch theme (resets stopwatch)";
    }
}
