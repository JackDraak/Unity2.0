using UnityEngine;

public class EnergyBonus : MonoBehaviour
{
    [SerializeField] AudioClip collectSound;
    [SerializeField] GameObject collectEffect;
    [SerializeField] GameObject mainEffect;
    [SerializeField] GameObject orbObject;
    [SerializeField] GameObject parentBonusObject;

    AudioSource audioSource;
    PlayerController playerController;

    private void Start()
    {
        bool success;

        success = (audioSource = gameObject.GetComponent<AudioSource>());
            if (!success) Debug.Log("EnergyBonus.cs: AudioSource ERROR.");
        success = (playerController = GameObject.FindObjectOfType<PlayerController>());
            if (!success) Debug.Log("EnergyBonus.cs: PlayerController ERROR.");
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
        playerController.ChargeWeaponBattery(.5f);
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
