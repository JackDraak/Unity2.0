using UnityEngine;

public class GameCameraController : MonoBehaviour {
    [SerializeField] GameObject player;
    private const float vFactor = 0.1f;
    private const float qFactor = 0.03f;

    void LateUpdate()
    {
        Vector3 velocity = Vector3.zero;
        Vector3 forward = player.transform.forward * 10.0f;
        Vector3 needPos = player.transform.position - forward;

        transform.position = 
            Vector3.SmoothDamp(transform.position, needPos, ref velocity, vFactor);
        transform.rotation = 
            Quaternion.Lerp(transform.rotation, player.transform.rotation, qFactor);
        transform.LookAt(player.transform);
    }
}
