using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] AudioClip explode, hit;
    [SerializeField] float maxFireDelay = 4.2f;
    [SerializeField] float minFireDelay = 0.8f;
    [SerializeField] float range = 10;
    [SerializeField] int hitPoints = 10;
    [SerializeField] int volley = 1;
    [SerializeField] ParticleSystem weapon;

    private float firetime;
    private ObjectPool hitPool;
    private ObjectPool killPool;
    private PlayerController playerController;
    private PlayerManager playerManager;
    private ScoreController scoreController;

    private void Start()
    {
        bool success;

        success = (hitPool = GameObject.FindGameObjectWithTag("HitPool").GetComponent<ObjectPool>());
            if (!success) Debug.Log("EnemyController.cs: HitPool ERROR.");
        success = (killPool = GameObject.FindGameObjectWithTag("KillPool").GetComponent<ObjectPool>());
            if (!success) Debug.Log("EnemyController.cs: KillPool ERROR.");
        success = (playerController = FindObjectOfType<PlayerController>());
            if (!success) Debug.Log("EnemyController.cs: PlayerController ERROR.");
        success = (playerManager = FindObjectOfType<PlayerManager>());
            if (!playerManager) Debug.Log("EnemyController.cs: no playerManager ERROR.");
        success = (scoreController = FindObjectOfType<ScoreController>());
            if (!success) Debug.Log("EnemyController.cs: ScoreController ERROR.");
    }

    private void Update()
    {
        if (playerManager.PlayerIsAlive())
        {
            if (playerController == null) 
            {
                // Debug.Log("EC.cs playerController REUP:");
                playerController = GameObject.FindObjectOfType<PlayerController>();
                if (!playerController) Debug.Log("EC.cs playerController REUP-FAIL.");
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
            scoreController.Add(3); // TODO: move this value to the inspector?
            hitPool.PopEffect(transform);
            AudioSource.PlayClipAtPoint(hit, transform.position);
            if (hitPoints <= 0)
            {
                scoreController.Add(15); // TODO: move this value to the inspector?
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
            weapon.Emit(volley);
        }
    }
}

