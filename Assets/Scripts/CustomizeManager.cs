using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class CustomizeManager : MonoBehaviour
{
    public static CustomizeManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private List<WagonCustomize> wagonMeshes;
    [SerializeField] private List<Color> currentColors;

    private WagonCustomize currentWagonMesh;
    private string currentText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        currentWagonMesh = wagonMeshes[0];
        ChangeWagon(currentWagonMesh);
        SetColor(currentColors[0], currentColors[1]);
    }

    public void ChangeWagon(WagonCustomize wagon)
    {
        currentWagonMesh.gameObject.SetActive(false);
        currentWagonMesh = wagon;
        currentWagonMesh.SetColor(currentColors[0], currentColors[1]);
        currentWagonMesh.UpdateTexts(currentText);
        currentWagonMesh.gameObject.SetActive(true);
    }

    public void SetColor(Color PrimaryColor, Color SecondaryColor)
    {
        if (currentWagonMesh == null) return;

        currentColors[0] = PrimaryColor;
        currentColors[1] = SecondaryColor;
        currentWagonMesh.SetColor(PrimaryColor, SecondaryColor);
    }

    public void UpdateTexts(string newText)
    {
        currentText = newText;
        currentWagonMesh.UpdateTexts(newText);
    }
}
