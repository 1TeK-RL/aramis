using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class GameManagerv2 : MonoBehaviour
{
    [SerializeField] GameObject GareParent;
    public  List<Gare> gares = new List<Gare>();
    public List<Gare> possibleGares = new List<Gare>();
    [SerializeField] TrainController trainController;
    [SerializeField] GameObject trainObject;
    [SerializeField] TMPro.TextMeshProUGUI objectifText;
    [SerializeField] int numberofObjectifs;
    int objectifCount = 0;

    private Gare startingGare;
    private Gare objectifGare;

    private void Start()
    {
        foreach (Transform child in GareParent.transform)
        {
            Gare gare = child.GetComponent<Gare>();
            if (gare != null)
            {
                gares.Add(gare);
                possibleGares.Add(gare);
            }
        }

        if (gares.Count > 0)
        {
            int Random = UnityEngine.Random.Range(0, gares.Count);
            startingGare = gares[Random];
            objectifText.text = "Start at : " + startingGare.gareID;
            possibleGares.RemoveAt(Random);


        }
       

    }

    public void StartGame()
    {
        trainController.startingGare = startingGare;
        trainController.startingSplineID = startingGare.StartingSpline;
        trainController.StartGame();
        FindObjectif();
    }

    public void FindObjectif()
    {
        if (objectifCount < numberofObjectifs - 1)
        {
            int Random = UnityEngine.Random.Range(0, possibleGares.Count);
            objectifGare = possibleGares[Random];
            possibleGares.RemoveAt(Random);
            objectifText.text = "Go to : " + objectifGare.gareID;
          
            objectifCount++;
        }
        else
        {
            objectifText.text = "Return to : " + startingGare.gareID;
            objectifGare = startingGare;
        }

    }

    public void ArriverGare(int gareID)
    {
        if (gareID == objectifGare.gareID)
        {
            FindObjectif();
        }
    }
}
