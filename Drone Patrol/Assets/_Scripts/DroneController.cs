using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// TODO: Game mechanic: something that boosts and/or retards: rcsThrust or mainThrust.
//       Note: ^ Handles in-place: rcsFactor & thrustFactor 
// TODO: use ctrl- or alt- keys for Quit and Respawn.
// TODO: allow swap of port/starboard numbers if player wants inverted controls.
// TODO: fix GUItext colour issue
// TODO: fix lighting issues (i.e. tail not showing until first 3 deaths & reset)
// http://www.sharemygame.com/share/f5018b3f-52f6-4708-bd10-d3d0f3b0203f

public class DroneController : MonoBehaviour
{
    // Data
    private const float ExplosionDelay = 2.222f;
    private const float FinishDelay = 2.075f;
    private const float StartDelay = 2.633f;
    private const int BaseHitPoints = 5;
    private const int CollisionVelocityThreshold = 2;
    private const int DefaultDroneMass = 1;
    private const int MaxPlayerLives = 3;
    private const int StandardDelay = 1;
    private string scene_01 = "Level_01";
    private string scene_02 = "Level_02";

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
    [SerializeField] Text collectibles;
    [SerializeField] Text droneLives;
    [SerializeField] Text gameTime;
    [SerializeField] Text health;
    [SerializeField] Text mass;


    // Member variables
    [Range(-0.5f, 2f)] private float rcsFactor = 1;
    [Range(-0.5f, 2f)] private float thrustFactor = 1;
    private AudioSource audioSource;
    private GameObject finish = null;
    private Quaternion startRotation;
    private Rigidbody myRigidbody;
    private Vector3 startPosition;
    private RigidbodyConstraints rigidbodyConstraints;
    private bool thrustAudio;
    private int hitPoints;
    private int playerLives;
    private int score;
    private int scoreToClear;
    private enum State { Dead, Alive, Dying, Resetting, Transcending }
    private State thisState;
    private GameObject[] pickups; 

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

        // Collectibles
        pickups = GameObject.FindGameObjectsWithTag("Recycler_Active");
        scoreToClear = pickups.Length;

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
        ProcessInput(); // Keyboard only ATM
        ProcessMass(); // Drone mass +OverTime
        RotateSirenLamps(); // Faster for higher mass drones: haptics!
        ProcessAudio();
        ProcessVisualEffects();
        UpdateGUI();
        MonitorExit(); // Reveal exit portal when the time is right.
    }

    private void UpdateGUI()
    {
        if (GetCount() == 0) collectibles.text = "Find the Exit Portal!";
        else collectibles.text = GetCount().ToString() + " Orbs remain";

        int droneStore = (playerLives - 1);
        if (droneStore < 0) droneLives.text = "";
        else if (droneStore == 1) droneLives.text = "1 Stored drone";
        else droneLives.text = droneStore.ToString() + " Stored drones";

        if (myRigidbody.mass < 1.2) mass.material.color = Color.black;
        else mass.material.color = Color.red;
        float mText = Mathf.Round(myRigidbody.mass * 100f) / 100f;
        mass.text = mText.ToString() + " Drone mass";

      //  float tText = (Mathf.Round(Time.time) * 10f) / 10f;
      //  gameTime.text = tText.ToString() + " Sec";

      //  float tHealth = Mathf.Round((hitPoints / BaseHitPoints) * 10000f) / 100f;
      //  health.text = tHealth.ToString() + "% Health";
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
                if (score >= scoreToClear)
                {
                    thisState = State.Transcending;
                    myRigidbody.isKinematic = true;
                    AudioSource.PlayClipAtPoint(finishSound, transform.position);
                    transform.position = 
                        other.gameObject.transform.position 
                        + new Vector3(0f,-0.2f,-0.3f);
                    Invoke("LoadLevelTwo", FinishDelay);
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
                yield return new WaitForSeconds(StartDelay);
            }
            else if (thisState == State.Dying)
            {
                droneBody.SetActive(false);
                explosion.SetActive(true);
                playerLives--;
                AudioSource.PlayClipAtPoint(explosionSound, transform.position);
                yield return new WaitForSeconds(ExplosionDelay);
                if (playerLives <= 0)
                {
                    Invoke("LoadLevelOne", StandardDelay);
                    yield return new WaitForSeconds(StandardDelay);
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

    private void Warp()
    {
        const int wrFactor = 360;
        const int wsFactor = 90;

        transform.localScale = 
            transform.localScale - 
            ((transform.localScale / 100f) * wsFactor * Time.deltaTime);

        transform.Rotate(Vector3.back * wrFactor * Time.deltaTime);
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

    private void MonitorExit()
    {
        if (score >= scoreToClear && !finish.activeSelf) finish.SetActive(true);
        if (thisState == State.Transcending) Warp();
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
        const int mainThrust = 925;
        float rotationForce = rcsThrust * Time.deltaTime * rcsFactor;
        float thrustForce = mainThrust * Time.deltaTime * thrustFactor;

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

    private void LoadLevelOne() { SceneManager.LoadScene(scene_01); }
    private void LoadLevelTwo() { SceneManager.LoadScene(scene_02); }
    private void Quit(bool key_q) { if (key_q) Application.Quit(); }
    private int GetCount() { return scoreToClear - score; }
}
