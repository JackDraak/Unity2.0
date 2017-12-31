using UnityEngine;

public class MainLightControl : MonoBehaviour
{
    private LevelValet levelValet;
    private Light directionalLight;
    private KeyCode brighter, darker;
    private KeyValet keyValet;
    private float duration = 60;

    private void Start()
    {
        keyValet = FindObjectOfType<KeyValet>();
        levelValet = FindObjectOfType<LevelValet>();
        brighter = keyValet.GetKey("Light-Brighten");
        darker = keyValet.GetKey("Light-Darken");
        directionalLight = GetComponent<Light>();
    }

    private void Update()
    {
     if (Input.GetKeyDown(brighter))
        {
            levelValet.SetBrightness((float) (levelValet.GetBrightness() + .5));
            if (levelValet.GetBrightness() > 5) levelValet.SetBrightness(5);
        }

        if (Input.GetKeyDown(darker))
        {
            levelValet.SetBrightness((float)(levelValet.GetBrightness() - .5));
            if (levelValet.GetBrightness() < -2) levelValet.SetBrightness(-2);
        } 

        float phi = Time.time / duration * 2 * Mathf.PI;
        float amplitude = levelValet.GetBrightness() + Mathf.Cos(phi) * 0.5F + 0.5F;
        directionalLight.intensity = (amplitude);
    }
}
