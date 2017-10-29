using UnityEngine;

public class OrbController : MonoBehaviour {
    [SerializeField] ParticleSystem greenSmoke;

    void OnTriggerEnter(Collider other)
    {
        var em = greenSmoke.emission;
        em.rateOverTime = 0;
    }
}
