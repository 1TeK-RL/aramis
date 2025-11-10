using TMPro;
using UnityEngine;

public class WagonCustomize : MonoBehaviour
{
    [SerializeField] private Renderer wagonRenderer;
    [SerializeField] private Renderer porteRenderer;

    public int[] materialIndicesToChange;

    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;
    public TextMeshProUGUI text3;


    public void SetColor(Color PrimaryColor, Color SecondaryColor)
    {
        if (wagonRenderer == null || materialIndicesToChange.Length < 2) return;

        // Clone le tableau de matériaux pour ne pas modifier sharedMaterials
        Material[] mats = wagonRenderer.materials;
        Material[] porteMats = porteRenderer.materials;

        // Material primaire
        Material newMaterial1 = new Material(mats[materialIndicesToChange[0]]);
        Material newMaterialporte1 = new Material(porteMats[2]);
        newMaterial1.color = PrimaryColor;
        newMaterialporte1.color = PrimaryColor;
        mats[materialIndicesToChange[0]] = newMaterial1;
        porteMats[2] = newMaterialporte1;

        // Material secondaire
        Material newMaterial2 = new Material(mats[materialIndicesToChange[1]]);
        newMaterial2.color = SecondaryColor;
        mats[materialIndicesToChange[1]] = newMaterial2;
        Material newMaterialporte2 = new Material(porteMats[1]);
        newMaterialporte2.color = SecondaryColor;
        porteMats[1] = newMaterialporte2;

        wagonRenderer.materials = mats;
        porteRenderer.materials = porteMats;
    }

    public void UpdateTexts(string newText)
    {
        if (text1 != null) text1.text = newText;
        if (text2 != null) text2.text = newText;
        if (text3 != null) text3.text = newText;
    }
}
