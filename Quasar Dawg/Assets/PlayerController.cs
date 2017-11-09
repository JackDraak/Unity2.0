using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour {
    [Range(1, 100000)][SerializeField] float sFactor = 20000f;
    private bool alive = true;
    private float delta;
    private float inputX; 
    private float inputY;
    private Rigidbody rigidbody;

    void Start()        { rigidbody = GetComponent<Rigidbody>(); delta = Time.fixedDeltaTime; }
    void Update()       { if (alive) PollControls(); }
    void FixedUpdate()  { UpdatePlayerPosition(); }

    void UpdatePlayerPosition()
    {
        // TODO: ship needs to be glued to camera by distance. Bonus points, tilt around corners!
        if ((inputY == 0 && inputX == 0) || !alive) return;
        if (inputX != 0) 
            rigidbody.AddRelativeForce(Vector3.right * inputX * delta * sFactor);
        if (inputY != 0) 
            rigidbody.AddRelativeForce(Vector3.up * inputY * delta * sFactor);
    }

    private void PollControls()
    {
        inputX = CrossPlatformInputManager.GetAxis("Horizontal"); 
        inputY = CrossPlatformInputManager.GetAxis("Vertical");
    }
}
