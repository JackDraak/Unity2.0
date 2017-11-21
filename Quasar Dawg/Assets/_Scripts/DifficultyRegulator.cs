using UnityEngine;

public class DifficultyRegulator : MonoBehaviour
{
    [SerializeField] float enemyVolleyMin = 1;
    [SerializeField] float enemyVolleyMax = 2;
    [SerializeField] float enemyVolleyGrowthFactor = 0.1f;
    [SerializeField] float enemyVolleyGrowthFrequency = 20;
    private int volleyMin;
    private int volleyMax;

    [Space(8)]
    [SerializeField] float playerForwardSpeed = 0.8f;
    [SerializeField] float playerFireRate = 75;
    [SerializeField] float playerShieldChargeRate = 20;
    [SerializeField] float playerWeaponChargeRate = 20;
    [SerializeField] float playerShieldCapacity = 600;
    [SerializeField] float playerWeaponCapacity = 600;
    [SerializeField] float playerDamage;

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
        enemyVolleyMin = enemyVolleyMin += enemyVolleyMin * enemyVolleyGrowthFactor * (Time.deltaTime / enemyVolleyGrowthFrequency);
        enemyVolleyMax = enemyVolleyMax += enemyVolleyMax * enemyVolleyGrowthFactor * (Time.deltaTime / enemyVolleyGrowthFrequency);
        volleyMin = (int)Mathf.Abs(enemyVolleyMin);
        volleyMax = (int)Mathf.Abs(enemyVolleyMax);
    }

    public int EnemyVolleyMin() { return volleyMin; }
    public int EnemyVolleyMax() { return volleyMax; }
}
