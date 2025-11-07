    using UnityEngine;
using UnityEngine.UI;

public class ColorButton : MonoBehaviour
{
    public PersonnalisationManager customizer;
    public Color color;

    public void ApplyColor()
    {
        customizer.SetColor(color);
    }
}

