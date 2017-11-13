using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour {
    [Header("Values to tweak Player facing angles:")]
    [Range(0, 14)][Tooltip("factor for lateral rotation skew")][SerializeField] float skewVertical = 7f;
    [Range(0, 12)][Tooltip("factor for vertical rotation skew")][SerializeField] float skewHorizontal = 6f;
    [Range(0, 60)][Tooltip("factor for roll skew")][SerializeField] float skewRoll = 30f;
    [Range(0, 20)][Tooltip("factor for throw (axis) skew")][SerializeField] float skewThrow = 10f;
    [Range(0, 30)][Tooltip("factor for skew Lerping for pitch and yaw")][SerializeField] float skewLerp = 15f;
    [Range(0, 10)][Tooltip("factor for skew Lerping for roll")][SerializeField] float skewRollLerp = 5f;
    [Space(10)]
    [Header("Player bounds:")]
    [Range(0, 9.6f)][Tooltip("Range of motion, in m")][SerializeField] float lateralRange = 4.8f;
    [Range(0, 5.6f)][Tooltip("Range of motion, in m")][SerializeField] float verticalMax = 2.8f;
    [Range(0, 5)][Tooltip("Range of motion, in m")][SerializeField] float verticalMin = 2.5f;
    [Range(0, 13.2f)][Tooltip("Speed, in ms^-1")][SerializeField] float strafeSpeed = 6.6f;

    private Rigidbody rigidbody;
    private bool alive = true;
    private bool shoot = false;
    private bool pause = false;
    private float delta;
    private Vector2 controlAxis;
    private Vector3 priorRotation;

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
        SetLocalAngles();
    }

    private void SetLocalPosition()
    {
        if ((float.IsNaN(controlAxis.x) && float.IsNaN(controlAxis.y)) || !alive) return;
        float xOffset = controlAxis.x * strafeSpeed * delta;
        float yOffset = controlAxis.y * strafeSpeed * delta;
        float desiredXpos = transform.localPosition.x + xOffset;
        float desiredYpos = transform.localPosition.y + yOffset;
        float clampedXPos = Mathf.Clamp(desiredXpos, -lateralRange, lateralRange);
        float clampedYPos = Mathf.Clamp(desiredYpos, -verticalMin, verticalMax);
        transform.localPosition = new Vector3(clampedXPos, clampedYPos, transform.localPosition.z);
    }

    private void SetLocalAngles()
    {
        if ((float.IsNaN(skewVertical) && float.IsNaN(skewHorizontal)) || !alive) return;

        // set a fixed pitch and yaw based on screen position (and controlAxis throw if applicable):
        Vector3 pos = transform.localPosition;
        float pitch = -pos.y * skewVertical - (controlAxis.y * skewThrow);
        float yaw = pos.x * skewHorizontal + (controlAxis.x * skewThrow);

        // roll left or right when strafing left or right
        float roll;
        float gap = Mathf.Abs(controlAxis.x - controlAxis.y);
        if (controlAxis.x < 0) roll = gap * skewRoll;
        else if (controlAxis.x > 0) roll = -gap * skewRoll;
        else roll = 0;

        // Lerp between prior rotation and desired fixed rotation:
        pitch = Mathf.Lerp(priorRotation.x, pitch, delta * skewLerp);
        yaw = Mathf.Lerp(priorRotation.y, yaw, delta * skewLerp);
        roll = Mathf.Lerp(priorRotation.z, roll, delta * skewRollLerp);

        // Set rotation to an intermediary this update:
        transform.localRotation = Quaternion.Euler(pitch, yaw, roll);

        // X Y Z = Pitch Yaw Roll, respectively.
        priorRotation.x = pitch;
        priorRotation.y = yaw;
        priorRotation.z = roll;
    }

    private void PollControls()
    {
        if (!alive) return;
        controlAxis.x = CrossPlatformInputManager.GetAxis("Horizontal");
        controlAxis.y = CrossPlatformInputManager.GetAxis("Vertical");
        shoot = CrossPlatformInputManager.GetButton("Fire1");
    }
}
