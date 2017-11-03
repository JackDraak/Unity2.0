using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Features:
// TODO: fix GUItext colour issue
// TODO: Game mechanic: something that boosts and/or retards: rcsThrust or mainThrust.
//       Note: ^ Handles in-place: rcsFactor & thrustFactor 
// TODO: use ctrl- or alt- keys for Quit and Respawn.

// Bugs
// TODO: fix lighting issues (i.e. tail not showing until first 3 deaths & reset)
// TODO: allow swap of port/starboard numbers if player wants inverted controls.

// Demo @ https://jackdraak.itch.io/

public class DroneController : MonoBehaviour
{
    // Data
    private const float ExplosionDelay = 2.222f;
    private const float FinishDelay = 2.075f;
    private const float StartDelay = 2.633f;
    private const int BaseHitPoints = 6;
    private const int CollisionVelocityThreshold = 2;
    private const int DefaultDroneMass = 1;
    private const int FinalLevelIndex = 5;
    private const int FirstLevelIndex = 2;
    private const int MaxPlayerLives = 3;
    private const int StandardDelay = 1;
    private string scene_01 = "_StartMenu_DP";

    // Sound
    [SerializeField] AudioClip bonusSound;
    [SerializeField] AudioClip collisionSound;
    [SerializeField] AudioClip explosionSound;
    [SerializeField] AudioClip finishSound;
    [SerializeField] AudioClip startSound;
    [SerializeField] AudioClip thrustSound;

    // Effects
    [SerializeField] GameObject droneBody;
    [SerializeField] GameObject explosion;
    [SerializeField] GameObject mapMarker;
    [SerializeField] GameObject siren_B;
    [SerializeField] GameObject siren_R;
    [SerializeField] ParticleSystem thrustSmoke;

    // Gui Texts
    [SerializeField] Text collectibles;
    [SerializeField] Text droneLives;
    [SerializeField] Text gees;
    [SerializeField] Text mass;
    [SerializeField] Text invincible;
    [SerializeField] Text gameTime; // TODO: not in-use at this time
    //[SerializeField] Text health; // TODO: only showing 100% or 0%... huh?
 
    // Member variables
    [Range(-0.5f, 2f)] private float rcsFactor = 1;
    [Range(-0.5f, 2f)] private float thrustFactor = 1;
    private AudioSource audioSource;
    private GameObject finish = null;
    private Quaternion startRotation;
    private Rigidbody myRigidbody;
    private Vector3 startPosition;
    private RigidbodyConstraints rigidbodyConstraints;
    private bool debugMode;
    private bool debugInvulnerable = false;
    private bool doOnce = false;
    private bool finishHaptic = true;
    private bool thrustAudio = false;
    private int hitPoints;
    private int playerLives;
    private int score;
    private int scoreToClear;
    private enum State { Dead, Alive, Dying, Resetting, Transcending }
    private State thisState;
    private GameObject[] pickups = null;
    private GameObject[] uniquePickups;

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
        playerLives = MaxPlayerLives; // TODO: put in a LevelManager() to track user data between levels
        startPosition = transform.position;
        startRotation = transform.rotation;
        rigidbodyConstraints = myRigidbody.constraints;
        score = 0;

        // Set state & begin
        thisState = State.Resetting;
        StartCoroutine(ResetPlayer(true));
        StartCoroutine(BlinkMapMarker());
        if (finish != null) finish.SetActive(false);

