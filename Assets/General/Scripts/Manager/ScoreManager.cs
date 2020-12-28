using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour {

    public static ScoreManager instance;

    public ScoreVisualizer scoreVisualizer;

    public SoundManager soundManager;
    public Transform spawnTarget;
    public Transform scoreEffectContainer;
    public GameObject addScoreEffectPrefab;
    public GameObject minusScoreEffectPrefab;
    public Transform scoreDestination;

    [ReadOnly]
    public string scoreName = "score";

    void Awake()
    {
        instance = this;

        PlayerPrefs.SetString(scoreName, "0");
    }

	public void AddScore(int amount)
    {
        soundManager.AddScore();
        SpawnScoreEffect(addScoreEffectPrefab);         
    }

    public void MinusScore(int amount)
    {
        soundManager.MinusScore();
        SpawnScoreEffect(minusScoreEffectPrefab);
        scoreVisualizer.UpdateText(-amount);       
    }

    public void AddScore()
    {
        scoreVisualizer.UpdateText(8);  
    }

    public void SpawnScoreEffect(GameObject effectPrefab)
    {
        GameObject vfx = Instantiate(effectPrefab, spawnTarget.position , Quaternion.identity, scoreEffectContainer);
        ObjectMover objMover = vfx.GetComponent<ObjectMover>();

        if(objMover==null) return;

        objMover.target = scoreDestination;
        objMover.enabled = true;
    }

}
