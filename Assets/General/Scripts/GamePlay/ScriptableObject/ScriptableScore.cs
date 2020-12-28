using UnityEngine;



[CreateAssetMenu(fileName = "Score", menuName = "Score")]
/// <summary>
/// Scriptable object to store score value between scenes
/// Tips: right click in Assets folder and select "Score" to create ScriptableScore asset.
/// usually use by EventOnConditional to compare game score is win or lose.
///  </summary>
public class ScriptableScore : ScriptableObject {

    public int score;
    public int maximumScore = 999;
    public int minimumScore = 0;


    public void AddScore(int amount)
    {
        score += amount;
    }
}
