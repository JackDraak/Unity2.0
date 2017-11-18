using UnityEngine;

public class EnemyController : MonoBehaviour
{
    HitPool hitPool;

    private void Start()
    {
        bool success;

        success = (hitPool = GameObject.FindObjectOfType<HitPool>());
        if (!success) Debug.Log("EnemyController.cs: HitPool ERROR.");
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.tag == "PlayerBeamWeapon") hitPool.HitEffect(transform);
    }
}

