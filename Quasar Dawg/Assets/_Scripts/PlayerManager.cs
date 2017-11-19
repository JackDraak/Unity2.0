﻿using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    static PlayerManager instance = null;

    private PlayerController playerController;

    private Vector3 playerStartPosition = Vector3.zero;
    private Quaternion playerStartRotation;

    private void Start()
    {
        if (instance != null && instance != this) { Destroy(gameObject); }
        else { instance = this; GameObject.DontDestroyOnLoad(gameObject); }

        bool success = (playerController = GameObject.FindObjectOfType<PlayerController>());
        if (!success) Debug.Log("LevelManager.cs: PlayerController ERROR.");
    }

    public void SetPlayerPosition(Vector3 position) { playerStartPosition = position; }
    public void SetPlayerRotation(Quaternion rotation) { playerStartRotation = rotation; }

    public void ResetPlayer()
    {
        playerController.gameObject.SetActive(false);
        StartCoroutine(LaunchPlayer());
    }

    private IEnumerator LaunchPlayer()
    {
        yield return new WaitForSeconds(3);
        playerController.ChargeShieldBattery(true);
        playerController.ChargeWeaponBattery(true);
        playerController.transform.localPosition = playerStartPosition;
        playerController.transform.localRotation = playerStartRotation;
        playerController.gameObject.SetActive(true);
    }
}