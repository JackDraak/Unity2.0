using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour {
    [SerializeField] int rcsThrust = 275;
    [SerializeField] int mainThrust = 850;
    [SerializeField] ParticleSystem particleSystem;
    Rigidbody rigidbody;
    AudioSource audioSource;
    Vector3 startPosition;
    Quaternion startRotation;
    bool thrustAudio;

    // Use this for initialization.
    void Start () {
        startPosition = transform.position;
        startRotation = transform.rotation;
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
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
        // TODO: allow swap port/starboard numbers if player wants inverted controls.
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
        bool key_lc = Input.GetKey(KeyCode.LeftControl);

    // Input parsing...
        // ...Special-case cues
        Quit(key_q);
        ResetPlayer(key_r);

        // ...Audio cues
        thrustAudio = (key_w || key_a || key_d || key_ua || key_la || key_ra);

        // ...Thrust cues
        if (key_w || key_ua) ActivateThrust(main);
        if (key_a || key_la) { ActivateThrust(port); return; }
        else if (key_d || key_ra) { ActivateThrust(starboard); return; }
    }

    private static void Quit(bool key_q)
    {
        // TODO: make this Ctrl-Q or Alt-X?
        if (key_q) Application.Quit(); 
    }

    private void ResetPlayer(bool key_r)
    {
        // TODO: make this Ctrl-R?
        if (key_r)
        {
            rigidbody.isKinematic = true; // Nullify velocity
            transform.position = startPosition;
            transform.rotation = startRotation;
            rigidbody.isKinematic = false; // Re-enable physics
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
        if (thrustAudio && particleSystem.isStopped) particleSystem.Play();
        else if (particleSystem.isPlaying && !thrustAudio) particleSystem.Stop();
    }

    private void ActivateThrust(int v)
    {
        float rotationForce = rcsThrust * Time.deltaTime;
        float thrustForce = mainThrust * Time.deltaTime;

        if (v == 0) rigidbody.AddRelativeForce(Vector3.up * thrustForce);
        else
        {
            rigidbody.freezeRotation = true;
            transform.Rotate((-v) * Vector3.forward * rotationForce);
            rigidbody.freezeRotation = false;
        }
    }
}
