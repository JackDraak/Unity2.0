﻿using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;

/* Development notes: Lots of place-holder assets, many that I created myself, others from the asset
 * store or elsewhere (i.e. from the Udemy course).  While I publish this source on GitHub and am glad
 * if people find utility in it, please be careful to not grab/use something here in a way you may not
 * be allowed to. (In general, anything here can be used freely for personal use, but again, please be
 * informed before you go rummaging for treasure.) Cheers. -Jack D.
 * 
 * TODO: find or generate some structures to help bring the level(s) to life
 * TODO: fix all collisions (player/terrain, player/enemy, player/structure)
 * 
 * TODO: enemy weapon that freeezes recharge?
 * TODO: have spawn-rate increase over time
 * TODO: use camera-speed hooks (DONE) to have a boss fight? (WIP)
 * TODO: play with time more?
 * TODO: Prevent spawning while player resetting? 
 *      
 * TODO: Improve ship damage/reset effects/sounds
 * TODO: Make enemies more interesting / animated
 *       
 * TODO: Have other collectible boosters or downers.... variety of enemy weapons....
 * 
 *     ... i.e. handles in-place to boost or retard discharge-rate, volley-size, battery capacity, 
 *     etc., player maneuverability factors aplenty....
 */

public class PlayerController : MonoBehaviour
{
    // DEVNOTE: These debugging commands work in the editor or on "development" builds. 
    // Assign them to keys not in-use [in the KeyValet.cs class]:
    private bool invulnerableCommand;       private KeyCode invulnerableKey;
    private bool maxEnergyCommand;          private KeyCode maxEnergyKey;
    private bool rechargeShieldCommand;     private KeyCode shieldKey;
    private bool rechargeWeaponCommand;     private KeyCode weaponKey;

    #region So many things to set in the inspector....
    [Header("Values to tweak Player facing angles:")]
    [Range(0f, 18f)][Tooltip("factor for vertical rotation skew")]          [SerializeField] float skewHorizontal = 9f;
    [Range(0f, 90f)][Tooltip("factor for roll skew, in degrees")]           [SerializeField] float skewRoll = 45f;
    [Range(0f, 20f)][Tooltip("factor for throw (axis) skew")]               [SerializeField] float skewThrow = 10f;
    [Range(0f, 18f)][Tooltip("factor for lateral rotation skew")]           [SerializeField] float skewVertical = 9f;

    [Space(6)]
    [Range(0f, 30f)][Tooltip("factor for skew Lerping for pitch and yaw")]  [SerializeField] float skewLerp = 15f;
    [Range(0f, 10f)][Tooltip("factor for skew Lerping for roll")]           [SerializeField] float skewRollLerp = 5f;

    [Space(10)]
    [Header("Player bounds:")]
    [Range(0f, 9.6f)]   [Tooltip("Range of drift, in m")]                   [SerializeField] float lateralRange = 4.8f;
    [Range(0f, 5.6f)]   [Tooltip("Range of drift, in m")]                   [SerializeField] float verticalMax = 2.8f;
    [Range(0f, 5.0f)]   [Tooltip("Range of drift, in m")]                   [SerializeField] float verticalMin = 2.5f;

    [Space(8)]
    [Header("Player defaults:")]
    [SerializeField] float basePlayerDamage = 1;
    [SerializeField] float basePlayerForwardSpeed = 0.85f;
    [SerializeField] float basePlayerRollDelay = 2f;
    [SerializeField] float basePlayerShieldCapacity = 200;
    [SerializeField] float basePlayerShieldChargeRate = 10;
    [SerializeField] float basePlayerShieldUseRate = 34;
    [SerializeField] float basePlayerStrafeSpeed = 3.8f;
    [SerializeField] float basePlayerWeaponCapacity = 600;
    [SerializeField] float basePlayerWeaponChargeRate = 20;
    [SerializeField] float basePlayerWeaponCoolTime = 90;
    [SerializeField] float basePlayerWeaponUseRate = 5;
    [SerializeField] int basePlayerVolley = 2;

    [Space(10)]
    [Header("Player components:")]
    [Tooltip("Weapon battery slider")]                                  [SerializeField] Slider weaponSlider;
    [Tooltip("Weapon battery slider colours")]                          [SerializeField] Color weaponCharged, weaponDischarged;
    [Tooltip("Weapon battery slider fill for colour control")]          [SerializeField] Image weaponFill;

    [Space(6)]
    [Tooltip("Shield battery slider")]                                  [SerializeField] Slider shieldSlider;
    [Tooltip("Shield battery slider colours")]                          [SerializeField] Color shieldCharged, shieldDischarged;
    [Tooltip("Shield battery slider fill for colour control")]          [SerializeField] Image shieldFill;

