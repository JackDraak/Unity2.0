using System.Collections;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    private bool alive = true;
    private PlayerController playerController;
    private Quaternion playerStartRotation;
    private Vector3 playerStartPosition = Vector3.zero;

    private void OnEnable()
    {
        // Singleton pattern, preferred over making the class static:
        PlayerHandler[] checker;
        checker = FindObjectsOfType<PlayerHandler>();
        if (checker.Length > 1) Destroy(gameObject);
        else DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        bool success = (playerController = FindObjectOfType<PlayerController>());
            if (!success) Debug.Log("PlayerHandler.cs: playerController INFO, ERROR.");
    }

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

    public bool PlayerIsAlive() { return alive; }
    public void SetPlayerPosition(Vector3 position) { playerStartPosition = position; }
    public void SetPlayerRotation(Quaternion rotation) { playerStartRotation = rotation; }
}
