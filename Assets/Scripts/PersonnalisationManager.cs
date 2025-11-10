using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class PersonnalisationManager : MonoBehaviour
{
    public static PersonnalisationManager Instance { get; private set; }

    [Header("Référence du wagon à personnaliser")]
    
    public List<WagonCustomize> wagons; // le MeshRenderer ou SkinnedMeshRenderer du wagon
    private WagonCustomize currentWagon;
    public List<Color> currentColors;
    public string currentText;

   

    // Fonction appelée par les boutons

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
        currentWagon = wagons[0];
        ChangeWagon(currentWagon);
        SetColor(currentColors[0], currentColors[1]);

    }

    public void ChangeWagon(WagonCustomize wagon)
    {
        currentWagon.gameObject.SetActive(false);
        currentWagon = wagon;
        currentWagon.SetColor(currentColors[0], currentColors[1]);
        currentWagon.UpdateTexts(currentText);
        currentWagon.gameObject.SetActive(true);
    }

    public void SetColor(Color PrimaryColor, Color SecondaryColor)
    {
        if (currentWagon == null) return;

        currentColors[0] = PrimaryColor;
        currentColors[1] = SecondaryColor;
        currentWagon.SetColor(PrimaryColor, SecondaryColor);

    }



    public void UpdateTexts(string newText)
    {
        currentText = newText;
        currentWagon.UpdateTexts(newText);
    }

}
