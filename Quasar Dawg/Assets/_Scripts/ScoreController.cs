using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scorebox;
    private int score;

    private void Start()            { scorebox = GetComponent<TextMeshProUGUI>(); Reset(); }
    private void Update()           { scorebox.text = "<size=+20>S</size>core: " + score; }

    public void Add(int points)     { score += points; }
    public void Reset()             { score = 0; }
}
