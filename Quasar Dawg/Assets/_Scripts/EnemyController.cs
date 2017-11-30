using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] AudioClip explode, hit;
    [SerializeField] float maxFireDelay = 4.2f;
    [SerializeField] float minFireDelay = 0.8f;
    [SerializeField] float range = 10;
    [SerializeField] int hitPoints = 10;
    [SerializeField] int hitScore = 3;
    [SerializeField] int killScore = 15;
    [SerializeField] ParticleSystem weaponParticleSystem;

    private DifficultyRegulator difficultyRegulator;
    private EffectPool hitPool;
    private EffectPool killPool;
    private float firetime;
    private GUITextHandler guiTextHandler;
    private PlayerController playerController;
    private PlayerHandler playerHandler;

    private static int seed = 0;

    private void Start()
    {
        if (seed == 0) seed = Mathf.FloorToInt(Time.deltaTime * 10000000);
        Random.InitState(seed);

        difficultyRegulator = FindObjectOfType<DifficultyRegulator>();
        guiTextHandler = FindObjectOfType<GUITextHandler>();
        hitPool = GameObject.FindGameObjectWithTag("HitPool").GetComponent<EffectPool>();
        killPool = GameObject.FindGameObjectWithTag("KillPool").GetComponent<EffectPool>();
        playerController = FindObjectOfType<PlayerController>();
        playerHandler = FindObjectOfType<PlayerHandler>();
    }

    private void Update()
    {
        if (playerHandler.PlayerIsAlive())
        {
            if (playerController == null) 
            {
                playerController = FindObjectOfType<PlayerController>();
                if (!playerController) Debug.Log("EnemyController.cs: playerController INFO, REUP-FAIL.");
            }
            transform.LookAt(playerController.transform);
            TryShoot();
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.tag == "PlayerBeamWeapon")
        {
            hitPoints--;
            guiTextHandler.AddToScore(hitScore);
            hitPool.PopEffect(transform);
            AudioSource.PlayClipAtPoint(hit, transform.position);
            if (hitPoints <= 0)
            {
                guiTextHandler.AddToScore(killScore);
                killPool.PopEffect(transform);
                AudioSource.PlayClipAtPoint(explode, transform.position);
                Destroy(this.gameObject);
            }
        }
    }

    private void TryShoot()
    {
        float distance = Vector3.Distance(transform.position, playerController.transform.position);
        if (distance < range && (Time.time > firetime))
        {
            firetime = Time.time + Random.Range(minFireDelay, maxFireDelay);
            weaponParticleSystem.Emit(Random.Range(difficultyRegulator.EnemyVolleyMin(), difficultyRegulator.EnemyVolleyMax()));
        }
    }
}

