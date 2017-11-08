using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class BensPlayer : MonoBehaviour
{

    [Tooltip("Speed in ms^-1")][SerializeField] float speed = 20f;
    [Tooltip("Range of motion in m")][SerializeField] float xRange = 5f;
    [Tooltip("Range of motion in m")][SerializeField] float yRange = 3f;

    void FixedUpdate()
    {
        float overhead = 0.5f; // special y-offset because of where ship is drawn
        float xThrow = CrossPlatformInputManager.GetAxis("Horizontal");
        float yThrow = CrossPlatformInputManager.GetAxis("Vertical");

        float xOffset = xThrow * speed * Time.deltaTime;
        float yOffset = yThrow * speed * Time.deltaTime;

        float rawXPos = transform.localPosition.x + xOffset;
        float clampedXPos = Mathf.Clamp(rawXPos, -xRange, xRange);

        float rawYPos = transform.localPosition.y + yOffset;
        float clampedYPos = Mathf.Clamp(rawYPos, -yRange, yRange + overhead);

        transform.localPosition = new Vector3(clampedXPos, clampedYPos, transform.localPosition.z);
    }
}
