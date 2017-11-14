using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [Header("Values to tweak Player facing angles:")]
    [Range(0, 18)][Tooltip("factor for lateral rotation skew")][SerializeField] float skewVertical = 9f;
    [Range(0, 18)][Tooltip("factor for vertical rotation skew")][SerializeField] float skewHorizontal = 9f;
    [Range(0, 60)][Tooltip("factor for roll skew")][SerializeField] float skewRoll = 30f;
    [Range(0, 20)][Tooltip("factor for throw (axis) skew")][SerializeField] float skewThrow = 10f;
    [Range(0, 30)][Tooltip("factor for skew Lerping for pitch and yaw")][SerializeField] float skewLerp = 15f;
    [Range(0, 10)][Tooltip("factor for skew Lerping for roll")][SerializeField] float skewRollLerp = 5f;

    [Space(10)]
    [Header("Player bounds:")]
    [Range(0, 9.6f)][Tooltip("Range of motion, in m")][SerializeField] float lateralRange = 4.8f;
    [Range(0, 5.6f)][Tooltip("Range of motion, in m")][SerializeField] float verticalMax = 2.8f;
    [Range(0, 5)][Tooltip("Range of motion, in m")][SerializeField] float verticalMin = 2.5f;
    [Range(0, 10.4f)][Tooltip("Speed, in ms^-1")][SerializeField] float strafeSpeed = 5.2f;
    [Range(0, 2000f)][Tooltip("Weapon cooldown time, in ms")][SerializeField] float weaponCooldownTime = 200f;

    [Space(10)]
    [Header("Player weapon systems:")]
    [SerializeField] ParticleSystem weapon_0;
    [SerializeField] ParticleSystem weapon_1;
    [SerializeField] ParticleSystem weapon_2;
    [SerializeField] ParticleSystem weapon_3;
    [SerializeField] AudioClip dischargeSound;

    private bool alive = true;
    private float delta = 0;
    private float coolTime = 0;
    private int lastWeaponFired = 0;
    private Vector2 controlAxis = Vector2.zero;
    private Vector3 priorRotation = Vector3.zero;
    private AudioSource audioSource;

    private void Start()            { audioSource = FindObjectOfType<AudioSource>(); coolTime = Time.time + (weaponCooldownTime / 1000f); }
    private void FixedUpdate()      { UpdatePlayerPosition(); }
    private void Update()           { if (CrossPlatformInputManager.GetButton("Fire1")) TryDischargeWeapon(); }
    private void TryPewPew()        { if (audioSource.isPlaying) return; audioSource.Play(); }


    void UpdatePlayerPosition()
    {
        if (!alive) return;
        PollAxis();

        delta = Time.deltaTime;
        SetLocalPosition();
        SetLocalAngles();
    }

    private void SetLocalPosition()
    {
        if ((float.IsNaN(controlAxis.x) && float.IsNaN(controlAxis.y)) || !alive) return;

        // set a desired position based on controlAxis input:
        float desiredXpos = transform.localPosition.x + controlAxis.x * strafeSpeed * delta;
        float desiredYpos = transform.localPosition.y + controlAxis.y * strafeSpeed * delta;

        // use bounds to restrain player to play area:
        float clampedXPos = Mathf.Clamp(desiredXpos, -lateralRange, lateralRange);
        float clampedYPos = Mathf.Clamp(desiredYpos, -verticalMin, verticalMax);

        // apply translation:
        transform.localPosition = new Vector3(clampedXPos, clampedYPos, transform.localPosition.z);
    }

    private void SetLocalAngles()
    {
        if ((float.IsNaN(skewVertical) && float.IsNaN(skewHorizontal)) || !alive) return;

        // set a desired pitch and yaw based on screen position (and controlAxis throw if applicable):
        Vector3 pos = transform.localPosition;
        float pitch = -pos.y * skewVertical - (controlAxis.y * skewThrow);
        float yaw = pos.x * skewHorizontal + (controlAxis.x * skewThrow);

        // set a desired roll when strafing left or right:
        float roll = controlAxis.x * -skewRoll;

        // Lerp between prior rotation and desired fixed rotation:
        pitch = Mathf.Lerp(priorRotation.x, pitch, delta * skewLerp);
        yaw = Mathf.Lerp(priorRotation.y, yaw, delta * skewLerp);
        roll = Mathf.Lerp(priorRotation.z, roll, delta * skewRollLerp);

        // apply rotation for this update:
        transform.localRotation = Quaternion.Euler(pitch, yaw, roll);

        // Store this updates' rotations for use next update. (X Y Z = Pitch Yaw Roll, respectively.)
        priorRotation.x = pitch;
        priorRotation.y = yaw;
        priorRotation.z = roll;
    }

    private void PollAxis()
    {
        if (!alive) return;

        controlAxis.x = CrossPlatformInputManager.GetAxis("Horizontal");
        controlAxis.y = CrossPlatformInputManager.GetAxis("Vertical");
    }

    private void TryDischargeWeapon()
    {
        if (Time.time > coolTime)
        {
            if (float.IsNaN(weaponCooldownTime)) weaponCooldownTime = 0.001f;
            coolTime = Time.time + (weaponCooldownTime / 1000f);
            TryPewPew();
            switch (lastWeaponFired)
            {
                case 0:
                    weapon_1.Emit(3);
                    coolTime = Time.time + (weaponCooldownTime / 1000f);
                    lastWeaponFired = 1;
                    break;
                case 1:
                    weapon_2.Emit(3);
                    coolTime = Time.time + (weaponCooldownTime / 1000f);
                    lastWeaponFired = 2;
                    break;
                case 2:
                    weapon_3.Emit(3);
                    coolTime = Time.time + (weaponCooldownTime / 1000f);
                    lastWeaponFired = 3;
                    break;
                case 3:
                    weapon_0.Emit(3);
                    coolTime = Time.time + (weaponCooldownTime / 1000f);
                    lastWeaponFired = 0;
                    break;
                default:
                    break;
            } 
        }
    }
}
