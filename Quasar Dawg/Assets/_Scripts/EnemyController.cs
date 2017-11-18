using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] int hitPoints = 5;

    ObjectPool hitPool;
    ObjectPool killPool;
    ScoreController scoreController;

    private void Start()
    {
        bool success;

        success = (hitPool = GameObject.FindGameObjectWithTag("HitPool").GetComponent<ObjectPool>());
        if (!success) Debug.Log("EnemyController.cs: HitPool ERROR.");

        success = (killPool = GameObject.FindGameObjectWithTag("KillPool").GetComponent<ObjectPool>());
        if (!success) Debug.Log("EnemyController.cs: KillPool ERROR.");

        success = (scoreController = GameObject.FindObjectOfType<ScoreController>());
        if (!success) Debug.Log("EnemyController.cs: ScoreController ERROR.");
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.tag == "PlayerBeamWeapon")
        {
            hitPoints--;
            scoreController.Add(3);
            hitPool.PopEffect(transform);
        }
        if (hitPoints <= 0)
        {
            scoreController.Add(15);
            killPool.PopEffect(transform);
            Destroy(this.gameObject);
        }
    }
}

