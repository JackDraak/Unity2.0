using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour
{
    [Header("Values to tweak Player facing angles:")]
    [Range(0f, 18f)][Tooltip("factor for lateral rotation skew")]           [SerializeField] float skewVertical = 9f;
    [Range(0f, 18f)][Tooltip("factor for vertical rotation skew")]          [SerializeField] float skewHorizontal = 9f;
    [Range(0f, 90f)][Tooltip("factor for roll skew, in degrees")]           [SerializeField] float skewRoll = 45f;
    [Range(0f, 20f)][Tooltip("factor for throw (axis) skew")]               [SerializeField] float skewThrow = 10f;
    [Range(0f, 30f)][Tooltip("factor for skew Lerping for pitch and yaw")]  [SerializeField] float skewLerp = 15f;
    [Range(0f, 10f)][Tooltip("factor for skew Lerping for roll")]           [SerializeField] float skewRollLerp = 5f;

    [Space(10)][Header("Player bounds:")]
    [Range(0f, 9.6f)]   [Tooltip("Range of drift, in m")]                   [SerializeField] float lateralRange = 4.8f;
    [Range(0f, 5.6f)]   [Tooltip("Range of drift, in m")]                   [SerializeField] float verticalMax = 2.8f;
    [Range(0f, 5.0f)]   [Tooltip("Range of drift, in m")]                   [SerializeField] float verticalMin = 2.5f;
    [Range(0f, 10.4f)]  [Tooltip("Speed, in ms^-1")]                        [SerializeField] float strafeSpeed = 5.2f;
    [Range(0f, 500.0f)] [Tooltip("Weapon cooldown time, in ms")]            [SerializeField] float weaponCooldownTime = 75f;
    [Range(1, 12)]      [Tooltip("Weapon volley, in particles/discharge")]  [SerializeField] int volley = 3;
    [Range(1, 40)]      [Tooltip("Weapon battery charge-rate, in p/s")]     [SerializeField] int chargeRate = 20;
    [Range(1, 1200)]    [Tooltip("Weapon battery capacity")]                [SerializeField] int capacity = 600;
    [Range(0, 16)]      [Tooltip("Weapon battery use-rate, in p/volley")]   [SerializeField] int useRate = 8;

    [Space(10)] [Header("Player weapon components:")]
    [SerializeField] AudioClip dischargeSound;
    [SerializeField] GameObject dischargeLight_0;
    [SerializeField] GameObject dischargeLight_1;
    [SerializeField] GameObject dischargeLight_2;
    [SerializeField] GameObject dischargeLight_3;
    [SerializeField] ParticleSystem weapon_0;
    [SerializeField] ParticleSystem weapon_1;
    [SerializeField] ParticleSystem weapon_2;
    [SerializeField] ParticleSystem weapon_3;
    [SerializeField] Slider slider;

    private bool            alive = true;
    private float           delta = 0;
    private float           battery = 0;
    private float           coolTime = 0;
    private int             lastWeaponFired = 0;
    private Vector2         controlAxis = Vector2.zero;
    private Vector3         priorRotation = Vector3.zero;
    private AudioSource     audioSource;

    private void Start()
    {
        audioSource = GameObject.FindGameObjectWithTag("PlayerAudioSource").GetComponent<AudioSource>();
        if (!audioSource) Debug.Log("ERROR no audioSource.");
        battery = capacity;
    }

    private void FixedUpdate()          { UpdatePlayerPosition(); }
    private void Update()               { UpdateWeaponState(); }
    private void TryPewPew()            { if (audioSource.isPlaying) return; audioSource.Play(); }
    private void UpdateWeaponSlider()   { slider.value = battery / capacity; }

    private void UpdatePlayerPosition()
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

    private void UpdateWeaponState()
    {
        ClearDischargeEffects();
        ChargeBattery();
        TryDischargeWeapon();
        UpdateWeaponSlider();
    }

    private void ChargeBattery()
    {
        if (battery < 0) battery = 0;
        battery += (chargeRate * Time.deltaTime);
        if (battery > capacity) battery = capacity;
    }

    private void TryDischargeWeapon()
    {
        if (!(CrossPlatformInputManager.GetButton("Fire1"))) return;

        if (Time.time > coolTime)
        {
            battery -= useRate;
            if (battery < 0) return;

            coolTime = Time.time + (weaponCooldownTime / 1000f);
            TryPewPew();
            switch (lastWeaponFired)
            {
                case 0:
                    dischargeLight_1.SetActive(true);
                    weapon_1.Emit(volley);
                    lastWeaponFired = 1;
                    break;
                case 1:
                    dischargeLight_2.SetActive(true);
                    weapon_2.Emit(volley);
                    lastWeaponFired = 2;
                    break;
                case 2:
                    dischargeLight_3.SetActive(true);
                    weapon_3.Emit(volley);
                    lastWeaponFired = 3;
                    break;
                case 3:
                    dischargeLight_0.SetActive(true);
                    weapon_0.Emit(volley);
                    lastWeaponFired = 0;
                    break;
                default:
                    break;
            }
        }
    }

    private void ClearDischargeEffects()
    {
        dischargeLight_0.SetActive(false);
        dischargeLight_1.SetActive(false);
        dischargeLight_2.SetActive(false);
        dischargeLight_3.SetActive(false);
    }
}
