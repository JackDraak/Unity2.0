using UnityEngine;

public class DifficultyRegulator : MonoBehaviour
{
    [SerializeField] float enemyVolleyMin = 1;
    [SerializeField] int enemyVolleyMinClamp = 25;

    [Space(8)]
    [SerializeField] float enemyVolleyMax = 2;
    [SerializeField] int enemyVolleyMaxClamp = 50;

    private int volleyMax = 0;
    private int volleyMin = 0;
    private bool volleyMinClapmed = false;
    private bool volleyMaxClamped = false;

    [Space(8)]
    [Tooltip("percent/100, i.e. 1 = 100%")]
    [SerializeField] float enemyVolleyGrowthFactor = 0.08f; // 8% growth...

    [Tooltip("time taken to grow 1 factor, in seconds")]
    [SerializeField] float enemyVolleyGrowthFrequency = 25; // ...every 25 seconds.

    [Space(8)]
    // TODO: tie these in for player, currently it's just filler...
    [SerializeField] float playerForwardSpeed = 0.9f; 
    [SerializeField] float playerFireRate = 75;
    [SerializeField] float playerShieldChargeRate = 20;
    [SerializeField] float playerWeaponChargeRate = 20;
    [SerializeField] float playerShieldCapacity = 600;
    [SerializeField] float playerWeaponCapacity = 600;
    [SerializeField] float playerDamage = 1;

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
        volleyMin = (int) Mathf.Abs(enemyVolleyMin);
        volleyMax = (int)Mathf.Abs(enemyVolleyMax);
    }

    private void Update()
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

    public int EnemyVolleyMin() { return volleyMin; }
    public int EnemyVolleyMax() { return volleyMax; }
}
