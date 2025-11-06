using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Title,
    Customize,
    Objective,
    Gameplay,
    Win,
    Lose
}

public enum NbPlayers 
{
    One,
    Two
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }
    public NbPlayers CurrentNbPlayers { get; private set; }

    public event System.Action<GameState> OnGameStateChanged;

    [SerializeField] GameObject GareParent;
    public List<Gare> gares = new List<Gare>();
    public List<Gare> possibleGares = new List<Gare>();
    [SerializeField] WagonController trainController;
    [SerializeField] GameObject trainObject;
    [SerializeField] TMPro.TextMeshProUGUI objectifText;
    [SerializeField] int numberofObjectifs;
    int objectifCount = 0;

    private Gare startingGare;
    private Gare objectifGare;

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

    private void Start()
    {
        SetState(GameState.Title);

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

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);

        switch (newState)
        {
            case GameState.Title:
                break;

            case GameState.Customize:
                break;

            case GameState.Objective:
                break;

            case GameState.Gameplay:
                break;

            case GameState.Win:
                break;

            case GameState.Lose:
                break;
        }
    }

    public void GoToCustomizeWithOnePlayer()
    {
        CurrentNbPlayers = NbPlayers.One;
        SetState(GameState.Customize);
    }

    public void GoToCustomizeWithTwoPlayers()
    {
        CurrentNbPlayers = NbPlayers.Two;
        SetState(GameState.Customize);
    }

    public void GoToObjective()
    {
        SetState(GameState.Objective);
    }

    public void GoToGameplay()
    {
        SetState(GameState.Gameplay);
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
