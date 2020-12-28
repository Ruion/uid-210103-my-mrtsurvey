using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Threading.Tasks;

public class PuzzleAlphabet : MonoBehaviour
{
    public string alphabet;

    public bool isInteractable = true;

    [SerializeField]
    private DOTweenAnimation dta, dtaCorrect;

    public void NotifyPuzzle()
    {
        if (!isInteractable) return;

        isInteractable = false;

        PuzzleValidator.instance.ValidateAlphabet(this);
        dta.DOPlayForwardById("card");
    }

    public void CorrectAnsweHandler()
    {
        isInteractable = false;
        GetComponent<EventTrigger>().enabled = false;

        //if (dtaCorrect != null)
            //dtaCorrect.DOPlayById("Correct");
    }

    public async void WrongAnsweHandler()
    {
        dta.DOPlayBackwards();
        await Task.Delay(201);

        isInteractable = true;
    }
}