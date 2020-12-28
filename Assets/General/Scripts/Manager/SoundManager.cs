using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioSource addScore;
    public AudioSource minusScore;

	public void AddScore(){ addScore.PlayOneShot(addScore.clip); }
	public void MinusScore(){ minusScore.PlayOneShot(minusScore.clip); }

    public void AddScoreDelay(float delay)
    {
        Invoke("AddScore", delay);
    }

    public void MinusScoreDelay(float delay)
    {
        Invoke("MinusScore", delay);
    }
}
