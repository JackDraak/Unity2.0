using UnityEngine;

public class OrbController : MonoBehaviour {
    [SerializeField] ParticleSystem greenSmoke;

	// Use this for initialization
	void Start ()
    {

	}

    void OnTriggerEnter(Collider other)
    {
        var em = greenSmoke.emission;
        em.rateOverTime = 0;
    }
}
