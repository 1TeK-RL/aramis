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

    [SerializeField] private GameObject stationsParent;
    [SerializeField] private WagonController currentWagon;
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private int objectiveMax;

    private List<Station> stations = new List<Station>();

    private Station startingStation;
    private Station nextStation;

    private int objectiveCount = 0;

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

        foreach (GameObject stationChild in stationsParent.transform)
        {
            Station station = stationChild.GetComponent<Station>();
            if (station != null)
            {
                stations.Add(station);
            }
        }

        if (stations.Count > 0)
        {
            int Random = UnityEngine.Random.Range(0, stations.Count);
            startingStation = stations[Random];
            stations.RemoveAt(Random);

            objectiveText.text = "Start at : " + startingStation.GetStationID();
        }
    }

    public void StartGame()
    {
        currentWagon.StartGameplay(startingStation);
        FindObjective();
    }

    private void FindObjective()
    {
        if (objectiveCount < objectiveMax - 1)
        {
            int Random = UnityEngine.Random.Range(0, stations.Count);
            nextStation = stations[Random];
            stations.RemoveAt(Random);
            objectiveText.text = "Go to : " + nextStation.GetStationID();

            objectiveCount++;
        }
        else
        {
            objectiveText.text = "Return to : " + startingStation.GetStationID();
            nextStation = startingStation;
        }

    }

    public void ArrivedInStation(int stationID)
    {
        if (stationID == nextStation.GetStationID())
        {
            FindObjective();
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
}
