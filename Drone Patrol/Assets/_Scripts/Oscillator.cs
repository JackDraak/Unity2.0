using UnityEngine;

public class Oscillator : MonoBehaviour {
    [Range(0f, 20f)][SerializeField] float period = 6f;
    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [SerializeField] bool mode = true;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (float.IsNaN(period)) return;
        const float Tau = Mathf.PI * 2f;
        float cycles = Time.time / period;
        float rawSinWave = Mathf.Sin(cycles * Tau);
        float movementFactor = rawSinWave / 2f + 0.5f;
        Vector3 offset = movementVector * movementFactor * rawSinWave;
        if (!mode) offset = movementVector * movementFactor;

        transform.position = startPos + offset;
    }
}
