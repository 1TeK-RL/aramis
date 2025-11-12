using NUnit.Framework;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core;

public class GhostCustomize : MonoBehaviour
{
    [SerializeField] private List<WagonCustomize> wagonMeshes;
    private WagonMaterial currentMaterial;
    private string currentText;
    private int WagonMeshIndex = 0;

    public void SetCustomization()
    {
        wagonMeshes[WagonMeshIndex].SetMaterial(currentMaterial);
        wagonMeshes[WagonMeshIndex].UpdateTexts(currentText);
    }

    public void SetMaterial(WagonMaterial materials)
    {
        currentMaterial = materials;
    }

    public void SetText(string newText)
    {
        currentText = newText;
    }

    public void SetWagonIndex(int index)
    {
        WagonMeshIndex = index;
    }
}