        debugMode = Debug.isDebugBuild;
    }

    void Update()
    {
        ProcessAudio();
        ProcessVisualEffects();
        UpdateGUI();
    }

    private void FixedUpdate()
    {
        MonitorExit(); // Reveal exit portal when the time is right
        ProcessInput(); // Keyboard only ATM
        ProcessMass(); // Drone mass +OverTime
        RotateSirenLamps(); // Faster for higher mass drones: haptics!
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
        bool key_x = Input.GetKey(KeyCode.X);
        bool key_w = Input.GetKey(KeyCode.W);
        bool key_la = Input.GetKey(KeyCode.LeftArrow);
        bool key_ra = Input.GetKey(KeyCode.RightArrow);
        bool key_ua = Input.GetKey(KeyCode.UpArrow);
        bool key_lb = Input.GetKeyDown(KeyCode.LeftBracket);
        bool key_rb = Input.GetKeyDown(KeyCode.RightBracket);


        // Input parsing... parse any time:
        if (key_x) Quit(key_x);
        if (key_lb || key_rb)
            ChangeGravity(key_lb, key_rb);
        // ...parse these inputs only in .Alive State:
        if (thisState == State.Alive)
        {
            // ...Audio cues
            thrustAudio = (key_w || key_a || key_d || key_ua || key_la || key_ra);

            // ...Thrust cues
            if (key_w || key_ua) ActivateThrust(main);
            if (key_a || key_la) { ActivateThrust(port); return; }
            else if (key_d || key_ra) { ActivateThrust(starboard); return; }

            if (debugMode) ProcessDeveloperInput();
        }
    }

    void ProcessDeveloperInput()
    {
        bool key_i = Input.GetKeyDown(KeyCode.I);
        bool key_l = Input.GetKeyDown(KeyCode.L);
        bool key_o = Input.GetKeyDown(KeyCode.O);
        bool key_r = Input.GetKeyDown(KeyCode.R);

        if (key_o) LoadNextLevel();
        if (key_l) playerLives++;
        if (key_i)
        {
            debugInvulnerable = !debugInvulnerable;
            if (debugInvulnerable)
            {
                invincible.gameObject.SetActive(true);
                invincible.text = "I'm Invincible!";
            }

            else
            {
                invincible.gameObject.SetActive(false);
                invincible.text = "";
            }
        }
        if (key_r)
        {
            thisState = State.Resetting;
            StartCoroutine(ResetPlayer(key_r));
        }
    }

    void OnTriggerEnter(Collider trigger)
    {
        switch (trigger.gameObject.tag)
        {
            case "Recycler_Active":
                score++;
                Debug.Log("Score: " + trigger + " : " + score);
                AudioSource.PlayClipAtPoint(bonusSound, transform.position, 0.66f); // TODO setup volume control preferences
                if (hitPoints < BaseHitPoints) hitPoints++;
                myRigidbody.mass = DefaultDroneMass;
                StartCoroutine(ClaimOrb(trigger));
                break;
            case "Orb":
                trigger.gameObject.SetActive(false);
                break;
            case "Finish":
                if (score >= scoreToClear)
                {
                    if (!doOnce)
                    {
                        thisState = State.Transcending;
                        myRigidbody.isKinematic = true;
                        AudioSource.PlayClipAtPoint(finishSound, transform.position, 0.66f);
                        transform.position = 
                            trigger.gameObject.transform.position 
                            + new Vector3(0f,-0.2f,-0.3f);
                        Invoke("LoadNextLevel", FinishDelay);
                        doOnce = true;
                    }
                }
                break;
            default:
                // Debug.Log("Drone - Trigger - Default - " + trigger);
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
                    if (!debugInvulnerable) hitPoints--;
                    AudioSource.PlayClipAtPoint(collisionSound, transform.position, 0.66f);
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
                    if (!debugInvulnerable) hitPoints--;
                    AudioSource.PlayClipAtPoint(collisionSound, transform.position, 0.66f);
                    if (hitPoints <= 0)
                    {
                        thisState = State.Dying;
                        StartCoroutine(ResetPlayer(true));
                    }
                }
                break;
            case "Pad":
                if (collision.relativeVelocity.magnitude > CollisionVelocityThreshold)
                    AudioSource.PlayClipAtPoint(collisionSound, transform.position, 0.66f);
                break;
            case "Recycler_Active":
                // Debug.Log("Drone - Recycler collision - " + collision);
                break;
            default:
                 // Debug.Log("Drone - Collision - Default - " + collision);
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
                AudioSource.PlayClipAtPoint(startSound, transform.position, 0.66f);
                yield return new WaitForSeconds(StartDelay);
            }
            else if (thisState == State.Dying)
            {
                droneBody.SetActive(false);
                explosion.SetActive(true);
                playerLives--;
                AudioSource.PlayClipAtPoint(explosionSound, transform.position, 0.66f);
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
                Debug.Log("ResetPlayer(true) called but not dying or resetting.");
            }
            myRigidbody.mass = DefaultDroneMass;
            myRigidbody.isKinematic = false;
            myRigidbody.freezeRotation = false;
            myRigidbody.constraints = rigidbodyConstraints;
            thisState = State.Alive;
        }
    }

    private void UpdateGUI()
    {
        bool finished = false;
        if (GetCount() == 0 && !finished)
        {
            finished = true;
            collectibles.text = "Find the Exit Portal!";
            StartCoroutine(ShowFinish());
        }
        else collectibles.text = GetCount().ToString() + " Orbs remain";

        int droneStore = (playerLives - 1);
        if (droneStore < 0) droneLives.text = "";
        else if (droneStore == 1) droneLives.text = "1 Stored drone";
        else droneLives.text = droneStore.ToString() + " Stored drones";

        if (myRigidbody.mass < 1.2) mass.color = Color.white;
        else mass.color = Color.red;

        float mText = Mathf.Round(myRigidbody.mass * 100f) / 100f;
        mass.text = mText.ToString() + " Drone mass";

        float tGees = Mathf.Round(-Physics.gravity.y * 100) / 100;
        gees.text = "You Set the G's at: " + tGees.ToString() + " m/s²";

        // TODO: fix this to get health as %.... right now, only results in states of 0% or 100%
        //  float tText = (Mathf.Round(Time.time) * 10f) / 10f;
        //  gameTime.text = tText.ToString() + " Sec";
        //  float tHealth = Mathf.Round((hitPoints / BaseHitPoints) * 10000f) / 100f;
        //  health.text = tHealth.ToString() + "% Health";
    }

    private void ProcessMass()
    {
        float driftMassFactor = 70f;
        float driftMass = Time.fixedDeltaTime / driftMassFactor;

        myRigidbody.mass += driftMass;
    }

    private void ProcessAudio()
    {
        var audioOn = audioSource.isPlaying;

        if (!audioOn && thrustAudio) audioSource.PlayOneShot(thrustSound, 0.66f);
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

    private void ChangeGravity
        (bool k1, bool k2)
    {
        Vector3 myG = Physics.gravity; 
        if (k1) myG += new Vector3(0, 1f, 0);
        if (k2) myG -= new Vector3(0, 1f, 0);
        if (myG.y < -10) myG = new Vector3(0, -10, 0);
        if (myG.y > 10) myG = new Vector3(0, 10, 0);
        Physics.gravity = myG;
        //if (k0) Physics.gravity = new Vector3(0, -9.80665f, 0); // "standard" Earth gravity
    }

    private void RotateSirenLamps()
    {
        float sirenRotationBase = 2.2f;
        float sirenRotationFactor = 0.7f;
        float sirenSpeed = 120f;
        float massFactor = myRigidbody.mass;
        if (massFactor > 2.5f) massFactor = 2.5f;

        // Note: Vector3.up/down are shorthand for y+/y-
        Transform rSiren = siren_R.transform;
        rSiren.Rotate
            (Vector3.up
            * Time.fixedDeltaTime
            * sirenSpeed
            * Mathf.Pow(sirenRotationBase, sirenRotationFactor + massFactor));

        Transform bSiren = siren_B.transform;
        bSiren.Rotate
            (Vector3.down
            * Time.fixedDeltaTime
            * sirenSpeed
            * Mathf.Pow(sirenRotationBase, sirenRotationFactor + massFactor));
    }

    private void Warp()
    {
        const int wrFactor = 360;
        const int wsFactor = 90;

        transform.localScale =
            transform.localScale -
            ((transform.localScale / 100f) * wsFactor * Time.fixedDeltaTime);

        transform.Rotate(Vector3.back * wrFactor * Time.fixedDeltaTime);
    }

    private void MonitorExit()
    {
        if (score >= scoreToClear && !finish.activeSelf) finish.SetActive(true);
        if (thisState == State.Transcending) Warp();
    }

    private void ActivateThrust(int v)
    {
        // Data & calculations
        const int rcsThrust = 200;
        const int mainThrust = 925;
        float rotationForce = rcsThrust * Time.fixedDeltaTime * rcsFactor;
        float thrustForce = mainThrust * Time.fixedDeltaTime * thrustFactor;

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

    private IEnumerator ClaimOrb(Collider other)
    {
        other.gameObject.tag = "Recycler_Inactive";
        yield return new WaitForSeconds(35);
        other.gameObject.SetActive(false);
    }

    private IEnumerator ShowFinish()
    {
        if (finishHaptic) collectibles.color = Color.green;
        else collectibles.color = Color.white;
        finishHaptic = !finishHaptic;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(ShowFinish());
    }

    private void LoadNextLevel()
    {
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        // If player just finished the "last level", load any random level
        if (nextScene > FinalLevelIndex) nextScene = 
                Random.Range(FirstLevelIndex, FinalLevelIndex + 1);

        SceneManager.LoadScene(nextScene);
    }

    private IEnumerator BlinkMapMarker()
    {
        yield return new WaitForSeconds(0.4f);
        if (mapMarker.activeInHierarchy) mapMarker.SetActive(false);
        else mapMarker.SetActive(true);
        Invoke("BumpBlink", 0.1f);
    }

    private void BumpBlink() { StartCoroutine(BlinkMapMarker()); }
    private void LoadLevelOne() { SceneManager.LoadScene(scene_01); }
    private void Quit(bool key_q) { if (key_q) Application.Quit(); }
    private int GetCount() { return scoreToClear - score; }
    
}
