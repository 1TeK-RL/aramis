using UnityEngine;
using TMPro;

public class WagonCustomize : MonoBehaviour
{
    [SerializeField] private Renderer wagonRenderer;
    [SerializeField] private Renderer porteRenderer;

    [SerializeField] private int[] materialIndicesToChange;

    [SerializeField] private TextMeshProUGUI text1;
    [SerializeField] private TextMeshProUGUI text2;
    [SerializeField] private TextMeshProUGUI text3;

    public void SetMaterial(WagonMaterial wagonMaterial)
    {
        if (wagonRenderer == null || materialIndicesToChange.Length < 2) return;

        Material[] mats = wagonRenderer.materials;
        Material[] porteMats = porteRenderer.materials;
        
        if (materialIndicesToChange[0] < mats.Length)
            mats[materialIndicesToChange[0]] = wagonMaterial.PrimaryMaterial;
        porteMats[2] = wagonMaterial.PrimaryMaterial;

        if (materialIndicesToChange[1] < mats.Length)
            mats[materialIndicesToChange[1]] = wagonMaterial.SecondaryMaterial;
        porteMats[1] = wagonMaterial.SecondaryMaterial;

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
