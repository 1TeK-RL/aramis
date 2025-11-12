using UnityEngine;

public enum CalibrationState
{
    NotCalibrated,
    Calibrating,
    Calibrated
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] GameObject title_Canvas;
    [SerializeField] GameObject customize_Canvas;
    [SerializeField] GameObject objective_Canvas;
    [SerializeField] GameObject gameplay_Canvas;
    [SerializeField] GameObject win_Canvas;
    [SerializeField] GameObject lose_Canvas;

    public CalibrationState CurrentCalibrationState { get; set; }

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

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        }
    }

    private void Start()
    {
        HandleGameStateChanged(GameManager.Instance.CurrentState);
        SetCalibrationState(CalibrationState.NotCalibrated);
    }

    public void HandleGameStateChanged(GameState state)
    {
        title_Canvas.SetActive(state == GameState.Title);
        customize_Canvas.SetActive(state == GameState.Customize);
        objective_Canvas.SetActive(state == GameState.Objective);
        gameplay_Canvas.SetActive(state == GameState.Gameplay);
        win_Canvas.SetActive(state == GameState.Win);
        lose_Canvas.SetActive(state == GameState.Lose);
    }

    public void SetCalibrationState(CalibrationState newState)
    {
        CurrentCalibrationState = newState;
    }
}
