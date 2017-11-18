using UnityEngine;

public class EnergyBonus : MonoBehaviour
{
    [SerializeField] GameObject parentBonusObject;
    [SerializeField] GameObject orbObject;
    [SerializeField] GameObject mainEffect;
    [SerializeField] GameObject collectEffect;
    [SerializeField] AudioClip collectSound;

    PlayerController playerController;
    AudioSource audioSource;

    private void Start()
    {
        bool success;

        success = (playerController = GameObject.FindObjectOfType<PlayerController>());
        if (!success) Debug.Log("EnergyBonus.cs: PlayerController ERROR.");

        success = (audioSource = gameObject.GetComponent<AudioSource>());
        if (!success) Debug.Log("EnergyBonus.cs: AudioSource ERROR.");
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "ShipCollider":
                CollectOrb();
                break;
            default:
                break;
        }
    }

    private void CollectOrb()
    {
        playerController.ChargeBattery(.5f);
        audioSource.Stop();
        audioSource.PlayOneShot(collectSound);
        mainEffect.SetActive(false);
        collectEffect.SetActive(true);
        orbObject.SetActive(false);
        Invoke("DestroySelf", 3f);
    }

    private void DestroySelf()
    {
        Destroy(parentBonusObject.gameObject);
    }
}
