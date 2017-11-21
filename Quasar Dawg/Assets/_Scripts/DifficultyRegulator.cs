using UnityEngine;

public class DifficultyRegulator : MonoBehaviour
{
    [SerializeField] float enemyVolleyMin = 1;
    [SerializeField] float enemyVolleyMax = 4;
    [SerializeField] float enemyVolleyGrowthFactor = 0.2f;
    [SerializeField] float enemyVolleyGrowthFrequency = 60;
    private int volleyMin;
    private int volleyMax;

    [Space(8)]
    [SerializeField] float playerForwardSpeed = 0.8f;
    [SerializeField] float playerFireRate = 75;
    [SerializeField] float playerShieldChargeRate;
    [SerializeField] float playerWeaponChargeRate;
    [SerializeField] float playerShieldCapacity;
    [SerializeField] float playerWeaponCapacity;
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
        enemyVolleyMin = enemyVolleyMin += enemyVolleyMin * enemyVolleyGrowthFactor * (Time.time / enemyVolleyGrowthFrequency);
        enemyVolleyMax = enemyVolleyMax += enemyVolleyMax * enemyVolleyGrowthFactor * (Time.time / enemyVolleyGrowthFrequency);
        volleyMin = (int)Mathf.Abs(enemyVolleyMin);
        volleyMax = (int)Mathf.Abs(enemyVolleyMax);


    }
}