    [Space(6)]
    [SerializeField] AudioClip bonusSound;
    [SerializeField] AudioClip dischargeSound;
    [SerializeField] AudioClip explodeSound;
    [SerializeField] AudioClip hitSound;
    [SerializeField] GameObject dischargeLight_0;   [SerializeField] ParticleSystem weapon_0;
    [SerializeField] GameObject dischargeLight_1;   [SerializeField] ParticleSystem weapon_1;
    [SerializeField] GameObject dischargeLight_2;   [SerializeField] ParticleSystem weapon_2;
    [SerializeField] GameObject dischargeLight_3;   [SerializeField] ParticleSystem weapon_3;
    #endregion

    #region More member variables... but shhh... these ones are private!
    private AudioSource                 audioSource;
    private bool                        alive = true;
    private bool                        debugMode = false;
    private bool                        invulnerable = false;
    private bool                        maxEnergy = false;
    private float                       coolTime = 0;
    private float                       delta = 0;
    private float                       rollTime = 0; 
    private float                       shieldBattery = 0;
    private float                       weaponBattery = 0;
    private int                         lastWeaponFired = 0;
    private int                         rollDirection = 0;
    private KeyValet                    keyValet;
    private PlayerHandler               playerHandler;
    private Quaternion                  startRot;
    private Vector2                     controlAxis = Vector2.zero;
    private Vector3                     priorRotation = Vector3.zero;
    private Vector3                     startPos;
    private WaypointProgressTracker     waypointProgressTracker;
    #endregion

    #region Getters and Setters...
    public float PlayerDamage
    {
        get { return basePlayerDamage; }
        set { basePlayerDamage = value; }
    }

    public float PlayerForwardSpeed
    {
        get { return basePlayerForwardSpeed; }
        set { basePlayerForwardSpeed = value; }
    }

    public float PlayerRollDelay
    {
        get { return basePlayerRollDelay; }
        set { basePlayerRollDelay = value; }
    }

    public float PlayerShieldCapacity
    {
        get { return basePlayerShieldCapacity; }
        set { basePlayerShieldCapacity = value; }
    }

    public float PlayerShieldChargeRate
    {
        get { return basePlayerShieldChargeRate; }
        set { basePlayerShieldChargeRate = value; }
    }

    public float PlayerShieldUseRate
    {
        get { return basePlayerShieldUseRate; }
        set { basePlayerShieldUseRate = value; }
    }

    public float PlayerStrafeSpeed
    {
        get { return basePlayerStrafeSpeed; }
        set { basePlayerStrafeSpeed = value; }
    }

    public float PlayerWeaponCapacity
    {
        get { return basePlayerWeaponCapacity; }
        set { basePlayerWeaponCapacity = value; }
    }

    public float PlayerWeaponChargeRate
    {
        get { return basePlayerWeaponChargeRate; }
        set { basePlayerWeaponChargeRate = value; }
    }

    public float PlayerWeaponCoolTime
    {
        get { return basePlayerWeaponCoolTime; }
        set { basePlayerWeaponCoolTime = value; }
    }

    public float PlayerWeaponUseRate
    {
        get { return basePlayerWeaponUseRate; }
        set { basePlayerWeaponUseRate = value; }
    }

    public int PlayerVolley
    {
        get { return basePlayerVolley; }
        set { basePlayerVolley = value; }
    }
    #endregion

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        keyValet = FindObjectOfType<KeyValet>();
        playerHandler = FindObjectOfType<PlayerHandler>();
        waypointProgressTracker = FindObjectOfType<WaypointProgressTracker>();

        debugMode = Debug.isDebugBuild;

        invulnerableKey = keyValet.GetKey("PlayerController-ToggleInvulnerable");
        maxEnergyKey = keyValet.GetKey("PlayerController-ToggleEnergyMax");
        shieldKey = keyValet.GetKey("PlayerController-ShieldCharge");
        weaponKey = keyValet.GetKey("PlayerController-WeaponCharge");

        playerHandler.SetPlayerPosition(transform.localPosition);
        playerHandler.SetPlayerRotation(transform.localRotation);
        ChargeShieldBattery(true);
        ChargeWeaponBattery(true);

        waypointProgressTracker.SetForwardSpeed(basePlayerForwardSpeed);
    }

    private void FixedUpdate()  { UpdatePlayerPosition(); }
    private void Update()       { UpdatePlayerState(); if (debugMode) TryDebug(); }

