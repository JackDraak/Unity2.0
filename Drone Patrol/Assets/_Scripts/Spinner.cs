using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour {
    [Range(-1, 1)] [SerializeField] int direction = 1;
    [Range(0f, 60f)] [SerializeField] float period = 50f;
    [SerializeField] bool mode = true; 

    void Update()
    {
        if (float.IsNaN(period) || direction == 0) return;

        const float Tau = Mathf.PI * 2f;
        float cycles = Time.time / period;
        float rawSinWave = Mathf.Sin(cycles * Tau);
        float movementFactor = cycles / 2f + 0.5f;
        float offset = movementFactor * rawSinWave;
        if (!mode) offset = movementFactor;

        transform.Rotate((-direction) * Vector3.forward * offset);
    }
}
