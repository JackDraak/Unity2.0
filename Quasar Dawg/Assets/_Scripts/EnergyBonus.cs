using UnityEngine;

public class EnergyBonus : MonoBehaviour
{
    [SerializeField] AudioClip collectSound;
    [SerializeField] GameObject collectEffect;
    [SerializeField] GameObject mainEffect;
    [SerializeField] GameObject orbObject;
    [SerializeField] GameObject parentBonusObject;

    private AudioSource audioSource;
    private PlayerController playerController;
    private PlayerManager playerManager;

        private void Start()
    {
        bool success;

        success = (audioSource = gameObject.GetComponent<AudioSource>());
            if (!success) Debug.Log("EnergyBonus.cs: audioSource ERROR.");
        success = (playerController = GameObject.FindObjectOfType<PlayerController>());
            if (!success) Debug.Log("EnergyBonus.cs: playerController ERROR.");
        success = (playerManager = FindObjectOfType<PlayerManager>());
            if (!success) Debug.Log("EnergyBonus.cs: playerManager ERROR.");
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
        if (playerController == null)
        {
            // Debug.Log("EB.cs playerController REUP:");
            playerController = GameObject.FindObjectOfType<PlayerController>();
            if (!playerController) Debug.Log("EB.cs playerController REUP-FAIL.");
        }
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
