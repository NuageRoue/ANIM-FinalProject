using UnityEngine;

public enum GameState
{
    MainMenu,
    CharacterSelect,
    DayPhase,
    EventPhase,
    NightPhase,
    Victory,
    Defeat
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    [SerializeField] private TurnController turnController;
    [SerializeField] private EventManager eventManager;

    // CampManager à brancher
    // [SerializeField] private CampManager campManager;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetState(GameState.MainMenu);
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;

        switch (newState)
        {
            case GameState.MainMenu:
                break;

            case GameState.CharacterSelect:
                break;

            case GameState.DayPhase:
                turnController.StartDay();
                break;

            case GameState.EventPhase:
                break;

            case GameState.NightPhase:
                break;

            case GameState.Victory:
                break;

            case GameState.Defeat:
                break;
        }
    }
}
