﻿using UnityEngine;

public class BonusController : MonoBehaviour
{
    [SerializeField] AudioClip collectSound;
    [SerializeField] GameObject collectEffect;
    [SerializeField] GameObject mainEffect;
    [SerializeField] GameObject orbObject;
    [SerializeField] GameObject parentBonusObject;

    private AudioSource audioSource;
    private DifficultyRegulator difficultyRegulator;
    private GUITextHandler guiTextHandler;
    private PlayerController playerController;
    private PlayerHandler playerHandler;

    private static int seed = 0;

    private void Start()
    {
        if (seed == 0) seed = Mathf.FloorToInt(Time.deltaTime * 10000000);
        Random.InitState(seed);

        bool success;

        success = (audioSource = gameObject.GetComponent<AudioSource>());
            if (!success) Debug.Log("EnergyBonus.cs: audioSource INFO, FAIL.");
        success = (difficultyRegulator = FindObjectOfType<DifficultyRegulator>());
            if (!success) Debug.Log("EnergyBonus.cs: difficultyRegulator INFO, FAIL.");
        success = (guiTextHandler = FindObjectOfType<GUITextHandler>());
            if (!success) Debug.Log("EnergyBonus.cs: guiTextHandler INFO, FAIL.");
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
            playerController = FindObjectOfType<PlayerController>();
            if (!playerController) Debug.Log("EnergyBonus.cs: playerController INFO, REUP-FAIL.");
        }

        // variety of potential bonus effects, chose one at random:
        int variety = Mathf.FloorToInt(Random.Range(0, 3));
        switch (variety)
        {
            case 0:
                playerController.ChargeWeaponBattery(.5f); // TODO: have unique audio based on bonus? 
                guiTextHandler.ShowBonusText("<size=+20>B</size>laster <size=+20>C</size>harge-<size=+20>B</size>oost<br><size=+10>+50%</size>");
                Debug.Log("bonus case 0 - Weapon power boost");
                break;
            case 1:
                playerController.ChargeShieldBattery(.33f); // TODO: have unique audio based on bonus? 
                guiTextHandler.ShowBonusText("<size=+20>S</size>hield <size=+20>C</size>harge-<size=+20>B</size>oost<br><size=+10>+33%</size>");
                Debug.Log("bonus case 1 - Shield power boost");
                break;
            case 2:
                // bool success = true;
                // if (difficultyRegulator == null) success = (difficultyRegulator = FindObjectOfType<DifficultyRegulator>());
                // if (!success) Debug.Log("EnergyBonus.cs: difficultyRegulator INFO, RE-UP FAIL.");
                difficultyRegulator.AdrenalineRush();
                // TODO: have unique (additional) audio based on bonus? 
                Debug.Log("bonus case 2 - Adrenaline boost");
                break;
            case 3:
                Debug.Log("bonus case 3 - boost TBA");
                break;
            default:
                guiTextHandler.ShowBonusText("<size=+20>D</size>efault <size=+20>C</size>ase!");
                Debug.Log("DEFAULT BONUS CASE");
                break;
        }
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