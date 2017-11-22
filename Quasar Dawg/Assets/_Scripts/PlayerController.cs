using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;

/* Development notes: Lots of place-holder assets, many that I created myself, others from the asset
 * store or elsewhere (i.e. from the Udemy course).  While I publish this source on GitHub and am glad
 * if people find utility in it, please be careful to not grab/use something here in a way you may not
 * be allowed to. (In general, anything here can be used freely for personal use, but again, please be
 * informed before you go rummaging for treasure.) Cheers. -Jack D.
 * 
 * TODO: Prevent spawning while player resetting? 
 * 
 * TODO: enemy weapon that freeezes recharge?
 * TODO: have spawn-rate increase over time
 * DONE: have enemy volley increase over time.
 * TODO: use camera-speed hooks (DONE) to have a boss fight? (WIP)
 *      
 * TODO: Improve ship damage/reset effects/sounds
 * TODO: Make enemies more interesting / animated
 * TODO: Add pause button
 * 
 * TODO: Use Q, E or shoulder buttons for barrell-roll dodge.
 * 
 * TODO: Have player controller take control of camera/waypoint script in order to stop (slow/speed-up?)
 *       waypoint patrol for Boss battles or something?
 *       
 * TODO: Have other collectible boosters or downers....
 * 
 *     ... i.e. handles in-place to boost or retard discharge-rate, volley-size, battery capacity, 
 *     etc., player maneuverability factors aplenty....
 */

public class PlayerController : MonoBehaviour
{
    // DEVNOTE: These debugging commands work in the editor or on "development" builds. 
    // Assign them to keys not in-use [in the KeyValet.cs class]:
    private bool rechargeShieldCommand;     private KeyCode shieldKey;
    private bool rechargeWeaponCommand;     private KeyCode weaponKey;
    private bool invulnerableCommand;       private KeyCode invulnerableKey;
    private bool maxEnergyCommand;          private KeyCode maxEnergyKey;

    #region So many things to set in the inspector....
    [Header("Values to tweak Player facing angles:")]
    [Range(0f, 18f)][Tooltip("factor for vertical rotation skew")]          [SerializeField] float skewHorizontal = 9f;
    [Range(0f, 90f)][Tooltip("factor for roll skew, in degrees")]           [SerializeField] float skewRoll = 45f;
    [Range(0f, 20f)][Tooltip("factor for throw (axis) skew")]               [SerializeField] float skewThrow = 10f;
    [Range(0f, 18f)][Tooltip("factor for lateral rotation skew")]           [SerializeField] float skewVertical = 9f;
    [Space(6)]
    [Range(0f, 30f)][Tooltip("factor for skew Lerping for pitch and yaw")]  [SerializeField] float skewLerp = 15f;
    [Range(0f, 10f)][Tooltip("factor for skew Lerping for roll")]           [SerializeField] float skewRollLerp = 5f;

    [Space(10)][Header("Player bounds:")]
    [Range(0f, 9.6f)]   [Tooltip("Range of drift, in m")]                   [SerializeField] float lateralRange = 4.8f;
    [Range(0f, 5.6f)]   [Tooltip("Range of drift, in m")]                   [SerializeField] float verticalMax = 2.8f;
    [Range(0f, 5.0f)]   [Tooltip("Range of drift, in m")]                   [SerializeField] float verticalMin = 2.5f;
    [Space(6)]
    [Range(0f, 10.4f)]  [Tooltip("Speed, in ms^-1")]                        [SerializeField] float strafeSpeed = 5.2f;
    [Range(1, 12)]      [Tooltip("Weapon volley, in particles/discharge")]  [SerializeField] int volley = 3;
    [Range(0f, 500.0f)] [Tooltip("Weapon cooldown time, in ms")]            [SerializeField] float weaponCooldownTime = 75f;
    [Space(6)]
    [Range(1, 1200)]    [Tooltip("Shield battery capacity")]                [SerializeField] int shieldCapacity = 600;
    [Range(1, 40)]      [Tooltip("Shield battery charge-rate, in p/s")]     [SerializeField] int shieldChargeRate = 20;
    [Range(0, 16)]      [Tooltip("Shield battery use-rate, in p/volley")]   [SerializeField] int shieldUseRate = 8;
    [Space(6)]
    [Range(1, 1200)]    [Tooltip("Weapon battery capacity")]                [SerializeField] int weaponCapacity = 600;
    [Range(1, 40)]      [Tooltip("Weapon battery charge-rate, in p/s")]     [SerializeField] int weaponChargeRate = 20;
    [Range(0, 16)]      [Tooltip("Weapon battery use-rate, in p/volley")]   [SerializeField] int weaponUseRate = 8;

    [Space(10)][Header("Player components:")]
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

    #region More member variables... but shhh... these ones are pirvate!
    private AudioSource                 audioSource;
    private bool                        alive = true;
    private bool                        debugMode = false;
    private bool                        invulnerable = false;
    private bool                        maxEnergy = false;
    private float                       coolTime = 0;
    private float                       delta = 0;
    private float                       shieldBattery = 0;
    private float                       weaponBattery = 0;
    private int                         lastWeaponFired = 0;
    private Vector2                     controlAxis = Vector2.zero;
    private Vector3                     priorRotation = Vector3.zero;
    private Vector3                     startPos;
    private KeyValet                    keyValet;
    private PlayerHandler               playerHandler;
    private Quaternion                  startRot;
    private WaypointProgressTracker     waypointProgressTracker;
#endregion

