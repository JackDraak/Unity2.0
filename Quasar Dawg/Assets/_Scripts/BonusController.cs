using System.Collections;
using UnityEngine;

public class BonusController : MonoBehaviour
{
    [SerializeField] AudioClip collectSound;
    [SerializeField] GameObject collectEffect;
    [SerializeField] GameObject mainEffect;
    [SerializeField] GameObject orbObject;
    [SerializeField] GameObject parentBonusObject;

    private AudioSource audioSource;
    private bool adrenalineRush = false;
    private float adrenalineBegin = 0;
    private float regularTimeScale = 1;
    private PlayerController playerController;
    private PlayerHandler playerHandler;
    private GUITextHandler guiTextHandler;

    private void Start()
    {
        bool success;

        success = (audioSource = gameObject.GetComponent<AudioSource>());
            if (!success) Debug.Log("EnergyBonus.cs: audioSource INFO, FAIL.");
        success = (playerController = FindObjectOfType<PlayerController>());
            if (!success) Debug.Log("EnergyBonus.cs: playerController INFO, FAIL.");
        success = (playerHandler = FindObjectOfType<PlayerHandler>());
            if (!success) Debug.Log("EnergyBonus.cs: playerHandler INFO, FAIL.");
        success = (guiTextHandler = FindObjectOfType<GUITextHandler>());
            if (!success) Debug.Log("EnergyBonus.cs: guiTextHandler INFO, FAIL.");
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
        // variety of potential bonus effects, chose one at random:
        int variety = Mathf.FloorToInt(Random.Range(0, 3));
        switch (variety)
        {
            case 0:
                playerController.ChargeWeaponBattery(.5f); // TODO: have unique audio based on bonus? 
                guiTextHandler.ShowBonusText("<size=+20>B</size>laster <size=+20>C</size>harge-<size=+20>B</size>oost<br><size=+10>+50%</size>");
                Debug.Log("case 0");
                break;
            case 1:
                playerController.ChargeShieldBattery(.33f); // TODO: have unique audio based on bonus? 
                guiTextHandler.ShowBonusText("<size=+20>S</size>hield <size=+20>C</size>harge-<size=+20>B</size>oost<br><size=+10>+33%</size>");
                Debug.Log("case 1");
                break;
            case 2:
                adrenalineRush = true;
                adrenalineBegin = Time.time;
                regularTimeScale = Time.timeScale;
                guiTextHandler.PopText("<size=+20>A</size>drenaline <size=+20>R</size>ush!");
                Time.timeScale = 0.5f;
                Invoke("NormalizeTime", 3);
           //     StartCoroutine(AdrenalineRush()); // TODO: have unique audio based on bonus? 
                Debug.Log("case 2");
                break;
            case 6:
                adrenalineRush = true;
                adrenalineBegin = Time.time;
                regularTimeScale = Time.timeScale;
            //    StartCoroutine(AdrenalineRush()); // TODO: have unique audio based on bonus? 
                Debug.Log("case 6");
                break;
            default:
                guiTextHandler.ShowBonusText("<size=+20>D</size>efault <size=+20>C</size>ase!");
                Debug.Log("DEFAULT CASE");
                break;
        }
        audioSource.Stop();
        audioSource.PlayOneShot(collectSound);
        mainEffect.SetActive(false);
        collectEffect.SetActive(true);
        orbObject.SetActive(false);
        Invoke("DestroySelf", 3f);
    }

    private void NormalizeTime()
    {
        Time.timeScale = 1;
        guiTextHandler.DropText();
    }

  /*  private IEnumerator AdrenalineRush()
    {
        var desiredScale = 0.6f;
        var duration = 5f;
        float adrenalineTime = Time.time - adrenalineBegin;
        float timeLeft = Mathf.Round((duration - adrenalineTime) * 10) / 10;
        if (timeLeft == Mathf.Epsilon)
        {
            adrenalineRush = false;
            guiTextHandler.HideAdrenalineText();
            yield return 0;
        }
        if (adrenalineRush)
        {
            guiTextHandler.ShowAdrenalineText("<size=+20>A</size>drenaline <size=+20>B</size>oost!<br><size=+20>" + timeLeft.ToString() + " seconds</size>");
            Time.timeScale = Mathf.InverseLerp(regularTimeScale, desiredScale, (timeLeft / duration));
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(AdrenalineRush());
        }
        yield return 0;
    } */

    private void DestroySelf()
    {
        Destroy(parentBonusObject.gameObject);
    }
}
