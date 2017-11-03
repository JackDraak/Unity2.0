using UnityEngine;

public class OrthoCameraController : MonoBehaviour {
    [SerializeField] GameObject player;
    private float offset;
    private Vector3 newPos;

    void Start()
    {
        offset = transform.position.x - player.transform.position.x;
    }

    void LateUpdate()
    {
        newPos = new Vector3 (player.transform.position.x + offset, 
            transform.position.y, transform.position.z);
 
        transform.position = newPos;
    }
}
