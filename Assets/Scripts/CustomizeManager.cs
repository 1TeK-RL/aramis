using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class CustomizeManager : MonoBehaviour
{
    public static CustomizeManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private List<WagonCustomize> wagonMeshes;
    [SerializeField] private WagonMaterial currentMaterial;

    private WagonCustomize currentWagonMesh;
    public string currentText;
    private int currentWagonIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        currentText = string.Empty;
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        currentWagonMesh = wagonMeshes[0];
        ChangeWagon(currentWagonIndex);
        SetMaterial(currentMaterial);
    }

    public void ChangeWagon(int WagonMeshIndex)
    {
        currentWagonMesh.gameObject.SetActive(false);
        currentWagonMesh = wagonMeshes[WagonMeshIndex];
        currentWagonMesh.SetMaterial(currentMaterial);
        currentWagonMesh.UpdateTexts(currentText);
        currentWagonMesh.gameObject.SetActive(true);
    }

    public void SetMaterial(WagonMaterial materials)
    {
        if (currentWagonMesh == null) return;

        currentMaterial = materials;
        currentWagonMesh.SetMaterial(currentMaterial);
        Debug.Log("Change Color");
    }

    public void UpdateTexts(string newText)
    {
        currentText = newText;
        currentWagonMesh.UpdateTexts(currentText);
    }

    public WagonMaterial GetCurrentMaterial()
    {
        return currentMaterial;
    }

    public int GetCurrentWagonIndex()
    {
        return currentWagonIndex;
    }
}

[System.Serializable]
public class WagonMaterial
{
    public Material PrimaryMaterial;
    public Material SecondaryMaterial;
}


