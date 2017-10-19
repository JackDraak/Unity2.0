using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        ProcessInput();
	}

    private void ProcessInput()
    {
        if (Input.GetKey(KeyCode.W)) Thrust(0); // Main thrust
        if (Input.GetKey(KeyCode.A)) { Thrust(-1); return; } // Port thrust
        else if (Input.GetKey(KeyCode.D)) { Thrust(1); return; } // Starboard thrust
        return;
    }

    private void Thrust(int v)
    {
        switch(v)
        {
            case -1: // Port
                Debug.Log("Port");
                break;
            case 0: // Main
                Debug.Log("Main");
                break;
            case 1: // Starboard
                Debug.Log("Starboard");
                break;
            default:
                throw new NotImplementedException();
        }
    }
}
