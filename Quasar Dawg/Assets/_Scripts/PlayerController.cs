using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour {
    [Header("Values to tweak Player facing angles:")]
    [Range(-12f, 12f)][Tooltip("factor for lateral rotation skew")][SerializeField] float rFactorX = 10f;
    [Range(-12f, 12f)] [Tooltip("factor for vertical rotation skew")] [SerializeField] float rFactorY = 7f;
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
        SetLocalAngles();
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
        Vector3 pos = transform.localPosition;
        float pitch = -pos.y * rFactorX;
        float yaw = pos.x * rFactorY;
        float roll = 0; // TODO: roll around corners? roll when maneuvering?
        // X Y Z = Pitch Yaw Roll, respectively.
        transform.localRotation = Quaternion.Euler(pitch, yaw, roll);
    }

    private void PollControls()
    {
        if (!alive) return;
        inputX = CrossPlatformInputManager.GetAxis("Horizontal");
        inputY = CrossPlatformInputManager.GetAxis("Vertical");
        shoot = CrossPlatformInputManager.GetButton("Fire1");
    }
}
