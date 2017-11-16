using UnityEngine;

public class EnergyBonus : MonoBehaviour
{
    [SerializeField] GameObject orb;
    [SerializeField] GameObject parentBonus;

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
        playerController.ChargeBattery(.5f);
        audioSource.Stop();
        Invoke("DestroySelf", 10f);
        orb.SetActive(false);
    }

    private void DestroySelf()
    {
        Destroy(parentBonus.gameObject);
    }
}
