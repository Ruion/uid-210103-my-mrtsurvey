using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Linq;

public class PuzzleBuilder : SerializedMonoBehaviour
{
    #region
    public Dictionary<string, Sprite> alphabetImages = new Dictionary<string, Sprite>();

    public List<PuzzleAlphabet> puzzleAlphabets = new List<PuzzleAlphabet>();

    [SerializeField]
    private List<string> selectedAlphabets = new List<string>();

    [SerializeField]
    private int puzzleTypeCount = 8;

    #endregion

    private void Awake()
    {
        Build();
    }

    [Button(ButtonSizes.Large)]
    public void Build()
    {
        List<PuzzleAlphabet> buildAlphabets = new List<PuzzleAlphabet>();
        buildAlphabets.AddRange(puzzleAlphabets);

        List<string> alphabetKeys = alphabetImages.Keys.ToList();
        var rand = new System.Random();

        selectedAlphabets = alphabetKeys.OrderBy(x => rand.Next(puzzleTypeCount)).Take(puzzleTypeCount).ToList();

        // Debug.Log(string.Join("\n", selectedAlphabets));

        // select 2 Image for 1 sprite
        for (int i = 0; i < puzzleTypeCount; i++)
        {
            PuzzleAlphabet[] puzzlePair = buildAlphabets.OrderBy(x => rand.Next(buildAlphabets.Count)).Take(2).ToArray();

            // change alphabet of 2 alphabet
            puzzlePair[0].alphabet = selectedAlphabets[i];
            puzzlePair[1].alphabet = selectedAlphabets[i];

            puzzlePair[0].name = selectedAlphabets[i] + "-A";
            puzzlePair[1].name = selectedAlphabets[i] + "-B";

            // change image of 2 card
            puzzlePair[0].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = alphabetImages[selectedAlphabets[i]];
            puzzlePair[1].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = alphabetImages[selectedAlphabets[i]];

            buildAlphabets.Remove(puzzlePair[0]);
            buildAlphabets.Remove(puzzlePair[1]);
        }
    }
}