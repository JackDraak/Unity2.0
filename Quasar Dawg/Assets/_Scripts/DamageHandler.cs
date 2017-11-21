using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour {

    private void OnCollisionEnter(Collision collision)
    {
        // TODO: we're obviously colliding with things... why no log messages?
        Debug.Log("DamageHandler.cs COLLISION tag: " + collision.gameObject.tag);
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("DamageHandler.cs PARTICLE_COLLISION tag: " + other.gameObject.tag);
        switch (other.gameObject.tag)
        {
            case "EnemyPinkWeapon":

                break;
            default:
                break;
        }
    }
}
