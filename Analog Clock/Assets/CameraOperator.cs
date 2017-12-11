using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraOperator : MonoBehaviour 
{
    [SerializeField] Transform[] actors;

    private float switchTime = 0;
    private int selection, switchCount = 0, switchDelay = 21;

    private void LateUpdate()
    {
        if (Time.time > switchTime)
        {
            selection = Mathf.FloorToInt(Random.Range(0, actors.Length));
            switchTime = Time.time + switchDelay;
        }

        if (actors[selection].position != null)
        {
            iTween.LookUpdate(gameObject, actors[selection].position, 41);
            // transform.LookAt(actors[selection].position);
        }
    }

    private void Start()
    {
        DoPath();
    }

    private void DoPath()
    {
        int duration = 300;
        Invoke("DoPath", duration + 1);
        iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath("cam"), "time", duration, "easetype", iTween.EaseType.easeInOutSine));
    }
}
