using UnityEngine;

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

    private static int seed = 0;

    private void Start()
    {
        if (seed == 0) seed = Mathf.FloorToInt(Time.deltaTime * 10000000);
        Random.InitState(seed);

        audioSource = gameObject.GetComponent<AudioSource>();
        difficultyRegulator = FindObjectOfType<DifficultyRegulator>();
        guiTextHandler = FindObjectOfType<GUITextHandler>();
        playerController = FindObjectOfType<PlayerController>();
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
        int variety = Mathf.FloorToInt(Random.Range(0, 5));
        switch (variety)
        {
            case 0: // TODO: have unique audio based on bonus? 
                playerController.ChargeWeaponBattery(.5f); 
                guiTextHandler.ShowBonusText("<size=+20>B</size>laster <size=+20>C</size>harge-<size=+20>B</size>oost<br><size=+10>+50%</size>");
                Debug.Log("bonus case 0 - Weapon power boost");
                break;
            case 1: // TODO: have unique audio based on bonus? 
                playerController.ChargeShieldBattery(.33f); 
                guiTextHandler.ShowBonusText("<size=+20>S</size>hield <size=+20>C</size>harge-<size=+20>B</size>oost<br><size=+10>+33%</size>");
                Debug.Log("bonus case 1 - Shield power boost");
                break;
            case 2: // TODO: have unique audio based on bonus? 
                difficultyRegulator.AdrenalineRush();
                Debug.Log("bonus case 2 - Adrenaline boost");
                break;
            case 3: // TODO: have unique audio based on bonus? 
                difficultyRegulator.BlasterDampen();
                Debug.Log("bonus(anti) case 3 - Dampen Blasters");
                break;
            case 4: // TODO: have unique audio based on bonus? 
                difficultyRegulator.StrafeDampen();
                Debug.Log("bonus(anti) case 4 - Dampen Strafe");
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
