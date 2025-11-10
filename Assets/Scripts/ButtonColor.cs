using UnityEngine;

public class ColorButton : MonoBehaviour
{
    [SerializeField] private Color PrimaryColor;
    [SerializeField] private Color SecondaryColor;

    public void ApplyColor()
    {
        CustomizeManager.Instance.SetColor(PrimaryColor, SecondaryColor);
    }
}

