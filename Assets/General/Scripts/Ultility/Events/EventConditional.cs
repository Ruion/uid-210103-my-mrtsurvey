using UnityEngine;
using Unity.Collections;
using Sirenix.OdinInspector;

/// <summary>
/// Class use by EventConditional to compare scriptableScore.score is lower or higher than score to win game.
/// </summary>
[System.Serializable]
public class ScoreCondition
{
    public string scoreName;
    public int score { get { return System.Int32.Parse(PlayerPrefs.GetString(scoreName)); } }

    [HideIf("useEnvSetting", false)]
    public int minimumScore;

    public bool useEnvSetting = true;
    public string envSettingName = "GAME_WIN_SCORE";
}

public class EventConditional : MonoBehaviour
{
    public ScoreCondition[] scoreConditions;
    [Unity.Collections.ReadOnly] public bool conditionIsPass = true;

    public EventSequencer OnConditionPass;
    public EventSequencer OnConditionFail;

    /// <summary>
    /// Execute unity events base on score value of ScroreCardCondition[] objects. The score lower than minimumScore in any ScoreCardCondition count as not pass.
    /// </summary>
    public void ExecuteScriptableScoreCondition()
    {
        for (int i = 0; i < scoreConditions.Length; i++)
        {
            int minimumScore = scoreConditions[i].minimumScore;
            if (scoreConditions[i].useEnvSetting)
                minimumScore = JSONExtension.LoadEnvInt(scoreConditions[i].envSettingName);

            if (scoreConditions[i].score < minimumScore) conditionIsPass = false;
        }

        if (conditionIsPass)
        {
            if (OnConditionPass.events.Length > 0) OnConditionPass.Run();
        }
        else
        {
            if (OnConditionFail.events.Length > 0) OnConditionFail.Run();
        }
    }
}