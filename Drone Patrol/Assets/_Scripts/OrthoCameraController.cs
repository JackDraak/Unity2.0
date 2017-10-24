using UnityEngine;

public class OrthoCameraController : MonoBehaviour {
    [SerializeField] GameObject player;
    private float offset;
    private Vector3 newPos;

    // Use this for initialization
    void Start()
    {
        offset = transform.position.x - player.transform.position.x;
    }

    // LateUpdate is called after eache Update frame
    void LateUpdate()
    {
        newPos = new Vector3 (player.transform.position.x + offset, 
            transform.position.y, transform.position.z);
 
        transform.position = newPos;
    }
}
