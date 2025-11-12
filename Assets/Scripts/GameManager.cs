using System.Collections;
using System.Collections.Generic;
using TMPro;
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

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    public event System.Action<GameState> OnGameStateChanged;

    [Header("References")]
    [SerializeField] private WagonController currentWagon;
    [SerializeField] private GameObject stationsParent;
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Objective Settings")]
    [SerializeField, Range(1, 3)] private int objectiveMax;

    [Header("Gameplay Timer")]
    [SerializeField] private float maxGameTime = 120f;

    private List<Station> allStations = new List<Station>();
    private List<Station> usedStartingStations = new List<Station>();

    private List<Station> currentRoute;
    private Station startingStation;
    private Station nextStation;
    private int objectiveCount;

    private float remainingTime;
    private Coroutine gameplayTimerRoutine;

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

        GhostManager.Instance.SetMaxGhosts(allStations.Count);
    }

    private void SetupObjective()
    {
        objectiveCount = 0;
        currentRoute = new List<Station>();

        List<Station> remainingStations = new List<Station>(allStations);
        foreach (Station used in usedStartingStations)
        {
            remainingStations.Remove(used);
        }

        startingStation = remainingStations[Random.Range(0, remainingStations.Count)];
        usedStartingStations.Add(startingStation);
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
                SetState(GameState.Win);
                return;
            }

            NextObjective();
        }
    }

    public void ReactiveStartingStations(Station station)
    {
        usedStartingStations.Remove(station);
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);

        switch (newState)
        {
            case GameState.Title:
                CustomizeManager.Instance.ResetCustomize();
                break;

            case GameState.Customize:
                break;

            case GameState.Objective:
                GhostManager.Instance.DestroyLastGhostIfFull();
                SetupObjective();
                currentWagon.SetupGameplay(startingStation);
                break;

            case GameState.Gameplay:
                StartTimer();
                currentWagon.StartGameplay();
                currentWagon.BeginRecording();
                NextObjective();
                GhostManager.Instance.LaunchGhosts();
                break;

            case GameState.Win:
                StopTimer();
                WagonData resultWin = currentWagon.StopRecording();

                resultWin.startingStation = startingStation;

                resultWin.wagonName = CustomizeManager.Instance.currentText;
                resultWin.wagonMaterial = CustomizeManager.Instance.GetCurrentMaterial();
                resultWin.wagonMeshIndex = CustomizeManager.Instance.GetCurrentWagonIndex();

                GhostManager.Instance.SpawnGhost(resultWin);
                currentWagon.ResetWagon();
                break;

            case GameState.Lose:
                StopTimer();
                _ = currentWagon.StopRecording();
                currentWagon.ResetWagon();
                break;
        }
    }

    public void GoToTitle() => SetState(GameState.Title);
    public void GoToCustomize() => SetState(GameState.Customize);
    public void GoToObjective() => SetState(GameState.Objective);
    public void GoToGameplay() => SetState(GameState.Gameplay);

    private void StartTimer()
    {
        StopTimer();
        remainingTime = maxGameTime;
        gameplayTimerRoutine = StartCoroutine(GameplayTimer());
    }

    private void StopTimer()
    {
        if (gameplayTimerRoutine != null)
        {
            StopCoroutine(gameplayTimerRoutine);
            gameplayTimerRoutine = null;
        }
    }

    private IEnumerator GameplayTimer()
    {
        while (remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;
            UpdateTimerUI();
            yield return null;
        }

        if (CurrentState == GameState.Gameplay)
        {
            SetState(GameState.Lose);
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
