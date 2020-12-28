using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.EventSystems;

public class PuzzleValidator : MonoBehaviour
{
    #region fields

    [SerializeField]
    private string firstAlphabet = "";

    [SerializeField]
    private Physics2DRaycaster raycaster;

    public static PuzzleValidator instance;

    [SerializeField]
    private PuzzleInteraction PI;

    [SerializeField]
    private int correctAlphabet;

    private PuzzleAlphabet firstSelectedPuzzle;
    private PuzzleAlphabet secondSelectedPuzzle;

    [SerializeField]
    private ScoreVisualizer sv;

    private SoundManager sm;

    [SerializeField]
    private UnityEvent onNotify, onEndNotify;

    private List<PuzzleAlphabet> matchPair = new List<PuzzleAlphabet>();
    private List<PuzzleAlphabet> unMatchPair = new List<PuzzleAlphabet>();

    private int unMatchIndex;
    private int matchIndex;

    #endregion fields

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        sm = FindObjectOfType<SoundManager>();
    }

    public void ValidateAlphabet(PuzzleAlphabet alphabet)
    {
        if (onNotify.GetPersistentEventCount() > 0) onNotify.Invoke();

        if (firstAlphabet == "")
        {
            firstAlphabet = alphabet.alphabet;

            firstSelectedPuzzle = alphabet;

            return;
        }
        else
        {
            secondSelectedPuzzle = alphabet;
        }

        if (firstAlphabet == alphabet.alphabet)
        {
            //if (raycaster != null)
            //    raycaster.enabled = false;

            matchPair.Add(firstSelectedPuzzle);
            matchPair.Add(alphabet);

            // if click true alphabet
            matchPair[0].CorrectAnsweHandler();
            matchPair[1].CorrectAnsweHandler();
            sm.AddScoreDelay(.5f);
            Invoke("CorrectAnswerHandler", .5f);

            firstAlphabet = "";

            PI.EndInteract();
        }
        else
        {
            //if (raycaster != null)
            //    raycaster.enabled = false;

            unMatchPair.Add(firstSelectedPuzzle);
            unMatchPair.Add(alphabet);

            // click on wrong alphabet
            sm.MinusScoreDelay(.5f);
            StartCoroutine(WrongAnswerHandler(unMatchPair[0], .5f));
            StartCoroutine(WrongAnswerHandler(unMatchPair[1], .5f));

            firstAlphabet = "";

            //Invoke("WrongAnswerHandler", .5f);

            PI.EndInteract();
        }

        Invoke("EndValidate", .5f);
    }

    private void EndValidate()
    {
        if (onEndNotify.GetPersistentEventCount() > 0) onEndNotify.Invoke();
    }

    private void CorrectAnswerHandler()
    {
        correctAlphabet++;

        if (sv != null)
            sv.UpdateText(1);

        PI.Interact(matchPair[0].transform, PI.ps2);
        PI.Interact(matchPair[1].transform, PI.ps1);

        matchPair.RemoveRange(0, 2);
    }

    private void WrongAnswerHandler()
    {
        unMatchPair.RemoveRange(0, 2);
    }

    private IEnumerator WrongAnswerHandler(PuzzleAlphabet alphabet, float delay)
    {
        yield return new WaitForSeconds(delay);
        // alphabet.WrongAnsweHandler();
        unMatchPair[unMatchIndex].WrongAnsweHandler();

        unMatchIndex++;
    }
}