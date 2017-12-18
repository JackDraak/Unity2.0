using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLightControl : MonoBehaviour
{
    private Light directionalLight;
    private KeyCode brighter, darker;
    private KeyValet keyValet;
    private float brightness;
    private float duration = 60;

    private void Start()
    {
        if (!(keyValet = FindObjectOfType<KeyValet>())) Debug.Log("AnalogClock.cs keyValet INFO, FAIL.");
        brighter = keyValet.GetKey("Light-Brighten");
        darker = keyValet.GetKey("Light-Darken");
        directionalLight = GetComponent<Light>();
    }

    private void Update()
    {
     if (Input.GetKeyDown(brighter))
        {
            brightness = (float) (brightness + .5);
            if (brightness > 5) brightness = 5;
        }

        if (Input.GetKeyDown(darker))
        {
            brightness = (float) (brightness - .5);
            if (brightness < -2) brightness = -2;
        } 

        float phi = Time.time / duration * 2 * Mathf.PI;
        float amplitude = brightness + Mathf.Cos(phi) * 0.5F + 0.5F;
        directionalLight.intensity = (amplitude);
    }
}
