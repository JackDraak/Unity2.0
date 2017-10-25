using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DroneController : MonoBehaviour
{
    [SerializeField] AudioClip bonusSound;
    [SerializeField] AudioClip collisionSound;
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] GameObject siren_B;
    [SerializeField] GameObject siren_R;
    [SerializeField] AudioClip thrustSound;
    private bool resetting = false;
    private bool thrustAudio;
    private const int BaseHitPoints = 5;
    private const int maxPlayerLives = 5;
    private int hitPoints = BaseHitPoints;
    private int playerLives = maxPlayerLives;
    private int score = 0;
    private AudioSource audioSource;
    private Quaternion startRotation;
    private Rigidbody rigidbody;
    private RigidbodyConstraints rigidbodyConstraints;
    private Vector3 startPosition;

    // Use this for initialization.
    void Start()
    {
        bool pass;
        pass = (rigidbody = GetComponent<Rigidbody>());
            if (!pass) Debug.Log("FAIL: rigidbody");
        pass = (audioSource = GetComponent<AudioSource>());
            if (!pass) Debug.Log("FAIL: audioSource");
        startPosition = transform.position;
        startRotation = transform.rotation;
        rigidbodyConstraints = rigidbody.constraints;
        playerLives++; ResetPlayer(true);
    }

    // Update is called once per frame.
    void Update()
    {
        ProcessMass();
        ProcessInput();
        RotateSirenLamps();
        ProcessVisualEffects();
        ProcessAudio();
    }

    private void ProcessMass()
    {
        float driftMass;
        float driftMassFactor = 70f;
        driftMass = Time.deltaTime / driftMassFactor;
        rigidbody.mass += driftMass;
    }

    private void RotateSirenLamps()
    {
        float sirenRotationBase = 2f;
        float sirenRotationFactor = 0.5f;
        float sirenSpeed = 200f;
        siren_R.transform.Rotate
            (new Vector3(0, 1, 0)
            * Time.deltaTime
            * sirenSpeed
            * Mathf.Pow(sirenRotationBase, sirenRotationFactor + rigidbody.mass));
        siren_B.transform.Rotate
            (new Vector3(0, -1, 0)
            * Time.deltaTime
            * sirenSpeed
            * Mathf.Pow(sirenRotationBase, sirenRotationFactor + rigidbody.mass));
    }

    private void ProcessInput()
    {
        // Data
        // TODO: allow swap of port/starboard numbers if player wants inverted controls.
        int port = -1;
        int starboard = 1;
        const int main = 0;

        // Inputs we care about
        bool key_w = Input.GetKey(KeyCode.W);
        bool key_a = Input.GetKey(KeyCode.A);
        bool key_d = Input.GetKey(KeyCode.D);
        bool key_ua = Input.GetKey(KeyCode.UpArrow);
        bool key_la = Input.GetKey(KeyCode.LeftArrow);
        bool key_ra = Input.GetKey(KeyCode.RightArrow);
        bool key_q = Input.GetKey(KeyCode.Q);
        bool key_r = Input.GetKey(KeyCode.R);

        // Input parsing...
        // ...Special-case cues
        if (key_q) Quit(key_q);

        if (!resetting)
        {
            if (key_r) StartCoroutine(ResetPlayer(key_r));

            // ...Audio cues
            thrustAudio = (key_w || key_a || key_d || key_ua || key_la || key_ra);

            // ...Thrust cues
            if (key_w || key_ua) ActivateThrust(main);
            if (key_a || key_la) { ActivateThrust(port); return; }
            else if (key_d || key_ra) { ActivateThrust(starboard); return; }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Recycler_Active":
                AudioSource.PlayClipAtPoint(bonusSound, transform.position);
                if (hitPoints < BaseHitPoints) hitPoints++;
                rigidbody.mass = 1f;
                other.gameObject.tag = "Recycler_Inactive";
                other.gameObject.SetActive(false);
                score++;
                break;
            case "Finish":
                if (score == 21) SceneManager.LoadScene("Level_02");
                break;
            default:
                Debug.Log("unknown trigger.");
                break;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Obstacle":
                if (collision.relativeVelocity.magnitude > 2)
                {
                    hitPoints--;
                    AudioSource.PlayClipAtPoint(collisionSound, transform.position);
                    if (hitPoints <= 0) StartCoroutine(ResetPlayer(true));
                }
                break;
            case "Ground":
                if (collision.relativeVelocity.magnitude > 2)
                {
                    hitPoints--;
                    AudioSource.PlayClipAtPoint(collisionSound, transform.position);
                    if (hitPoints <= 0) StartCoroutine(ResetPlayer(true));
                }
                break;
            case "Pad":
                Debug.Log("Pad collision.");
                break;
            case "Recycler_Active":
                Debug.Log("Recycler collision.");
                break;
            default:
                Debug.Log("unknown collision.");
                break;
        }
    }

    private static void Quit(bool key_q)
    {
        // TODO: make this Ctrl-Q or Alt-X?
        if (key_q) Application.Quit();
    }

    private IEnumerator ResetPlayer(bool key_r)
    {
        resetting = true;
        playerLives--;
        if (playerLives <= 0)
        {
            SceneManager.LoadScene(0);
        }
        // TODO: make this Ctrl-R?
        if (key_r)
        {
            thrustAudio = false;
            hitPoints = BaseHitPoints;
            rigidbody.isKinematic = true; // Nullify velocity
            transform.position = startPosition;
            transform.rotation = startRotation;
            yield return new WaitForSeconds(1.7f);
            rigidbody.isKinematic = false;
            rigidbody.mass = 1;
        }
        resetting = false;
    }

    private void ProcessAudio()
    {
        var audioOn = audioSource.isPlaying;
        if (!audioOn && thrustAudio) audioSource.Play();
        else if (audioOn && !thrustAudio) audioSource.Stop();
    }

    private void ProcessVisualEffects()
    {
        const int zip = 0;
        const int emissionRate = 15;
        const float sirenMassTrigger = 1.2f;

        // Thrusters
        var em = particleSystem.emission;
        if (thrustAudio) em.rateOverTime = emissionRate;
        else if (!thrustAudio) em.rateOverTime = zip;

        // Siren
        if (rigidbody.mass > sirenMassTrigger || resetting)
        {
            siren_R.SetActive(true);
            siren_B.SetActive(true);
        }
        else
        {
            siren_R.SetActive(false);
            siren_B.SetActive(false);
        }
    }

    private void ActivateThrust(int v)
    {
        const int rcsThrust = 200;
        const int mainThrust = 835;
        float rotationForce = rcsThrust * Time.deltaTime;
        float thrustForce = mainThrust * Time.deltaTime;

        if (v == 0 && !resetting) rigidbody.AddRelativeForce(Vector3.up * thrustForce);
        else if (!resetting)
        { 
            rigidbody.freezeRotation = true;
            transform.Rotate((-v) * Vector3.forward * rotationForce);
            rigidbody.freezeRotation = false;
            rigidbody.constraints = rigidbodyConstraints;
        }
    }
}
