using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class PuzzleAutoPlayer : MonoBehaviour
{
    [SerializeField]
    private PuzzleBuilder puzzleBuilder;

    private int selectedPair = 0;

    public List<PuzzleAlphabet> puzzleAlphabet = new List<PuzzleAlphabet>();

    [Button(ButtonSizes.Medium)]
    private void Start()
    {
        puzzleAlphabet = new List<PuzzleAlphabet>();
        puzzleAlphabet.AddRange(puzzleBuilder.puzzleAlphabets);

        // Sort 2 puzzle with same alphabet next to each other
        puzzleAlphabet.Sort((x, b) => string.Compare(x.alphabet, b.alphabet, true));

        // Compares two specified String objects, ignoring or honoring their
        // case, and returns an integer that indicates their relative position in the sort order.
        // string.Compare(x.alphabet, b.alphabet, true)
    }

    [Button(ButtonSizes.Medium)]
    public async void MatchPuzzle()
    {
        // make selectedPair index revert to 0 and start again
        if (selectedPair >= puzzleAlphabet.Count) selectedPair = 0;

        // Flip the first puzzle
        puzzleAlphabet[selectedPair].NotifyPuzzle();

        // Wait for 1 second before execute next line of code
        await System.Threading.Tasks.Task.Delay(1000);

        // Flip the second puzzle
        puzzleAlphabet[selectedPair + 1].NotifyPuzzle();

        // Go to next pair of puzzle
        selectedPair += 2;
    }
}