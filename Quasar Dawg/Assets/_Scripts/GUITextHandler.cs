using TMPro;
using UnityEngine;

public class GUITextHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI bonusText;
    [SerializeField] TextMeshProUGUI scoreText;

    private int score;

    private void Start()
    {
        ResetScore();
        HideBonusText();
    }

    private void Update()
    {
        scoreText.text = "<size=+20>S</size>core: " + score.ToString();
    }

    public void AddToScore(int points)      { score += points; }
    public void ResetScore()                { score = 0; }
    private void HideBonusText()            { bonusText.gameObject.SetActive(false); }

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

}
