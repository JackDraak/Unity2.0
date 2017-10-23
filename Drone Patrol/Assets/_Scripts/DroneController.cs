using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Concept: gravity increases over time...... (maybe just increase mass of player-drone?)
 * Touching the landing pads(?) helps normalize gravity....
 * Touch all the pads in the time-limit to save the core......
 * 
 */

public class DroneController : MonoBehaviour {
    [SerializeField] int rcsThrust = 275;
    [SerializeField] int mainThrust = 850;
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] AudioClip collisionSound;
    [SerializeField] AudioClip thrustSound;
    [SerializeField] AudioClip bonusSound;
    Rigidbody rigidbody;
    RigidbodyConstraints rigidbodyConstraints;
    Vector3 startPosition;
    AudioSource audioSource;
    Quaternion startRotation;
    bool thrustAudio;
    bool resetting = false;
    const int BaseHitPoints = 5;
    int hitPoints = BaseHitPoints;

    // Use this for initialization.
    void Start () {
        bool pass;
        pass = (rigidbody = GetComponent<Rigidbody>());
        if (!pass) Debug.Log("FAIL: rigidbody");
        pass = (audioSource = GetComponent<AudioSource>());
        if (!pass) Debug.Log("FAIL: audioSource");
        startPosition = transform.position;
        startRotation = transform.rotation;
        rigidbodyConstraints = rigidbody.constraints;
    }
	
	// Update is called once per frame.
	void Update () {
        ProcessInput();
        ProcessVisualEffects();
        ProcessAudio();
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
        Quit(key_q);

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

    void OnCollisionEnter(Collision collision)
    {
      switch(collision.gameObject.tag)
        {
            case "Pad":
                if (collision.relativeVelocity.magnitude > 1)
                {
                    AudioSource.PlayClipAtPoint(bonusSound, transform.position);
                    if (hitPoints < BaseHitPoints) hitPoints++;
                }
                break;
            case "Obstacle":
                if (collision.relativeVelocity.magnitude > 2)
                {
                    hitPoints--;
                    AudioSource.PlayClipAtPoint(collisionSound, transform.position);
                    if (hitPoints <= 0)
                    {
                        StartCoroutine(ResetPlayer(true));
                        hitPoints = BaseHitPoints;
                    }
                }
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
        // TODO: make this Ctrl-R?
        if (key_r)
        {
            resetting = true;
            thrustAudio = false;
            rigidbody.isKinematic = true; // Nullify velocity
            transform.position = startPosition;
            transform.rotation = startRotation;
            yield return new WaitForSeconds(1.7f);
            rigidbody.isKinematic = false;
            resetting = false;
        }
    }

    private void ProcessAudio()
    {
        var audioOn = audioSource.isPlaying;
        if (!audioOn && thrustAudio) audioSource.Play();
        else if (audioOn && !thrustAudio) audioSource.Stop();
    }

    private void ProcessVisualEffects()
    {
        var em = particleSystem.emission;
        if (thrustAudio) em.rateOverTime = 15;
        else if (!thrustAudio) em.rateOverTime = 0;
    }

    private void ActivateThrust(int v)
    {
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
