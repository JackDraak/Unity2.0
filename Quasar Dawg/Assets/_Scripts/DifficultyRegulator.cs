using UnityEngine;

public class DifficultyRegulator : MonoBehaviour
{
    [Header("Enemy defaults:")]
    [SerializeField]
    float enemyVolleyMin = 1;
    [SerializeField] int enemyVolleyMinClamp = 25;

    [Space(6)]
    [SerializeField]
    float enemyVolleyMax = 2;
    [SerializeField] int enemyVolleyMaxClamp = 50;

    [Space(6)]
    [Tooltip("percent/100, i.e. 1 = 100%")]
    [SerializeField]
    float enemyVolleyGrowthFactor = 0.07f; // 8% growth...

    [Tooltip("time taken to grow 1 factor, in seconds")]
    [SerializeField]
    float enemyVolleyGrowthFrequency = 30; // ...every 25 seconds.

    // Player-related variables:
    private float adrenalineStartTime = 0;
    private float adrenalineDurationTime = 4.2f;
    private float dampenDurationTime = 6;
    private float dampenFactor = 5;
    private float dampenStartTime = 0;
    private float defaultPlayerForwardSpeed;
    private float defaultPlayerRollDelay;
    private float defaultPlayerShieldCapacity;
    private float defaultPlayerShieldChargeRate;
    private float defaultPlayerShieldUseRate;
    private float defaultPlayerStrafeSpeed;
    private float defaultPlayerWeaponCapacity;
    private float defaultPlayerWeaponChargeRate;
    private float defaultPlayerWeaponCooldownTime;
    private float defaultPlayerWeaponUseRate;
    private float regularTimeScale = 1;
    private float strafeDampenDurationTime = 5;
    private float strafeDampenStartTime = 0;
    private GUITextHandler guiTextHandler;
    private int defaultPlayerVolley;
    private PlayerController playerController;

    // Enemy-related variables:
    private int volleyMax = 0;
    private int volleyMin = 0;
    private bool adrenalineRush = false;
    private bool dampenBlaster = false;
    private bool dampenStrafe = false;
    private bool volleyMinClapmed = false;
    private bool volleyMaxClamped = false;

    private void OnEnable()
    {
        // Singleton pattern, preferred over making the class static:
        DifficultyRegulator[] checker;
        checker = FindObjectsOfType<DifficultyRegulator>();
        if (checker.Length > 1) Destroy(gameObject);
        else DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        guiTextHandler = FindObjectOfType<GUITextHandler>();
        playerController = FindObjectOfType<PlayerController>();

        volleyMin = (int)Mathf.Abs(enemyVolleyMin);
        volleyMax = (int)Mathf.Abs(enemyVolleyMax);

        /*
        defaultPlayerForwardSpeed = playerController.PlayerForwardSpeed;
        defaultPlayerRollDelay = playerController.PlayerRollDelay;
        defaultPlayerShieldCapacity = playerController.PlayerShieldCapacity;
        defaultPlayerShieldChargeRate = playerController.PlayerShieldChargeRate;
        defaultPlayerShieldUseRate = playerController.PlayerShieldUseRate;
        defaultPlayerWeaponCapacity = playerController.PlayerWeaponCapacity;
        defaultPlayerWeaponChargeRate = playerController.PlayerWeaponChargeRate;
        defaultPlayerWeaponUseRate = playerController.PlayerWeaponUseRate;
        defaultPlayerVolley = playerController.PlayerVolley;
        */
        defaultPlayerStrafeSpeed = playerController.PlayerStrafeSpeed;
        defaultPlayerWeaponCooldownTime = playerController.PlayerWeaponCoolTime;
    }

    private void Update()
    {
        RegulateEnemy();
        RegulatePlayer();
    }

    public int EnemyVolleyMin() { return volleyMin; }
    public int EnemyVolleyMax() { return volleyMax; }

    private void RegulatePlayer()
    {
        MonitorAdrenalineRush();
        MonitorBlasters();
        MonitorStrafe();
    }

    private void MonitorAdrenalineRush()
    {
        if (Time.time > adrenalineDurationTime + adrenalineStartTime && adrenalineRush)
        {
            adrenalineRush = false;
            guiTextHandler.DropText();
            Time.timeScale = regularTimeScale;
        }
    }

    private void MonitorBlasters()
    {
        if (Time.time > dampenDurationTime + dampenStartTime && dampenBlaster)
        {
            dampenBlaster = false;
            guiTextHandler.DropText();
            playerController.PlayerWeaponCoolTime = defaultPlayerWeaponCooldownTime;
        }
    }

    private void MonitorStrafe()
    {
        if (Time.time > strafeDampenDurationTime + strafeDampenStartTime && dampenStrafe)
        {
            dampenStrafe = false;
            guiTextHandler.DropText();
            playerController.PlayerStrafeSpeed = defaultPlayerStrafeSpeed;
        }
    }

    private void RegulateEnemy()
    {
        if (!volleyMinClapmed)
        {
            enemyVolleyMin += enemyVolleyMin * enemyVolleyGrowthFactor * (Time.deltaTime / enemyVolleyGrowthFrequency);
            volleyMin = (int)Mathf.Abs(enemyVolleyMin);

        }
        if (!volleyMaxClamped)
        {
            enemyVolleyMax += enemyVolleyMax * enemyVolleyGrowthFactor * (Time.deltaTime / enemyVolleyGrowthFrequency);
            volleyMax = (int)Mathf.Abs(enemyVolleyMax);
        }

        if (volleyMin > enemyVolleyMinClamp)
        {
            volleyMin = enemyVolleyMinClamp;
            enemyVolleyMin = volleyMin;
            volleyMinClapmed = true;
        }
        if (volleyMax > enemyVolleyMaxClamp)
        {
            volleyMax = enemyVolleyMaxClamp;
            enemyVolleyMax = volleyMax;
            volleyMaxClamped = true;
        }
    }

    public void AdrenalineRush()
    {
        if (Time.time > adrenalineDurationTime + adrenalineStartTime)
        {
            guiTextHandler.PopText("<size=+20>A</size>drenaline <size=+20>R</size>ush!");
            Time.timeScale = 0.5f;
        }
        adrenalineStartTime = Time.time;
        adrenalineRush = true;
    }

    public void BlasterDampen()
    {
        if (Time.time > dampenDurationTime + dampenStartTime)
        {
            guiTextHandler.PopText("<size=+20>B</size>lasters <size=+20>D</size>ampened!");
            playerController.PlayerWeaponCoolTime = defaultPlayerWeaponCooldownTime * dampenFactor;
        }
        dampenStartTime = Time.time;
        dampenBlaster = true;
    }

    public void StrafeDampen()
    {
        if (Time.time > strafeDampenDurationTime + strafeDampenStartTime)
        {
            guiTextHandler.PopText("<size=+20>S</size>trafe <size=+20>D</size>ampened!");
            playerController.PlayerStrafeSpeed = defaultPlayerStrafeSpeed * 0.5f;
        }
        strafeDampenStartTime = Time.time;
        dampenStrafe = true;
    }
}
