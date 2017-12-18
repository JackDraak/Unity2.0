using System;
using UnityEngine;
using URandom = UnityEngine.Random;
using UnityEngine.UI;

public class BlenderClock : MonoBehaviour
{
    [SerializeField] AudioClip[] secondHandFX;
    [SerializeField] Text bottomLeft, bottomRight, topLeft, topRight;
    [SerializeField] Transform sceneCamera, hourHand, minuteHand, secondHand, stopwatchHand, sweepHand, sweepLampTransform;

    private AudioHandler audioHandler;
    private AudioSource audioSource;
    private bool sweepLamp = true, stopwatch = false;
    private DateTime startTime, stopTime;
    private float lookUpdate = 0, lookDelay = 5, updateElapsedTimer, updateInterval = 1, updateTimer;
    private float rFac_12Hour = (0.1f / 12), rFac_Hour, rFac_Minute = 0.1f;
    private int notch = 30, semiNotch = 6;
    private KeyCode endOfLine, guiUp, guiDown, switchTheme, toggleClick, toggleLamp, toggleOverlay, toggleStopwatch;
    private KeyValet keyValet;
    private LevelValet levelValet;
    private string currentTime, elapsedTime;
    private TimeSpan breakTime, runTime;

    private void Start()
    {
    }

    private void OnEnable()
    {
        startTime = DateTime.Now;
        ClockInit();
        InitGUI();
        UpdateClock();
    }

    private void Update()
    {
        iTween.LookUpdate(gameObject, sceneCamera.position, lookDelay);

        ControlMode();
        ControlMute();
        ControlLamp();
        ControlStopwatch();
        ControlVisability();

        if (Time.time >= updateTimer) UpdateClock();
        if (Time.time >= updateElapsedTimer) UpdateElapsedTime();
        UpdateGUI();
    }

    private void ClockInit()
    {
        audioSource = GetComponent<AudioSource>();
        if (!(audioHandler = FindObjectOfType<AudioHandler>())) Debug.Log("AnalogClock.cs: audioHandler INFO, FAIL.");
        if (!(levelValet = FindObjectOfType<LevelValet>())) Debug.Log("AnalogClock.cs: levelValet INFO, FAIL.");
        if (!(keyValet = FindObjectOfType<KeyValet>())) Debug.Log("AnalogClock.cs keyValet INFO, FAIL.");

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

    private void ControlLamp()
    {
        if (Input.GetKeyDown(toggleLamp))
        {
            sweepLamp = !sweepLamp;
            if (sweepLampTransform != null) sweepLampTransform.gameObject.SetActive(sweepLamp);
        }
    }

    private void ControlMode()
    {
        if (Input.GetKeyDown(switchTheme)) levelValet.LoadNextLevel();
        if (Input.GetKeyDown(endOfLine)) Application.Quit();
    }

    private void ControlMute()
    {
        if (Input.GetKeyDown(toggleClick)) audioHandler.ToggleFX();
    }

    private void ControlStopwatch()
    {
        if (stopwatch)
        {
            breakTime = DateTime.Now - stopTime;
            float ms = (float)breakTime.TotalMilliseconds;
            float stopwatchHandAngle = ((ms / 1000) % 60) * semiNotch;
            float sweepHandAngle = ((ms / 1000) % 360) * 360;
            stopwatchHand.localRotation = Quaternion.Euler(0f, 0f, stopwatchHandAngle);
            sweepHand.localRotation = Quaternion.Euler(0f, 0f, sweepHandAngle);
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
            Color invisible = new Color(0, 0, 0, 0);
            Color visible = new Color(250, 250, 250, 250);
            levelValet.ToggleOverlay();
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
    }

    private void InitGUI()
    {
        int fontsize = levelValet.GetFontsize();
        topLeft.fontSize = fontsize;
        topRight.fontSize = fontsize;
        bottomLeft.fontSize = fontsize;
        bottomRight.fontSize = fontsize;
    }

    private void UpdateClock()
    {
        if (!audioHandler) ClockInit();
        updateTimer = Time.time + updateInterval;
        if (audioHandler.GetFX())
            audioSource.PlayOneShot(secondHandFX[Mathf.FloorToInt(URandom.Range(0, secondHandFX.Length))]);
        DateTime time = DateTime.Now;
        currentTime = time.ToLongTimeString();

        secondHand.localRotation = Quaternion.Euler(0f, 0f, time.Second * semiNotch);
        minuteHand.localRotation = Quaternion.Euler(0f, 0f, (time.Minute * semiNotch) + (time.Second * rFac_Minute));
        hourHand.localRotation = Quaternion.Euler(0f, 0f, (time.Hour * notch) + (time.Minute * rFac_Hour));
    }

    private void UpdateElapsedTime()
    {
        updateElapsedTimer += updateInterval / 10;
        runTime = DateTime.Now - startTime;
        elapsedTime = "Begun: " + startTime.ToString() + "\nElapsed: " + runTime.ToString();
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
