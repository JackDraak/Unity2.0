using UnityEngine;

public class GameCameraController : MonoBehaviour {
    [SerializeField] GameObject player;
    [SerializeField] float rDelay = 0.001f;
    private Vector3 playerStartPos;
    private Vector3 startPos;
    private Vector3 newPos;
    private float vFactor = .1f;

    void Start()
    {
        startPos = transform.position;
        playerStartPos = player.transform.position;
    }

    void Update()
    {
        float fc = Time.deltaTime;
        if (Time.deltaTime > (fc + rDelay))
        {
            CheckForReset();
            fc = Time.deltaTime;
            print(fc);
        }
        else fc++;
    }

    private void CheckForReset()
    {
        if (player.transform.position == playerStartPos) Reset();
    }

    private void Reset()
    {
        transform.position = startPos;
    }

    // LateUpdate is called after eache Update frame.
    void LateUpdate()
    {
        Vector3 velocity = Vector3.zero;
        Vector3 forward = player.transform.forward * 10.0f;
        Vector3 needPos = player.transform.position - forward;
        transform.position = Vector3.SmoothDamp(transform.position, needPos, ref velocity, vFactor );
        transform.rotation = Quaternion.Lerp(transform.rotation, player.transform.rotation, 0.03f);
        transform.LookAt(player.transform);
    }
}
