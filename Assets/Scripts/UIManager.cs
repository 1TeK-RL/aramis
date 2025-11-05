using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] GameObject init_UI;
    [SerializeField] GameObject customize_UI;
    [SerializeField] GameObject objective_UI;
    [SerializeField] GameObject gameplay_UI;
    [SerializeField] GameObject win_UI;
    [SerializeField] GameObject lose_UI;

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
    }

    public void HandleGameStateChanged(GameState state)
    {
        init_UI.SetActive(state == GameState.Init);
        customize_UI.SetActive(state == GameState.Customize);
        objective_UI.SetActive(state == GameState.Objective);
        gameplay_UI.SetActive(state == GameState.Gameplay);
        win_UI.SetActive(state == GameState.Win);
        lose_UI.SetActive(state == GameState.Lose);
    }
}
