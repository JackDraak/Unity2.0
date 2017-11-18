using UnityEngine;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    [SerializeField] GameObject weaponHitEffect;
    [SerializeField] float destroyDelay = 3;

    HitPool hitPool;

    struct OldEffect
    {
        public GameObject gameObject;
        public float creationTime;
    }

    List<OldEffect> oldEffects = new List<OldEffect>();

    private void Start()
    {
        bool success;

        success = (hitPool = GameObject.FindObjectOfType<HitPool>());
        if (!success) Debug.Log("EnemyController.cs: HitPool ERROR.");
    }

    private void Update()
    {
        if (oldEffects.Count > 0)
        {
            Debug.Log(oldEffects.Count);
            foreach (OldEffect effect in oldEffects)
            {
                if (Time.time > effect.creationTime + destroyDelay)
                {
                    DestroyObject(effect);
                }
                if (Time.time > effect.creationTime + destroyDelay + 1)
                {
                    oldEffects.Remove(effect);
                }
            }
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        // Debug.Log("EnemyController.cs OnParticleCollision(" + other.gameObject.tag + ")");
        if (other.gameObject.tag == "PlayerBeamWeapon") ImHit();
    }

    private void ImHit()
    {
        
    }

    private void TakeBeamHit() // this was setup prior to HitPool.cs
    {
        // public static Object Instantiate(Object original, Transform parent, bool instantiateInWorldSpace);
        // public static Object Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent);
        GameObject hitEffectObject = Instantiate(weaponHitEffect, transform.position, Quaternion.identity, transform);

        hitEffectObject.transform.localScale = Vector3.one * 0.1f;

        OldEffect effect;
        effect.gameObject = hitEffectObject;
        effect.creationTime = Time.time;
        oldEffects.Add(effect);
    }

    private void DestroyObject(OldEffect trash)
    {
        DestroyObject(trash.gameObject);
        //oldEffects.Remove(trash);
    }
}

