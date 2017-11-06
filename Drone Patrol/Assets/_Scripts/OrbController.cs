using UnityEngine;
using System.Collections;

public class OrbController : MonoBehaviour {
    [SerializeField] ParticleSystem greenSmoke;
    [SerializeField] GameObject orb;
    float fadeTime = 1.431f; // Duration of audio-cue
    string buttonName = "Toggle Material";
    //color records and state booleans
    private Color solidColor;
    private Color fadedColor;
    private bool fading = false;
    private bool faded = false;

    void Start()
    {
  //      solidColor = orb.GetComponent<Shader>().   color;
        fadedColor = new Color(solidColor.r, solidColor.g, solidColor.b, 0.0f);
    }
    //check for input only if we aren't in the middle of fading
    void Init()
    {
        if (faded)
            StartCoroutine(FadeIn());
        else
            StartCoroutine(FadeOut());
    }
    //set fading and lerp from faded to solid over fadeTime
    IEnumerator FadeIn()
    {
        fading = true;
        for (float t = 0.0f; t < fadeTime; t += Time.deltaTime)
        {
            orb.GetComponent<Material>().color = Color.Lerp(fadedColor, solidColor, t / fadeTime);
            yield return new WaitForFixedUpdate();
        }
        fading = false;
        faded = false;
    }
    //set fading and lerp from solid to faded over fadeTime
    IEnumerator FadeOut()
    {
        fading = true;
        for (float t = 0.0f; t < fadeTime; t += Time.deltaTime)
        {
            orb.GetComponent<Material>().color = Color.Lerp(solidColor, fadedColor, t / fadeTime);
            yield return new WaitForFixedUpdate();
        }
        fading = false;
        faded = true;
    }

    void OnTriggerEnter(Collider other)
    {
 //       Init();
        var em = greenSmoke.emission;
        em.rateOverTime = 0;
    }
}
