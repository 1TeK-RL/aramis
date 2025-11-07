    using UnityEngine;
using UnityEngine.UI;

public class ColorButton : MonoBehaviour
{
    
    public Color PrimaryColor;
    public Color SecondaryColor;

    public void ApplyColor()
    {
        PersonnalisationManager.Instance.SetColor(PrimaryColor, SecondaryColor);
    }
}

