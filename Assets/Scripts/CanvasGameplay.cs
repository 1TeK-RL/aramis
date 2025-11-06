using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasGameplay : MonoBehaviour
{
    [SerializeField] List<Image> images;

    public void changeColor(Color color)
    {
        foreach (Image img in images)
        {
            img.color = color;
        }
    }
}