#region Battery Maintenance...
    public void ChargeShieldBattery()
    {
        if (shieldBattery < 0) shieldBattery = 0;
        shieldBattery += (basePlayerShieldChargeRate * Time.deltaTime);
        if (shieldBattery > basePlayerShieldCapacity) shieldBattery = basePlayerShieldCapacity;
    }

    public void ChargeWeaponBattery()
    {
        if (weaponBattery < 0) weaponBattery = 0;
        weaponBattery += (basePlayerWeaponChargeRate * Time.deltaTime);
        if (weaponBattery > basePlayerWeaponCapacity) weaponBattery = basePlayerWeaponCapacity;
    }

    public void ChargeShieldBattery(bool torf)
    {
        if (torf) shieldBattery = basePlayerShieldCapacity;
        else shieldBattery = 0;
    }

    public void ChargeWeaponBattery(bool torf)
    {
        if (torf) weaponBattery = basePlayerWeaponCapacity;
        else weaponBattery = 0;
    }

    public void ChargeShieldBattery(float percentage)
    {
        if (shieldBattery < 0) shieldBattery = 0;
        shieldBattery += (percentage * basePlayerShieldCapacity);
        if (shieldBattery > basePlayerShieldCapacity) shieldBattery = basePlayerShieldCapacity;
    }

    public void ChargeWeaponBattery(float percentage)
    {
        if (weaponBattery < 0) weaponBattery = 0;
        weaponBattery += (percentage * basePlayerWeaponCapacity);
        if (weaponBattery > basePlayerWeaponCapacity) weaponBattery = basePlayerWeaponCapacity;
    }
    #endregion

#region Collision events...
    private void OnCollisionEnter(Collision collision)
    {
        // TODO: we're obviously colliding with things... why no log messages?
        Debug.Log("PlayerController.cs COLLISION tag: " + collision.gameObject.tag); 
    }

    private void OnParticleCollision(GameObject other)
    {
        //Debug.Log(other.gameObject.tag);
        switch (other.gameObject.tag)
        {
            case "EnemyPinkWeapon":
                AudioSource.PlayClipAtPoint(hitSound, transform.position);
                // TODO: have a visual effect here?
                if (!invulnerable) shieldBattery -= 40; // TODO: make this adjustbale in the inspector?
                else Debug.Log("I N V U L N E R A B L E");
                if (shieldBattery < 0)
                {
                    AudioSource.PlayClipAtPoint(explodeSound, transform.position);
                    // TODO: have a visual effect here too!
                    playerHandler.ResetPlayer();
                }
                break;
            default:
                break;
        }
    }
    #endregion

#region Other updates...
    private void SetLocalAngles()
    {
        if ((float.IsNaN(skewVertical) && float.IsNaN(skewHorizontal)) || !alive) return;

        // set a desired pitch and yaw based on screen position (and controlAxis throw if applicable):
        Vector3 pos = transform.localPosition;
        float pitch = -pos.y * skewVertical - (controlAxis.y * skewThrow);
        float yaw = pos.x * skewHorizontal + (controlAxis.x * skewThrow);

        // set a desired roll when strafing left or right vs. aileron rolling:
        float roll;
        if (Time.time > rollTime + basePlayerRollDelay && rollDirection != 0) roll = rollDirection * 370;
        else roll = controlAxis.x * -skewRoll;

        // Lerp between prior rotation and desired fixed rotation:
        pitch = Mathf.SmoothStep(priorRotation.x, pitch, delta * skewLerp);
        yaw = Mathf.SmoothStep(priorRotation.y, yaw, delta * skewLerp);
        roll = Mathf.SmoothStep(priorRotation.z, roll, delta * skewRollLerp);
        roll = MonitorRoll(roll);

        // apply rotation for this update:
        transform.localRotation = Quaternion.Euler(pitch, yaw, roll);

        // Store this updates' rotations for use next update. (X Y Z = Pitch Yaw Roll, respectively.)
        priorRotation.x = pitch;
        priorRotation.y = yaw;
        priorRotation.z = roll;
    }

    private float MonitorRoll(float roll)
    {
        if (roll < -350)
        {
            roll += 360;
            rollDirection = 0;
            rollTime = Time.time;
        }
        if (roll > 350)
        {
            roll -= 360;
            rollDirection = 0;
            rollTime = Time.time;
        }
        return roll;
    }

    private void SetLocalPosition()
    {
        if ((float.IsNaN(controlAxis.x) && float.IsNaN(controlAxis.y)) || !alive) return;

        // while aileron rolling, boost strafe speed:
        float rollBoost; if (rollDirection != 0) rollBoost = 1.75f; else rollBoost = 1;

        // set a desired position based on controlAxis input:
        float desiredXpos = transform.localPosition.x + controlAxis.x * basePlayerStrafeSpeed * rollBoost * delta;
        float desiredYpos = transform.localPosition.y + controlAxis.y * basePlayerStrafeSpeed * rollBoost * delta;

        // use bounds to confine player to viewable area:
        float clampedXPos = Mathf.Clamp(desiredXpos, -lateralRange, lateralRange);
        float clampedYPos = Mathf.Clamp(desiredYpos, -verticalMin, verticalMax);

        // apply translation:
        transform.localPosition = new Vector3(clampedXPos, clampedYPos, transform.localPosition.z);
    }

    private void UpdatePlayerPosition()
    {
        if (!alive) return;
        delta = Time.deltaTime;

        PollAxis();
        PollRolls();
        SetLocalPosition();
        SetLocalAngles();
    }

    private void UpdateShieldSlider()
    {
        Color colour;
        shieldSlider.value = shieldBattery / basePlayerShieldCapacity;
        colour = Vector4.Lerp(shieldCharged, shieldDischarged, 1 - shieldSlider.value);
        shieldFill.color = colour;
    }

    private void UpdateWeaponSlider()
    {
        Color colour;
        weaponSlider.value = weaponBattery / basePlayerWeaponCapacity;
        colour = Vector4.Lerp(weaponCharged, weaponDischarged, 1 - weaponSlider.value);
        weaponFill.color = colour;
    }

    private void UpdatePlayerState()
    {
        ClearDischargeEffects();

        ChargeWeaponBattery();
        ChargeShieldBattery();
        TryDischargeWeapon();
        UpdateWeaponSlider();
        UpdateShieldSlider();
    }
    #endregion

