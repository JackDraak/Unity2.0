using UnityEngine;

public class WeaponController : MonoBehaviour {
    public GameObject weaponHitEffect;

	// Use this for initialization
	void Start ()
    {
		
	}

    void OnParticleCollision(GameObject other)
    {
        Debug.Log("WeaponController.cs OnParticleCollision(" + other.gameObject.tag + ")");
    /*    Rigidbody body = other.GetComponent<Rigidbody>();
        if (body)
        {
            Vector3 direction = other.transform.position - transform.position;
            Instantiate(weaponHitEffect, this.transform); // TODO fix this
            direction = direction.normalized;
            body.AddForce(direction * 5);
        } */
    }
}
