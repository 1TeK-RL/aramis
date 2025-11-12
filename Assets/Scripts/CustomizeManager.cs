using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class CustomizeManager : MonoBehaviour
{
    public static CustomizeManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private List<WagonCustomize> wagonMeshes;
    [SerializeField] private WagonMaterial startMaterial;
    [SerializeField] private TMP_InputField inputField;
    private WagonMaterial currentMaterial;

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
        currentMaterial = startMaterial;
        ChangeWagon(currentWagonIndex);
        SetMaterial(currentMaterial);
    }

    public void ChangeWagon(int WagonMeshIndex)
    {
        currentWagonIndex = WagonMeshIndex;
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


    public void ResetCustomize()
    {
        currentMaterial = startMaterial;
        currentText = string.Empty;
        currentWagonIndex = 0;
        ChangeWagon(currentWagonIndex);
        SetMaterial(currentMaterial);
        UpdateTexts(currentText);
        inputField.text = "";
    }
}

[System.Serializable]
public class WagonMaterial
{
    public Material PrimaryMaterial;
    public Material SecondaryMaterial;
}


