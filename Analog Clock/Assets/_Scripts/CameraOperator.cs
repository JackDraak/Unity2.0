using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraOperator : MonoBehaviour 
{
    [SerializeField] Transform[] actors;

    private bool onPath = false, onLook = false;
    private float pathTime = 1, lookTime = 1; 
    private int selection, lookDelay = 21, panDelay = 200;

    private void LateUpdate()
    {
        float now = Time.time;
        if (now > pathTime) DoPath();
        if (now > lookTime) DoLook();
        iTween.LookUpdate(gameObject, actors[selection].position, lookDelay);
    }

    private void DoLook()
    {
        int duration = lookDelay;
        lookTime = Time.time + duration + 1;

        bool bad = true;
        while (bad)
        {
            selection = Mathf.FloorToInt(Random.Range(0, actors.Length));
            if (actors[selection].position != null) bad = false;
        }
    }

    private void DoPath()
    {
        int duration = panDelay;
        pathTime = Time.time + duration + 1;

        iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath("cam"), "time", duration, "easetype", iTween.EaseType.easeInOutSine));
    }
}
