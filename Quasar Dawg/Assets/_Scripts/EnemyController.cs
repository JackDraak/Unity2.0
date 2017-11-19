using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] int hitPoints = 10;
    [SerializeField] ParticleSystem weapon;
    [SerializeField] int volley = 1;
    [SerializeField] float range = 10;
    [SerializeField] float minFireDelay = 0.8f;
    [SerializeField] float maxFireDelay = 4.2f;
    [SerializeField] AudioClip hit, explode;

    private ObjectPool hitPool;
    private ObjectPool killPool;
    private ScoreController scoreController;
    private PlayerController playerController;
    private float firetime;

    private void Start()
    {
        bool success;

        success = (hitPool = GameObject.FindGameObjectWithTag("HitPool").GetComponent<ObjectPool>());
            if (!success) Debug.Log("EnemyController.cs: HitPool ERROR.");
        success = (killPool = GameObject.FindGameObjectWithTag("KillPool").GetComponent<ObjectPool>());
            if (!success) Debug.Log("EnemyController.cs: KillPool ERROR.");
        success = (scoreController = GameObject.FindObjectOfType<ScoreController>());
            if (!success) Debug.Log("EnemyController.cs: ScoreController ERROR.");
        success = (playerController = GameObject.FindObjectOfType<PlayerController>());
            if (!success) Debug.Log("EnemyController.cs: PlayerController ERROR.");
    }

    private void Update()
    {
        transform.LookAt(playerController.transform);
        TryShoot();
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

    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.tag == "PlayerBeamWeapon")
        {
            hitPoints--;
            scoreController.Add(3);
            hitPool.PopEffect(transform);
            AudioSource.PlayClipAtPoint(hit, transform.position);
        }
        if (hitPoints <= 0)
        {
            scoreController.Add(15);
            killPool.PopEffect(transform);
            AudioSource.PlayClipAtPoint(explode, transform.position);
            Destroy(this.gameObject);
        }
    }
}

