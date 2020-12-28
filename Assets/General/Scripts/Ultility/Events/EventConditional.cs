using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
    
/// <summary>
/// Class use by EventConditional to compare scriptableScore.score is lower or higher than score to win game.
/// </summary>
[System.Serializable]
public class ScoreCondition
{
    public string scoreName;
    public int score{ get{return System.Int32.Parse(PlayerPrefs.GetString(scoreName));} }
    public int minimumScore;
}

public class EventConditional : MonoBehaviour {
    public ScoreCondition[] scoreConditions;
    [ReadOnly]public bool conditionIsPass = true;
  
    public EventSequencer OnWin;
    public EventSequencer OnLose;

/// <summary>
/// Execute unity events base on score value of ScroreCardCondition[] objects. The score lower than minimumScore in any ScoreCardCondition count as not pass.
/// </summary>
	public void ExecuteScriptableScoreCondition()
    {
        JSONSetter globalSetting = FindObjectOfType<JSONSetter>();
        JObject jObject = globalSetting.LoadSetting();

        for (int i = 0; i < scoreConditions.Length; i++)
        {
           //  if(scoreConditions[i].score < System.Int32.Parse(jObject["tier2Score"].ToString())) conditionIsPass = false;
            if(scoreConditions[i].score < System.Int32.Parse(jObject["tier2Score"].ToString())) conditionIsPass = false;

            //if (scoreConditions[i].score < scoreConditions[i].minimumScore) conditionIsPass = false;        
        }

        if (conditionIsPass)
        {
           if(OnWin.events.Length > 0) OnWin.Run();
        }
        else
        {           
            if(OnLose.events.Length > 0) OnLose.Run();
        }
    }
}