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

    void Start()
    {
        Debug.Log("Nombre de matériaux dans le renderer : " + porteRenderer.materials.Length);
        for (int i = 0; i < porteRenderer.materials.Length; i++)
        {
            Debug.Log($"Matériau [{i}] : {porteRenderer.materials[i].name}");
        }
    }
    public void SetMaterial(WagonMaterial wagonMaterial)
    {
        if (wagonRenderer == null || materialIndicesToChange.Length < 2) return;

        // On récupère tous les matériaux actuels du renderer
        Material[] mats = wagonRenderer.materials;
        Material[] porteMats = porteRenderer.materials;
        

        // On s’assure que les indices sont valides
        if (materialIndicesToChange[0] < mats.Length)
            mats[materialIndicesToChange[0]] = wagonMaterial.PrimaryMaterial;
        porteMats[2] = wagonMaterial.PrimaryMaterial;

        if (materialIndicesToChange[1] < mats.Length)
            mats[materialIndicesToChange[1]] = wagonMaterial.SecondaryMaterial;
        porteMats[1] = wagonMaterial.SecondaryMaterial;

        // On réaffecte le tableau de matériaux au renderer
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
