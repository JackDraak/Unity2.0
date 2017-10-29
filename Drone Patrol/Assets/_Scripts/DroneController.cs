using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Priorities:
// TODO: fix GUItext colour issue
// TODO: Game mechanic: something that boosts and/or retards: rcsThrust or mainThrust.
//       Note: ^ Handles in-place: rcsFactor & thrustFactor 
// TODO: Keep working on G's mechanic?
// TODO: use ctrl- or alt- keys for Quit and Respawn.
// TODO: fix lighting issues (i.e. tail not showing until first 3 deaths & reset)
// TODO: allow swap of port/starboard numbers if player wants inverted controls.
// http://www.sharemygame.com/share/f8e87d4e-8198-4326-92fb-c86cbc693f7b

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
    [SerializeField] Text gameTime; // TODO: not in-use at this time
    [SerializeField] Text gees;
    //[SerializeField] Text health; // TODO: only showing 100% or 0%... huh?
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
    private GameObject[] pickups = null;
    private GameObject[] uniquePickups;

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
        // TODO: debug +80 pickups:
        // for (int f = 0; f < pickups.Length; f++) Debug.Log(pickups[f]); // WTF? 5 copies of each?
        // for (int f = 0; f < pickups.Length; f++) AddObjectToUniquePickups(pickups[f]);

        // TODO: new game bug with improved orbs.... this is now broken vvv (WIP)
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

/*    private bool ObjectIsUniquePickup(GameObject obj)
    {
        bool unique = true;
        if (uniquePickups == null) return unique;

        int size = uniquePickups.Length;
        for (int u = 0; u < size; u++)
        {
            if (obj == uniquePickups[u])
            {
                unique = false;
                break;
            }
        }
        return unique;
    }

   private void AddObjectToUniquePickups(GameObject obj)
    {
        int size = 0;
        if (uniquePickups != null) size = uniquePickups.Length;
        if (ObjectIsUniquePickup(obj)) uniquePickups[size] = obj;
    } */

    // Update is called once per frame.
    void Update()
    {
        ProcessInput(); // Keyboard only ATM
        ProcessAudio();
        ProcessVisualEffects();
        UpdateGUI();
        MonitorExit(); // Reveal exit portal when the time is right.
    }

    private void FixedUpdate()
    {
        ProcessMass(); // Drone mass +OverTime
        RotateSirenLamps(); // Faster for higher mass drones: haptics!
    }

    private void UpdateGUI()
    {
        collectibles.material.color = Color.black;
        if (GetCount() == 0) collectibles.text = "Find the Exit Portal!";
        else collectibles.text = GetCount().ToString() + " Orbs remain";

        droneLives.material.color = Color.black;
        int droneStore = (playerLives - 1);
        if (droneStore < 0) droneLives.text = "";
        else if (droneStore == 1) droneLives.text = "1 Stored drone";
        else droneLives.text = droneStore.ToString() + " Stored drones";

        if (myRigidbody.mass < 1.2)
        {
            mass.material.color = Color.black;
            mass.color = Color.black;
        }
        else
        {
            mass.material.color = Color.red;
            mass.color = Color.red;
        }
        float mText = Mathf.Round(myRigidbody.mass * 100f) / 100f;
        mass.text = mText.ToString() + " Drone mass";

        gees.material.color = Color.black;
        float tGees = Mathf.Round(-Physics.gravity.y * 100) /100;
        gees.text = "You Set the G's! {#} at: " + tGees.ToString();

      //  float tText = (Mathf.Round(Time.time) * 10f) / 10f;
      //  gameTime.text = tText.ToString() + " Sec";
      //  float tHealth = Mathf.Round((hitPoints / BaseHitPoints) * 10000f) / 100f;
      //  health.text = tHealth.ToString() + "% Health";
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
        bool key_0 = Input.GetKey(KeyCode.Alpha0);
        bool key_1 = Input.GetKey(KeyCode.Alpha1);
        bool key_2 = Input.GetKey(KeyCode.Alpha2);
        bool key_3 = Input.GetKey(KeyCode.Alpha3);
        bool key_4 = Input.GetKey(KeyCode.Alpha4);
        bool key_5 = Input.GetKey(KeyCode.Alpha5);
        bool key_6 = Input.GetKey(KeyCode.Alpha6);
        bool key_7 = Input.GetKey(KeyCode.Alpha7);
        bool key_8 = Input.GetKey(KeyCode.Alpha8);
        bool key_9 = Input.GetKey(KeyCode.Alpha9);

        // Input parsing...
        if (key_q) Quit(key_q);
        if (key_1 || key_2 || key_3 || key_4 || key_5 || key_6 || key_7 || key_8 || key_9 || key_0)
            ChangeGravity(key_1, key_2, key_3, key_4, key_5, key_6, key_7, key_8, key_9, key_0);
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

    private void ChangeGravity
        (bool k1, bool k2, bool k3, bool k4, bool k5, bool k6, bool k7, bool k8, bool k9, bool k0)
    {
        if (k0) Physics.gravity = new Vector3(0, -9.80665f, 0); // "standard" Earth gravity
        if (k1) Physics.gravity = new Vector3(0, -1f, 0);
        if (k2) Physics.gravity = new Vector3(0, -2f, 0);
        if (k3) Physics.gravity = new Vector3(0, -3f, 0);
        if (k4) Physics.gravity = new Vector3(0, -4f, 0);
        if (k5) Physics.gravity = new Vector3(0, -5f, 0);
        if (k6) Physics.gravity = new Vector3(0, -6f, 0);
        if (k7) Physics.gravity = new Vector3(0, -7f, 0);
        if (k8) Physics.gravity = new Vector3(0, -8f, 0);
        if (k9) Physics.gravity = new Vector3(0, -9f, 0);
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
                myRigidbody.mass = DefaultDroneMass;
                StartCoroutine(ClaimOrb(other));
                break;
            case "Orb":
                other.gameObject.SetActive(false);
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
                // Debug.Log("unknown trigger.");
                break;
        }
    }

    private IEnumerator ClaimOrb(Collider other)
    {
        other.gameObject.tag = "Recycler_Inactive";
        yield return new WaitForSeconds(35);
        other.gameObject.SetActive(false);
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
                // Debug.Log("unknown collision.");
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

    private IEnumerator BlinkMapMarker()
    {
        yield return new WaitForSeconds(0.4f);
        if (mapMarker.activeInHierarchy) mapMarker.SetActive(false);
        else mapMarker.SetActive(true);
        Invoke("BumpBlink", 0.1f);
    }

    private void BumpBlink() { StartCoroutine(BlinkMapMarker()); }
    private void LoadLevelOne() { SceneManager.LoadScene(scene_01); }
    private void LoadLevelTwo() { SceneManager.LoadScene(scene_02); }
    private void Quit(bool key_q) { if (key_q) Application.Quit(); }
    private int GetCount() { return scoreToClear - score; }
}