#region All the rest...
    private void ClearDischargeEffects()
    {
        dischargeLight_0.SetActive(false);
        dischargeLight_1.SetActive(false);
        dischargeLight_2.SetActive(false);
        dischargeLight_3.SetActive(false);
    }

    private void PollAxis()
    {
        if (!alive) return;

        controlAxis.x = CrossPlatformInputManager.GetAxis("Horizontal");
        controlAxis.y = CrossPlatformInputManager.GetAxis("Vertical");
    }

    private void PollRolls()
    {
        if (!alive || rollDirection != 0) return;

        if (CrossPlatformInputManager.GetButton("RollLeft")) rollDirection = 1;
        if (CrossPlatformInputManager.GetButton("RollRight")) rollDirection = -1;
    }

    private void TryDebug()
    {
        invulnerableCommand = Input.GetKeyDown(invulnerableKey);
        maxEnergyCommand = Input.GetKeyDown(maxEnergyKey);
        rechargeShieldCommand = Input.GetKeyDown(shieldKey);
        rechargeWeaponCommand = Input.GetKeyDown(weaponKey);

        if (invulnerableCommand) invulnerable = !invulnerable;
        if (maxEnergyCommand) maxEnergy = !maxEnergy;
        if (rechargeShieldCommand) ChargeShieldBattery(true);
        if (rechargeWeaponCommand) ChargeWeaponBattery(true);
    }

    private void TryDischargeWeapon()
    {
        if (!(CrossPlatformInputManager.GetButton("Fire1"))) return;

        if (Time.time > coolTime)
        {
            if (!maxEnergy) weaponBattery -= basePlayerWeaponUseRate;
            else Debug.Log("M A X  E N E R G Y");
            if (weaponBattery < 0) return;

            coolTime = Time.time + (basePlayerWeaponCoolTime / 1000f);
            TryPewPew();
            switch (lastWeaponFired)
            {
                case 0:
                    dischargeLight_1.SetActive(true);
                    weapon_1.Emit(basePlayerVolley);
                    lastWeaponFired = 1;
                    break;
                case 1:
                    dischargeLight_2.SetActive(true);
                    weapon_2.Emit(basePlayerVolley);
                    lastWeaponFired = 2;
                    break;
                case 2:
                    dischargeLight_3.SetActive(true);
                    weapon_3.Emit(basePlayerVolley);
                    lastWeaponFired = 3;
                    break;
                case 3:
                    dischargeLight_0.SetActive(true);
                    weapon_0.Emit(basePlayerVolley);
                    lastWeaponFired = 0;
                    break;
                default:
                    break;
            }
        }
    }

    private void TryPewPew() { if (audioSource.isPlaying) return; audioSource.Play(); }
#endregion
}
