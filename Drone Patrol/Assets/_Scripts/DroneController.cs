using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// NOTE: no sound when player touches launchpad (NAB).
// NOTE: need a death animation | explosion or the like to complement audio.

public class DroneController : MonoBehaviour
{
    // Data
    private const int BaseHitPoints = 5;
    private const int CollisionVelocityThreshold = 2;
    private const int MaxPlayerLives = 3;
    private const int DefaultDroneMass = 1;

    // Sound
    [SerializeField] AudioClip bonusSound;
    [SerializeField] AudioClip collisionSound;
    [SerializeField] AudioClip explosionSound;
    [SerializeField] AudioClip finishSound;
    [SerializeField] AudioClip startSound;
    [SerializeField] AudioClip thrustSound;

    // Effects
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] GameObject siren_B;
    [SerializeField] GameObject siren_R;

    // Member variables
    private AudioSource audioSource;
    private Quaternion startRotation;
    private Rigidbody rigidbody;
    private Vector3 startPosition;
    private RigidbodyConstraints rigidbodyConstraints;
    private enum State { Dead, Alive, Dying, Resetting, Transcending }
    State thisState;
    private bool thrustAudio;
    private int hitPoints;
    private int playerLives;
    private int score;

    // Use this for initialization.
    void Start()
    {
        // Components
        Debug.Log("Start() @ " + Time.time);
        bool pass;
        pass = (rigidbody = GetComponent<Rigidbody>());
            if (!pass) Debug.Log("FAIL: rigidbody");
        pass = (audioSource = GetComponent<AudioSource>());
            if (!pass) Debug.Log("FAIL: audioSource");

        // Data
        hitPoints = BaseHitPoints;
        playerLives = MaxPlayerLives;
        startPosition = transform.position;
        startRotation = transform.rotation;
        rigidbodyConstraints = rigidbody.constraints;
        score = 0; // TODO: Create GUI to display score, hitpoints, etc...

        // Set state & begin
        thisState = State.Resetting;
        StartCoroutine(ResetPlayer(true));
    }

    // Update is called once per frame.
    void Update()
    {
        ProcessInput(); // Keyboard only
        ProcessMass(); // Drone mass +OT
        ProcessAudio();
        ProcessVisualEffects();
        RotateSirenLamps(); // Faster for higher mass drones
    }

    private void ProcessInput()
    {
        // Data
        // TODO: allow swap of port/starboard numbers if player wants inverted controls.
        int port = -1;
        int starboard = 1;
        const int main = 0;

        // Inputs we care about
        bool key_a = Input.GetKey(KeyCode.A);
        bool key_d = Input.GetKey(KeyCode.D);
        bool key_q = Input.GetKey(KeyCode.Q);
        bool key_r = Input.GetKey(KeyCode.R);
        bool key_w = Input.GetKey(KeyCode.W);
        bool key_la = Input.GetKey(KeyCode.LeftArrow);
        bool key_ra = Input.GetKey(KeyCode.RightArrow);
        bool key_ua = Input.GetKey(KeyCode.UpArrow);

        // Input parsing...
        // ...Special-case cues
        if (key_q) Quit(key_q);

        if (thisState == State.Alive)
        {
            if (key_r)
            {
                thisState = State.Resetting;
                StartCoroutine(ResetPlayer(key_r));
            }

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
                score++;
                Debug.Log("Score: " + other + " : " + score);
                AudioSource.PlayClipAtPoint(bonusSound, transform.position);
                if (hitPoints < BaseHitPoints) hitPoints++;
                other.gameObject.tag = "Recycler_Inactive";
                other.gameObject.SetActive(false);
                rigidbody.mass = DefaultDroneMass;
                break;
            case "Finish":
                if (score >= 21)
                {
                    thisState = State.Transcending;
                    rigidbody.isKinematic = true;
                    AudioSource.PlayClipAtPoint(finishSound, transform.position);
                    Invoke("LoadLevelTwo", 2.075f);
                }
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
                if (collision.relativeVelocity.magnitude > CollisionVelocityThreshold)
                {
                    hitPoints--;
                    AudioSource.PlayClipAtPoint(collisionSound, transform.position);
                    if (hitPoints <= 0)
                    {
                        thisState = State.Dying;
                        StartCoroutine(ResetPlayer(true));
                    }
                }
                break;
            case "Ground":
                if (collision.relativeVelocity.magnitude > CollisionVelocityThreshold)
                {
                    hitPoints--;
                    AudioSource.PlayClipAtPoint(collisionSound, transform.position);
                    if (hitPoints <= 0)
                    {
                        thisState = State.Dying;
                        StartCoroutine(ResetPlayer(true));
                    }
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

    private IEnumerator ResetPlayer(bool key_r)
    {
        // TODO: make this Ctrl-R?
        if (key_r)
        {
            hitPoints = BaseHitPoints;
            thrustAudio = false;
            rigidbody.freezeRotation = true;
            rigidbody.isKinematic = true; // Nullify velocity
            if (thisState == State.Resetting)
            {
                transform.position = startPosition;
                transform.rotation = startRotation;
                AudioSource.PlayClipAtPoint(startSound, transform.position);
                yield return new WaitForSeconds(2.633f);
            }
            else if (thisState == State.Dying)
            {
                playerLives--;
                AudioSource.PlayClipAtPoint(explosionSound, transform.position);
                yield return new WaitForSeconds(2.222f);
                if (playerLives <= 0)
                {
                    Invoke("LoadLevelOne", 1f);
                    yield return new WaitForSeconds(1f);
                }
                transform.position = startPosition;
                transform.rotation = startRotation;
            }
            else
            {
                Debug.Log("ResetPlayer(true) called but noy dying or resetting.");
            }
            rigidbody.mass = DefaultDroneMass;
            rigidbody.isKinematic = false;
            rigidbody.freezeRotation = false;
            rigidbody.constraints = rigidbodyConstraints;
            thisState = State.Alive;
        }
    }

    private void LoadLevelOne() { SceneManager.LoadScene("Level_01"); }

    private void LoadLevelTwo() { SceneManager.LoadScene("Level_02"); }

    private static void Quit(bool key_q)
    {
        // TODO: make this Ctrl-Q or Alt-X?
        if (key_q) Application.Quit();
    }

    private void RotateSirenLamps()
    {
        float sirenRotationBase = 2f;
        float sirenRotationFactor = 0.5f;
        float sirenSpeed = 200f;

        // Note: Vector3.up/down are shorthand for y+/y-
        Transform rSiren = siren_R.transform;
        rSiren.Rotate
            (Vector3.up
            * Time.deltaTime
            * sirenSpeed
            * Mathf.Pow(sirenRotationBase, sirenRotationFactor + rigidbody.mass));

        Transform bSiren = siren_B.transform;
        bSiren.Rotate
            (Vector3.down
            * Time.deltaTime
            * sirenSpeed
            * Mathf.Pow(sirenRotationBase, sirenRotationFactor + rigidbody.mass));
    }

    private void ProcessMass()
    {
        float driftMassFactor = 70f;
        float driftMass = Time.deltaTime / driftMassFactor;

        rigidbody.mass += driftMass;
    }

    private void ProcessAudio()
    {
        var audioOn = audioSource.isPlaying;

        if (!audioOn && thrustAudio) audioSource.PlayOneShot(thrustSound);
        else if (audioOn && !thrustAudio) audioSource.Stop();
    }

    private void ProcessVisualEffects()
    {
        // Data
        const int emissionRate = 15;
        const float sirenMassTrigger = 1.2f;

        // Thruster particles
        var em = particleSystem.emission;
        if (thrustAudio) em.rateOverTime = emissionRate;
        else if (!thrustAudio) em.rateOverTime = 0;

        // Siren-light
        if (rigidbody.mass > sirenMassTrigger || thisState == State.Resetting)
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
        // Data & calculations
        const int rcsThrust = 200; // TODO: engineer things to effect this +/-
        const int mainThrust = 835; // TODO: engineer things to effect this +/-
        float rotationForce = rcsThrust * Time.deltaTime;
        float thrustForce = mainThrust * Time.deltaTime;

        // Thrust and/or rotation activation
        if (v == 0 && thisState == State.Alive)
            rigidbody.AddRelativeForce(Vector3.up * thrustForce);
        else if (thisState == State.Alive)
        { 
            rigidbody.freezeRotation = true;
            transform.Rotate((-v) * Vector3.forward * rotationForce);
            rigidbody.freezeRotation = false;
            rigidbody.constraints = rigidbodyConstraints;
        }
    }
}
