using System.Collections.Generic;
using UnityEngine;

public class PuzzleInteraction : MonoBehaviour
{
    public ParticleSystem ps1;
    public ParticleSystem ps2;

    public Color[] colors;

    private Color selectedColor;


    public void Interact(Transform puzzleTransform, ParticleSystem ps)
    {
        ps.transform.position = puzzleTransform.position;
        ps.Play();

        selectedColor = colors[Random.Range(0, colors.Length)];
        var main = ps1.main;

        var main2 = ps2.main;

       main.startColor = new Color(selectedColor.r, selectedColor.g, selectedColor.b);
       main2.startColor = new Color(selectedColor.r, selectedColor.g, selectedColor.b);
    }

    public void EndInteract()
    {
        ps1.Stop();
        ps2.Stop();
    }
}
