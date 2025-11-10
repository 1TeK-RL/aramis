using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    [Header("References")]
    [SerializeField] private WagonController currentWagon;
    [SerializeField] private GameObject stationsParent;
    [SerializeField] private TextMeshProUGUI objectiveText;

    [Header("Objective Settings")]
    [SerializeField, Range(1, 3)] private int objectiveMax;
    
    private List<Station> allStations = new List<Station>();

    private List<Station> currentRoute;
    private Station startingStation;
    private Station nextStation;
    private int objectiveCount;

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

        foreach (Transform stationChild in stationsParent.transform)
        {
            Station station = stationChild.GetComponent<Station>();
            if (station != null)
            {
                allStations.Add(station);
            }
        }
    }

    private void SetupObjective()
    {
        objectiveCount = 0;
        currentRoute = new List<Station>();

        startingStation = allStations[Random.Range(0, allStations.Count)];
        currentRoute.Add(startingStation);

        List<Station> availableStations = new List<Station>(allStations);
        availableStations.Remove(startingStation);

        int steps = Mathf.Clamp(objectiveMax, 1, availableStations.Count);
        for (int i = 0; i < steps; i++)
        {
            int randomIndex = Random.Range(0, availableStations.Count);
            Station next = availableStations[randomIndex];
            currentRoute.Add(next);
            availableStations.RemoveAt(randomIndex);
        }

        currentRoute.Add(startingStation);

        objectiveText.text = "Start at : " + startingStation.GetStationID();
    }

    private void NextObjective()
    {
        objectiveCount++;

        if (objectiveCount < currentRoute.Count)
        {
            nextStation = currentRoute[objectiveCount];
            string prefix = (objectiveCount == currentRoute.Count - 1) ? "Return to : " : "Go to : ";
            objectiveText.text = prefix + nextStation.GetStationID();
        }
    }

    public void ArrivedInStation(int stationID)
    {
        if (nextStation == null) return;

        if (stationID == nextStation.GetStationID())
        {
            if (nextStation == startingStation && objectiveCount == currentRoute.Count - 1)
            {
                objectiveText.text = "You returned to start!";
                SetState(GameState.Win);
                return;
            }

            NextObjective();
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
                WagonPathing result = currentWagon.StopRecording();

                GhostManager.Instance.SpawnGhost(result);

                break;

            case GameState.Lose:
                break;
        }
    }

    public void GoToTitle()
    {
        SetState(GameState.Title);
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
        SetupObjective();

        SetState(GameState.Objective);
    }

    public void GoToGameplay()
    {
        SetState(GameState.Gameplay);

        currentWagon.BeginRecording();
        currentWagon.StartGameplay(startingStation);

        NextObjective();
    }
}
