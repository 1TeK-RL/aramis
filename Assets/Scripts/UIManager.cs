using UnityEngine;
using UnityEngine.UI;

public enum CalibrationState
{
    NotCalibrated,
    Calibrated
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Canvases")]
    [SerializeField] GameObject title_Canvas;
    [SerializeField] GameObject customize_Canvas;
    [SerializeField] GameObject objective_Canvas;
    [SerializeField] GameObject gameplay_Canvas;
    [SerializeField] GameObject win_Canvas;
    [SerializeField] GameObject lose_Canvas;

    [Header("Objective Icons")]
    [SerializeField] Sprite[] ObjectiveIcons;

    [Header("Step Icons")]
    [SerializeField] Sprite[] StepIcons;

    [Header("Objective UI")]
    [SerializeField] Image StartingObjIcon;
    [SerializeField] Image NextObjIcon;
    [SerializeField] Image Step;

    [Header("Calibration Buttons")]
    [SerializeField] Image CalibrationButtonObjective;
    [SerializeField] Image CalibrationButtonGameplay;
    [SerializeField] Sprite NotCalibrated;
    [SerializeField] Sprite Calibrated;

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

        switch (newState)
        {
            case CalibrationState.NotCalibrated:
                CalibrationButtonObjective.sprite = NotCalibrated;
                CalibrationButtonGameplay.sprite = NotCalibrated;
                break;

            case CalibrationState.Calibrated:
                CalibrationButtonObjective.sprite = Calibrated;
                CalibrationButtonGameplay.sprite = Calibrated;
                break;
        }
    }

    public void SetStartingStationIcon(int stationID)
    {
        StartingObjIcon.sprite = ObjectiveIcons[stationID];
    }

    public void SetNextStationIcon(int stationID)
    {
        NextObjIcon.sprite = ObjectiveIcons[stationID];
    }

    public void SetStepIcon(int stepIndex)
    {
        Step.sprite = StepIcons[stepIndex];
    }
}
