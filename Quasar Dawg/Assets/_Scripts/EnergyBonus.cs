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
    private PlayerHandler playerHandler;

        private void Start()
    {
        bool success;// EnemyController.cs: hitPool INFO - FAIL.

        success = (audioSource = gameObject.GetComponent<AudioSource>());
            if (!success) Debug.Log("EnergyBonus.cs: audioSource INFO, FAIL.");
        success = (playerController = FindObjectOfType<PlayerController>());
            if (!success) Debug.Log("EnergyBonus.cs: playerController INFO, FAIL.");
        success = (playerHandler = FindObjectOfType<PlayerHandler>());
            if (!success) Debug.Log("EnergyBonus.cs: playerHandler INFO, FAIL.");
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
            playerController = FindObjectOfType<PlayerController>();
            if (!playerController) Debug.Log("EnergyBonus.cs: playerController INFO, REUP-FAIL.");
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
