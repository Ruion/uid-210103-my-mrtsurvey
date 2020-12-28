using UnityEngine;
using TMPro;

/// <summary>
/// Visualize the score to TextMeshProUGUI text. Use this component
/// to add/minus score and display score to text
/// </summary>
public class ScoreVisualizer : MonoBehaviour
{
    public TextMeshProUGUI[] scoreTexts;
    private int score;

    [SerializeField]
    private int maxScore;

    private void Awake()
    {
        score = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UpdateText(2);
        }
    }

    public void UpdateText(int amount)
    {
        score += amount;

        VisualiseScore();

        if (score >= maxScore) FindObjectOfType<GameManager>().GameOver();
    }

    public void GetScoreFromPlayerPrefs()
    {
        score = System.Int32.Parse(PlayerPrefs.GetString("score"));
    }

    public void VisualiseScore()
    {
        for (int t = 0; t < scoreTexts.Length; t++)
        {
            scoreTexts[t].text = score.ToString();
        }
    }

    public void VisualiseTier()
    {
        for (int t = 0; t < scoreTexts.Length; t++)
        {
            scoreTexts[t].text = PlayerPrefs.GetString("voucher_code").Insert(4, " ");
        }
    }

    public void SaveScore()
    {
        /*
        LoadGameSettingFromMaster();

        string scoreName = gameSettings.scoreName;

        PlayerPrefs.SetString(scoreName, score.ToString());

         */

        PlayerPrefs.SetString("score", score.ToString());

        GameSettingEntity gse = FindObjectOfType<GameSettingEntity>();

        if (score >= gse.gameSettings.tier1Score) PlayerPrefs.SetString("voucher_code", "TIER1");
        if (score >= gse.gameSettings.tier2Score) PlayerPrefs.SetString("voucher_code", "TIER2");
        if (score >= gse.gameSettings.tier3Score) PlayerPrefs.SetString("voucher_code", "TIER3");
    }
}