    private void Start()
    {
        if (!(audioSource = GetComponent<AudioSource>()))
            Debug.Log("PlayerController.cs: audioSource INFO, ERROR.");
        if (!(keyValet = FindObjectOfType<KeyValet>()))
            Debug.Log("PlayerController.cs: keyValet INFO, ERROR.");
        if (!(playerHandler = FindObjectOfType<PlayerHandler>()))
            Debug.Log("PlayerController.cs: playerHandler INFO, ERROR.");
        if (!(waypointProgressTracker = FindObjectOfType<WaypointProgressTracker>()))
            Debug.Log("PlayerController.cs: waypointProgressTracker INFO, ERROR");

        debugMode = Debug.isDebugBuild;

        shieldKey = keyValet.GetKey("PlayerController-ShieldCharge");
        weaponKey = keyValet.GetKey("PlayerController-WeaponCharge");
        invulnerableKey = keyValet.GetKey("PlayerController-ToggleInvulnerable");
        maxEnergyKey = keyValet.GetKey("PlayerController-ToggleEnergyMax");

        playerHandler.SetPlayerPosition(transform.localPosition);
        playerHandler.SetPlayerRotation(transform.localRotation);
        ChargeShieldBattery(true);
        ChargeWeaponBattery(true);

        waypointProgressTracker.SetForwardSpeed(.8f); // TODO: do more fun things with forward speed.....
    }

    private void FixedUpdate()  { UpdatePlayerPosition(); }
    private void Update()       { UpdatePlayerState(); if (debugMode) TryDebug(); }

#region Battery Maintenance...
    public void ChargeShieldBattery()
    {
        if (shieldBattery < 0) shieldBattery = 0;
        shieldBattery += (shieldChargeRate * Time.deltaTime);
        if (shieldBattery > shieldCapacity) shieldBattery = shieldCapacity;
    }

    public void ChargeWeaponBattery()
    {
        if (weaponBattery < 0) weaponBattery = 0;
        weaponBattery += (weaponChargeRate * Time.deltaTime);
        if (weaponBattery > weaponCapacity) weaponBattery = weaponCapacity;
    }

    public void ChargeShieldBattery(bool torf)
    {
        if (torf) shieldBattery = shieldCapacity;
        else shieldBattery = 0;
    }

    public void ChargeWeaponBattery(bool torf)
    {
        if (torf) weaponBattery = weaponCapacity;
        else weaponBattery = 0;
    }

    public void ChargeShieldBattery(float percentage)
    {
        if (shieldBattery < 0) shieldBattery = 0;
        shieldBattery += (percentage * shieldCapacity);
        if (shieldBattery > shieldCapacity) shieldBattery = shieldCapacity;
    }

    public void ChargeWeaponBattery(float percentage)
    {
        if (weaponBattery < 0) weaponBattery = 0;
        weaponBattery += (percentage * weaponCapacity);
        if (weaponBattery > weaponCapacity) weaponBattery = weaponCapacity;
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

        // set a desired roll when strafing left or right:
        float roll = controlAxis.x * -skewRoll;

        // Lerp between prior rotation and desired fixed rotation:
        //pitch = Mathf.Lerp(priorRotation.x, pitch, delta * skewLerp);
        //yaw = Mathf.Lerp(priorRotation.y, yaw, delta * skewLerp);
        //roll = Mathf.Lerp(priorRotation.z, roll, delta * skewRollLerp);

        pitch = Mathf.SmoothStep(priorRotation.x, pitch, delta * skewLerp);
        yaw = Mathf.SmoothStep(priorRotation.y, yaw, delta * skewLerp);
        roll = Mathf.SmoothStep(priorRotation.z, roll, delta * skewRollLerp);

        // apply rotation for this update:
        transform.localRotation = Quaternion.Euler(pitch, yaw, roll);

        // Store this updates' rotations for use next update. (X Y Z = Pitch Yaw Roll, respectively.)
        priorRotation.x = pitch;
        priorRotation.y = yaw;
        priorRotation.z = roll;
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

    private void UpdatePlayerPosition()
    {
        if (!alive) return;

        PollAxis();
        delta = Time.deltaTime;
        SetLocalPosition();
        SetLocalAngles();
    }

    private void UpdateShieldSlider()
    {
        Color colour = Color.white;
        shieldSlider.value = shieldBattery / shieldCapacity;
        colour = Vector4.Lerp(shieldCharged, shieldDischarged, 1 - shieldSlider.value);
        //   colour.r = Mathf.Lerp(shieldCharged.r, shieldDischarged.r, 1 - shieldSlider.value);
        //    colour.g = Mathf.Lerp(shieldCharged.g, shieldDischarged.g, 1 - shieldSlider.value);
        //    colour.b = Mathf.Lerp(shieldCharged.b, shieldDischarged.b, 1 - shieldSlider.value);
        //    colour.a = Mathf.Lerp(shieldCharged.a, shieldDischarged.a, 1 - shieldSlider.value);
        shieldFill.color = colour;
    }

    private void UpdateWeaponSlider()
    {
        Color colour = Color.white;
        weaponSlider.value = weaponBattery / weaponCapacity;
        colour = Vector4.Lerp(weaponCharged, weaponDischarged, 1 - weaponSlider.value);
         //   colour.r = Mathf.Lerp(weaponCharged.r, weaponDischarged.r, 1 - weaponSlider.value);
         //   colour.g = Mathf.Lerp(weaponCharged.g, weaponDischarged.g, 1 - weaponSlider.value);
         //   colour.b = Mathf.Lerp(weaponCharged.b, weaponDischarged.b, 1 - weaponSlider.value);
         //   colour.a = Mathf.Lerp(weaponCharged.a, weaponDischarged.a, 1 - weaponSlider.value);
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
            if (!maxEnergy) weaponBattery -= weaponUseRate;
            else Debug.Log("M A X  E N E R G Y");
            if (weaponBattery < 0) return;

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

    private void TryPewPew() { if (audioSource.isPlaying) return; audioSource.Play(); }
#endregion
}
