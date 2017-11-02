using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CountDown : MonoBehaviour {
    [SerializeField] Text countDown;
    const string scene_01 = "DronePatrol_01";
    const int limit = 20;
    float startTime;
    float seconds;

    void Start () { startTime = Time.time; }
	
	void Update ()
    {
        bool key = Input.anyKeyDown;
        if (key) SceneManager.LoadScene(scene_01);
        seconds = limit - Mathf.Round(Time.time - startTime);
        countDown.text = seconds.ToString();
    }
}
