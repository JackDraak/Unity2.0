using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] int hitPoints = 5;

    ObjectPool hitPool;
    ObjectPool killPool;

    private void Start()
    {
        bool success;

        success = (hitPool = GameObject.FindGameObjectWithTag("HitPool").GetComponent<ObjectPool>());
        if (!success) Debug.Log("EnemyController.cs: HitPool ERROR.");

        success = (killPool = GameObject.FindGameObjectWithTag("KillPool").GetComponent<ObjectPool>());
        if (!success) Debug.Log("EnemyController.cs: KillPool ERROR.");
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.tag == "PlayerBeamWeapon") hitPool.PopEffect(transform);
        hitPoints--;
        if (hitPoints <= 0)
        {
            killPool.PopEffect(transform);
            Destroy(this.gameObject);
        }
    }
}

