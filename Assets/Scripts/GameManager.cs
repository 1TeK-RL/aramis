using UnityEngine;

public enum GameState { Init, Customize, Objective, Gameplay, Win, Lose }
public enum NbPlayers { One, Two }
public enum CalibrationState
{
    NotCalibrated,
    Calibrating,
    Calibrated
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    public NbPlayers CurrentNbPlayers { get; set; }

    public CalibrationState CurrentCalibrationState { get; set; }

    public event System.Action<GameState> OnGameStateChanged;


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
        SetState(GameState.Init);
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;

        Debug.Log($"Game state changed to: {newState}");

        // Notify listeners (UIManager, AudioManager, etc.)
        OnGameStateChanged?.Invoke(newState);

        // Optional internal logic (if GameManager needs to react)
        switch (newState)
        {
            case GameState.Init:
                HandleInit();
                break;

            case GameState.Customize:
                HandleCustomize();
                break;

            case GameState.Objective:
                HandleObjective();
                break;

            case GameState.Gameplay:
                HandleGameplay();
                break;

            case GameState.Win:
                HandleWin();
                break;

            case GameState.Lose:
                HandleLose();
                break;
        }
    }

    public void GoToCustomizeWithOnePlayer()
    {
        SetState(GameState.Customize);
        CurrentNbPlayers = NbPlayers.One;
    }

    public void GoToCustomizeWithTwoPlayers()
    {
        SetState(GameState.Customize);
        CurrentNbPlayers = NbPlayers.Two;
    }

    public void GoToObjective()
    {
        SetState(GameState.Objective);
    }

    public void GoToGameplay()
    {
        SetState(GameState.Gameplay);
    }


    #region Handlers (optional to keep logic separated)
    private void HandleInit()
    {
        // Example: load profile, setup managers, preload resources...
    }

    private void HandleCustomize()
    {
        // Example: show customization UI
    }

    private void HandleObjective()
    {
        // Example: show objective popup
    }

    private void HandleGameplay()
    {
        // Example: start timers, spawn enemies, etc.
    }

    private void HandleWin()
    {
        // Example: stop gameplay, show win screen
    }

    private void HandleLose()
    {
        // Example: stop gameplay, show lose screen
    }
    #endregion
}
