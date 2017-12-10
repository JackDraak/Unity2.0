using UnityEngine;

public class AnalogClock : MonoBehaviour 
{
    [SerializeField] Transform hours;
    [SerializeField] Transform minutes;
    [SerializeField] Transform seconds;

    [SerializeField] AudioClip[] secondHandFX;

    private AudioSource audioSource;
    private float updateInterval = 1; // a "second", should be 1
    private float updateTime, realHours, realMinutes, realSeconds; 
    private System.DateTime startTime;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        updateTime = updateInterval;

        startTime = System.DateTime.Now;
        SetClock();
    }

    private void Update()
    {
        if (Time.time >= updateTime)
        {
            UpdateClock();
        }
    }

    private void SetClock()
    {
        seconds.Rotate(startTime.Second * Vector3.up * 6);
        minutes.Rotate(startTime.Minute * Vector3.up * 6);
        hours.Rotate(startTime.Hour * Vector3.up * 30);
    }

    private void UpdateClock()
    {
        updateTime += updateInterval;
        audioSource.PlayOneShot(secondHandFX[Mathf.FloorToInt(Random.Range(0, secondHandFX.Length))]);

        seconds.Rotate(Vector3.up * 6);
        minutes.Rotate(Vector3.up * .1f);
        hours.Rotate(Vector3.up * (.1f / 12));
    }
}
