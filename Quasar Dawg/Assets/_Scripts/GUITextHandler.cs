using TMPro;
using UnityEngine;

public class GUITextHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI bonusText;

    private int score;

    private void Start()
    {
        ReseScoret();
        HideBonusText();
    }

    private void Update()
    {
        scoreText.text = "<size=+20>S</size>core: " + score.ToString();
    }

    public void AddToScore(int points)      { score += points; }
    public void ReseScoret()                { score = 0; }

    public void ShowBonusText(string text)
    {
        bonusText.text = text;
        Invoke("HideBonusText", 3f);
        bonusText.gameObject.SetActive(true);
    }

    public void PopText(string text)
    {
        bonusText.text = text;
        if (!bonusText.gameObject.activeSelf) bonusText.gameObject.SetActive(true);
    }

    public void DropText()
    {
        if (bonusText.gameObject.activeSelf) bonusText.gameObject.SetActive(false);
    }

    private void HideBonusText()
    {
        bonusText.gameObject.SetActive(false);
    }
}
