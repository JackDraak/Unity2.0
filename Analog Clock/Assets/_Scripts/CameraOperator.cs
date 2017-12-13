using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraOperator : MonoBehaviour 
{
    [SerializeField] Transform[] actors;

    private bool onPath = false, onLook = false;
    private float switchTime = 0, pathTime = 1, lookTime = 1; 
    private int selection, switchCount = 0, targetDelay = 21;

    private void LateUpdate()
    {
        float now = Time.time;
        if (now > pathTime) DoPath();
        if (now > lookTime) DoLook();
    }

    private void DoLook()
    {
        int duration = targetDelay;
        lookTime = Time.time + duration + 1;

        bool bad = true;
        while (bad)
        {
            selection = Mathf.FloorToInt(Random.Range(0, actors.Length));
            if (actors[selection].position != null) bad = false;
        }
        iTween.LookUpdate(gameObject, actors[selection].position, duration);
    }

    private void DoPath()
    {
        int duration = 300;
        pathTime = Time.time + duration + 1;

        iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath("cam"), "time", duration, "easetype", iTween.EaseType.easeInOutSine));
    }
}
