using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour {
    [Header("Values to tweak right now:")]
    [Range(-10f, 10f)][Tooltip("factor for rotation skew")][SerializeField] float rFactorX = 0f;
    [Range(-10f, 10f)] [Tooltip("factor for rotation skew")] [SerializeField] float rFactorY = 0f;
    [Space(10)]
    [Header("Generally, leave these alone, they've been dialed-in:")]
    [Tooltip("Range of motion, in m")] [SerializeField] float xRange = 3.7f;
    [Tooltip("Range of motion, in m")][SerializeField] float yRange = 1.9f;
    [Tooltip("Speed, in ms^-1")][SerializeField] float speed = 4f;

    private Rigidbody rigidbody;
    private bool alive = true;
    private bool shoot = false;
    private bool pause = false;
    private float delta;
    private float inputX; 
    private float inputY;

    void Start()
    {
        if (!(rigidbody = GetComponent<Rigidbody>()))
            Debug.Log(this + " Start() failed to attached Player rigidbody!");
    }

    void FixedUpdate()
    {
        UpdatePlayerPosition();
    }

    void UpdatePlayerPosition()
    {
        if (!alive) return;
        delta = Time.deltaTime;
        PollControls();
        SetLocalPosition();
        // SetLocalAngles();
    }

    private void SetLocalPosition()
    {
        if ((float.IsNaN(inputX) && float.IsNaN(inputY)) || !alive) return;
        // special y-offset gives ship more space at the top of the frame for "balance".
        float overhead = 0.5f;

        float xOffset = inputX * speed * delta;
        float yOffset = inputY * speed * delta;
        float desiredXpos = transform.localPosition.x + xOffset;
        float desiredYpos = transform.localPosition.y + yOffset;
        float clampedXPos = Mathf.Clamp(desiredXpos, -xRange, xRange);
        float clampedYPos = Mathf.Clamp(desiredYpos, -yRange, yRange + overhead);

        transform.localPosition = new Vector3(clampedXPos, clampedYPos, transform.localPosition.z);
    }

    private void SetLocalAngles()
    {
        if ((float.IsNaN(rFactorX) && float.IsNaN(rFactorY)) || !alive) return;
        // X Y Z = Pitch Yaw Roll, respectively.
        float xPos = transform.localPosition.x;
        float yPos = transform.localPosition.y;
        float roll = 0; // transform.localRotation.eulerAngles.z;
        float pitch = xPos * rFactorX * delta;
        float yaw = yPos * rFactorY * delta;

        transform.Rotate(new Vector3(pitch, yaw, roll));
    }

    private void PollControls()
    {
        if (!alive) return;
        inputX = CrossPlatformInputManager.GetAxis("Horizontal");
        inputY = CrossPlatformInputManager.GetAxis("Vertical");
        shoot = CrossPlatformInputManager.GetButton("Fire1");
    }
}
