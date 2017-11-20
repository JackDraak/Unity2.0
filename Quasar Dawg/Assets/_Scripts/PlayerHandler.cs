using System.Collections;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    static PlayerHandler instance = null;

    private PlayerController playerController;
    private bool alive = true;
    private Quaternion playerStartRotation;
    private Vector3 playerStartPosition = Vector3.zero;

    private void Start()
    {
        if (instance != null && instance != this) { Destroy(gameObject); }
        else { instance = this; DontDestroyOnLoad(gameObject); }

        bool success = (playerController = FindObjectOfType<PlayerController>());
        if (!success) Debug.Log("PlayerHandler.cs: PlayerController ERROR.");
    }

    public bool PlayerIsAlive() { return alive; }

    private IEnumerator LaunchPlayer()
    {
        yield return new WaitForSeconds(3);
        playerController.ChargeShieldBattery(true);
        playerController.ChargeWeaponBattery(true);
        playerController.transform.localPosition = playerStartPosition;
        playerController.transform.localRotation = playerStartRotation;
        playerController.gameObject.SetActive(true);
        alive = true;
    }

    public void ResetPlayer()
    {
        alive = false;
        playerController.gameObject.SetActive(false);
        StartCoroutine(LaunchPlayer());
    }

    public void SetPlayerPosition(Vector3 position) { playerStartPosition = position; }
    public void SetPlayerRotation(Quaternion rotation) { playerStartRotation = rotation; }
}
