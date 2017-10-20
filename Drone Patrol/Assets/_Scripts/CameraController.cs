using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField] GameObject player;
    [SerializeField] float margin = 10; 
    private float offset;
    private Vector3 newPos;

    // Use this for initialization
    void Start () {
        offset = transform.position.x - player.transform.position.x;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // LateUpdate is called after eache Update frame
    void LateUpdate()
    {
        // newPos = new Vector3 (player.transform.position.x + (offset - margin), 
        //    transform.position.y, transform.position.z);

        // transform.position = newPos;

      
        Vector3 velocity = Vector3.zero;
        Vector3 forward = player.transform.forward * 10.0f;
        Vector3 needPos = player.transform.position - forward;
        transform.position = Vector3.SmoothDamp(transform.position, needPos,
                                                ref velocity, 0.05f);
        transform.LookAt(player.transform);
        // transform.rotation = player.transform.rotation;

    }
}
