using UnityEngine;

public class DifficultyRegulator : MonoBehaviour
{
    [Header("Enemy defaults:")]
    [SerializeField] float enemyVolleyMin = 1;
    [SerializeField] int enemyVolleyMinClamp = 25;

    [Space(6)]
    [SerializeField] float enemyVolleyMax = 2;
    [SerializeField] int enemyVolleyMaxClamp = 50;

    [Space(6)]
    [Tooltip("percent/100, i.e. 1 = 100%")]
    [SerializeField] float enemyVolleyGrowthFactor = 0.07f; // 8% growth...

    [Tooltip("time taken to grow 1 factor, in seconds")]
    [SerializeField] float enemyVolleyGrowthFrequency = 30; // ...every 25 seconds.

    [Space(8)]
    [Header("Player defaults:")]
    [SerializeField] float basePlayerDamage = 1;
    [SerializeField] float basePlayerForwardSpeed = 0.85f; 
    [SerializeField] float basePlayerRollDelay = 2f;
    [SerializeField] float basePlayerShieldCapacity = 250;
    [SerializeField] float basePlayerShieldChargeRate = 12;
    [SerializeField] float basePlayerShieldUseRate = 33;
    [SerializeField] float basePlayerStrafeSpeed = 3.8f;
    [SerializeField] float basePlayerWeaponCapacity = 600;
    [SerializeField] float basePlayerWeaponChargeRate = 20;
    [SerializeField] float basePlayerWeaponCoolTime = 65;
    [SerializeField] float basePlayerWeaponUseRate = 5;
    [SerializeField] int basePlayerVolley = 3;

    #region Getters and Setters...
    public float playerDamage
    {
        get { return basePlayerDamage; }
        set { basePlayerDamage = value; }
    }

    public float playerForwardSpeed
    {
        get { return basePlayerForwardSpeed; }
        set { basePlayerForwardSpeed = value; }
    }

    public float playerRollDelay
    {
        get { return basePlayerRollDelay; }
        set { basePlayerRollDelay = value; }
    }

    public float playerShieldCapacity
    {
        get { return basePlayerShieldCapacity; }
        set { basePlayerShieldCapacity = value; }
    }

    public float playerShieldChargeRate
    {
        get { return basePlayerShieldChargeRate; }
        set { basePlayerShieldChargeRate = value; }
    }

    public float playerShieldUseRate
    {
        get { return basePlayerShieldUseRate; }
        set { basePlayerShieldUseRate = value; }
    }

    public float playerStrafeSpeed
    {
        get { return basePlayerStrafeSpeed; }
        set { basePlayerStrafeSpeed = value; }
    }

    public float playerWeaponCapacity
    {
        get { return basePlayerWeaponCapacity; }
        set { basePlayerWeaponCapacity = value; }
    }

    public float playerWeaponChargeRate
    {
        get { return basePlayerWeaponChargeRate; }
        set { basePlayerWeaponChargeRate = value; }
    }

    public float playerWeaponCoolTime
    {
        get { return basePlayerWeaponCoolTime; }
        set { basePlayerWeaponCoolTime = value; }
    }

    public float playerWeaponUseRate
    {
        get { return basePlayerWeaponUseRate; }
        set { basePlayerWeaponUseRate = value; }
    }

    public int playerVolley
    {
        get { return basePlayerVolley; }
        set { basePlayerVolley = value; }
    }
    #endregion

    private GUITextHandler guiTextHandler;

    private int volleyMax = 0;
    private int volleyMin = 0;
    private bool adrenalineRush;
    private bool volleyMinClapmed = false;
    private bool volleyMaxClamped = false;
    private float adrenalineStartTime = 0;
    private float adrenalineDurationTime = 4.2f;
    private float regularTimeScale = 1;

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
        bool success;

        success = (guiTextHandler = FindObjectOfType<GUITextHandler>());
            if (!success) Debug.Log("DifficultyRegulator.cs: guiTextHandler INFO, FAIL.");

        volleyMin = (int) Mathf.Abs(enemyVolleyMin);
        volleyMax = (int)Mathf.Abs(enemyVolleyMax);
    }

    private void Update()
    {
        RegulateEnemyVolley();
        RegulateAdrenalineRush();
    }

    public int EnemyVolleyMin() { return volleyMin; }
    public int EnemyVolleyMax() { return volleyMax; }

    private void RegulateAdrenalineRush()
    {
        if (Time.time > adrenalineDurationTime + adrenalineStartTime && adrenalineRush)
        {
            adrenalineRush = false;
            NormalizeTime();
        }
    }

    private void RegulateEnemyVolley()
    {
        if (!volleyMinClapmed)
        {
            enemyVolleyMin = enemyVolleyMin += enemyVolleyMin * enemyVolleyGrowthFactor *
                (Time.deltaTime / enemyVolleyGrowthFrequency);
            volleyMin = (int)Mathf.Abs(enemyVolleyMin);

        }
        if (!volleyMaxClamped)
        {
            enemyVolleyMax = enemyVolleyMax += enemyVolleyMax * enemyVolleyGrowthFactor *
                (Time.deltaTime / enemyVolleyGrowthFrequency);
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

    private void NormalizeTime()
    {
        Time.timeScale = regularTimeScale;
        guiTextHandler.DropText();
    }
}
