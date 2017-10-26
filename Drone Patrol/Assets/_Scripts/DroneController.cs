using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// TODO: Game mechanic: something that boosts and/or retards: rcsThrust or mainThrust.
// TODO: Fix lighting issue caused by large ground-plane and background-plane..
// TODO: Create GUI to display score, hitpoints, etc...
// TODO: use ctrl- or alt- keys for Quit and Respawn.
// TODO: allow swap of port/starboard numbers if player wants inverted controls.

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
    [SerializeField] ParticleSystem thrustSmoke;
    [SerializeField] GameObject droneBody;
    [SerializeField] GameObject explosion;
    [SerializeField] GameObject mapMarker;
    [SerializeField] GameObject siren_B;
    [SerializeField] GameObject siren_R;

    // Member variables
    private AudioSource audioSource;
    private GameObject finish = null;
    private Quaternion startRotation;
    private Rigidbody myRigidbody;
    private Vector3 startPosition;
    private RigidbodyConstraints rigidbodyConstraints;
    private bool thrustAudio;
    private enum State { Dead, Alive, Dying, Resetting, Transcending }
    private int hitPoints;
    private int playerLives;
    private int score;
    private State thisState;

    // Use this for initialization.
    void Start()
    {
        // Components
        Debug.Log("Start() @ " + Time.time);
        bool success;
        success = (myRigidbody = GetComponent<Rigidbody>());
            if (!success) Debug.Log("FAIL: rigidbody");
        success = (audioSource = GetComponent<AudioSource>());
            if (!success) Debug.Log("FAIL: audioSource");
        if (GameObject.FindWithTag("Finish") != null) finish = GameObject.FindWithTag("Finish");
        else Debug.Log("FAIL: 'Finish' object not found");

        // Data
        hitPoints = BaseHitPoints;
        playerLives = MaxPlayerLives;
        startPosition = transform.position;
        startRotation = transform.rotation;
        rigidbodyConstraints = myRigidbody.constraints;
        score = 0; 

        // Set state & begin
        thisState = State.Resetting;
        StartCoroutine(ResetPlayer(true));
        StartCoroutine(BlinkMapMarker());
        if (finish != null) finish.SetActive(false);
    }

    // Update is called once per frame.
    void Update()
    {
        ProcessInput(); // Keyboard only
        ProcessMass(); // Drone mass +OT
        ProcessAudio();
        ProcessVisualEffects();
        RotateSirenLamps(); // Faster for higher mass drones
        if (score >= 21 && !finish.activeSelf) finish.SetActive(true);
    }

    private IEnumerator BlinkMapMarker()
    {
        yield return new WaitForSeconds(0.4f);
        if (mapMarker.activeInHierarchy) mapMarker.SetActive(false);
        else mapMarker.SetActive(true);
        Invoke("BumpBlink", 0.1f);
    }

    private void BumpBlink()
    {
        StartCoroutine(BlinkMapMarker());
    }

    private void ProcessInput()
    {
        // Data
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

    void OnTriggerEnter(Collider other)
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
                myRigidbody.mass = DefaultDroneMass;
                break;
            case "Finish":
                if (score >= 21)
                {
                    thisState = State.Transcending;
                    myRigidbody.isKinematic = true;
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
                if (collision.relativeVelocity.magnitude > CollisionVelocityThreshold)
                    AudioSource.PlayClipAtPoint(collisionSound, transform.position);
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
        if (key_r)
        {
            hitPoints = BaseHitPoints;
            thrustAudio = false;
            myRigidbody.freezeRotation = true;
            myRigidbody.isKinematic = true;
            if (thisState == State.Resetting)
            {
                transform.position = startPosition;
                transform.rotation = startRotation;
                AudioSource.PlayClipAtPoint(startSound, transform.position);
                yield return new WaitForSeconds(2.633f);
            }
            else if (thisState == State.Dying)
            {
                droneBody.SetActive(false);
                explosion.SetActive(true);
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
                explosion.SetActive(false);
                droneBody.SetActive(true);
            }
            else
            {
                Debug.Log("ResetPlayer(true) called but noy dying or resetting.");
            }
            myRigidbody.mass = DefaultDroneMass;
            myRigidbody.isKinematic = false;
            myRigidbody.freezeRotation = false;
            myRigidbody.constraints = rigidbodyConstraints;
            thisState = State.Alive;
        }
    }

    private void LoadLevelOne() { SceneManager.LoadScene("Level_01"); }

    private void LoadLevelTwo() { SceneManager.LoadScene("Level_02"); }

    private static void Quit(bool key_q)
    {
        if (key_q) Application.Quit();
    }

    private void RotateSirenLamps()
    {
        float sirenRotationBase = 2.2f;
        float sirenRotationFactor = 0.7f;
        float sirenSpeed = 120f;

        // Note: Vector3.up/down are shorthand for y+/y-
        Transform rSiren = siren_R.transform;
        rSiren.Rotate
            (Vector3.up
            * Time.deltaTime
            * sirenSpeed
            * Mathf.Pow(sirenRotationBase, sirenRotationFactor + myRigidbody.mass));

        Transform bSiren = siren_B.transform;
        bSiren.Rotate
            (Vector3.down
            * Time.deltaTime
            * sirenSpeed
            * Mathf.Pow(sirenRotationBase, sirenRotationFactor + myRigidbody.mass));
    }

    private void ProcessMass()
    {
        float driftMassFactor = 70f;
        float driftMass = Time.deltaTime / driftMassFactor;

        myRigidbody.mass += driftMass;
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
        var em = thrustSmoke.emission;
        if (thrustAudio) em.rateOverTime = emissionRate;
        else if (!thrustAudio) em.rateOverTime = 0;

        // Siren-light
        if (myRigidbody.mass > sirenMassTrigger || thisState == State.Resetting)
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
        const int rcsThrust = 200;
        const int mainThrust = 835;
        float rotationForce = rcsThrust * Time.deltaTime;
        float thrustForce = mainThrust * Time.deltaTime;

        // Thrust and/or rotation activation
        if (v == 0 && thisState == State.Alive)
            myRigidbody.AddRelativeForce(Vector3.up * thrustForce);
        else if (thisState == State.Alive)
        { 
            myRigidbody.freezeRotation = true;
            transform.Rotate((-v) * Vector3.forward * rotationForce);
            myRigidbody.freezeRotation = false;
            myRigidbody.constraints = rigidbodyConstraints;
        }
    }
}
