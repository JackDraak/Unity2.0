using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] AudioClip explode, hit;
    [SerializeField] float maxFireDelay = 4.2f;
    [SerializeField] float minFireDelay = 0.8f;
    [SerializeField] float range = 10;
    [SerializeField] int hitPoints = 10;
    [SerializeField] ParticleSystem weapon;

    private DifficultyRegulator difficultyRegulator;
    private EffectPool hitPool;
    private EffectPool killPool;
    private float firetime;
    private PlayerController playerController;
    private PlayerHandler playerHandler;
    private GUITextHandler guiTextHandler;

    private void Start()
    {
        bool success;

        success = (hitPool = GameObject.FindGameObjectWithTag("HitPool").GetComponent<EffectPool>());
            if (!success) Debug.Log("EnemyController.cs: hitPool INFO, FAIL.");
        success = (killPool = GameObject.FindGameObjectWithTag("KillPool").GetComponent<EffectPool>());
            if (!success) Debug.Log("EnemyController.cs: killPool INFO, FAIL.");
        success = (playerController = FindObjectOfType<PlayerController>());
            if (!success) Debug.Log("EnemyController.cs: playerController INFO, FAIL.");
        success = (playerHandler = FindObjectOfType<PlayerHandler>());
            if (!playerHandler) Debug.Log("EnemyController.cs: playerHandler INFO, FAIL.");
        success = (guiTextHandler = FindObjectOfType<GUITextHandler>());
            if (!success) Debug.Log("EnemyController.cs: guiTextHandler INFO, FAIL.");
        success = (difficultyRegulator = FindObjectOfType<DifficultyRegulator>());
            if (!success) Debug.Log("EnemyController.cs: difficultyRegulator INFO, FAIL.");
    }

    private void Update()
    {
        if (playerHandler.PlayerIsAlive())
        {
            if (playerController == null) 
            {
                // Debug.Log("EC.cs playerController REUP:");
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
            guiTextHandler.AddToScore(3); // TODO: move this value to the inspector? Have it go up over time?
            hitPool.PopEffect(transform);
            AudioSource.PlayClipAtPoint(hit, transform.position);
            if (hitPoints <= 0)
            {
                guiTextHandler.AddToScore(15); // TODO: move this value to the inspector? Have it go up over time?
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
            weapon.Emit(Random.Range(difficultyRegulator.EnemyVolleyMin(), difficultyRegulator.EnemyVolleyMax()));
        }
    }
}

