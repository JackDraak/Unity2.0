using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CountDown : MonoBehaviour {
    [SerializeField] Text countDown;
    float startTime;
    int limit = 20;
    string scene_01 = "DronePatrol_01";
    const float Tau = Mathf.PI * 2;

    // Use this for initialization
    void Start ()
    {
        startTime = Time.time;
        Debug.Log(Mathf.Sin(Tau / 4f));
	}
	
	// Update is called once per frame
	void Update ()
    {
        bool key = Input.anyKeyDown;
        if (key) SceneManager.LoadScene(scene_01);
        float seconds;
        seconds = limit - Mathf.Round(Time.time - startTime);
        countDown.text = seconds.ToString();
    }
}
