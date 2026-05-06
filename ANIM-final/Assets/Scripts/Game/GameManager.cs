using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public enum GameState
{
    StartPhase,
    DayPhase,
    NightPhase,
    Victory,
    Defeat
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    [Header("managers")]
    [SerializeField] private TurnController turnController;
    [SerializeField] private EventManager eventManager;
    [SerializeField] private HexGrid hexGrid;

    // CampManager à brancher
    // [SerializeField] private CampManager campManager;

    [Header("Survivors")]
    public HexCell startingCell;
    public Survivor[] survivors;


    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetState(GameState.StartPhase);
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        switch (CurrentState)
        {
            case GameState.StartPhase:
                StartPhase();            
                break;

            case GameState.DayPhase:
                turnController.StartDay();
                break;

            case GameState.NightPhase:
                break;

            case GameState.Victory:
                break;

            case GameState.Defeat:
                break;
        }
    }

    void StartPhase() 
    {
        startingCell = hexGrid?.Setup();
        InstantiateSurvivors();
        turnController.SetSurvivors(survivors);
        UpdateState();
    }

    void UpdateState()
    {
        switch (CurrentState)
        {
            case GameState.StartPhase:
                SetState(GameState.DayPhase);
                break;

            case GameState.DayPhase:
                turnController?.StartDay();
                break;

            case GameState.NightPhase:
                break;

            case GameState.Victory:
                break;

            case GameState.Defeat:
                break;
        }
    }

    void InstantiateSurvivors() 
    {
        Debug.Log("instantiating survivors");
        for (int i = 0; i < survivors.Length; i++)
        {
            survivors[i] = Instantiate(survivors[i], Vector3.zero, Quaternion.identity);
            
        }
        startingCell.PlaceSurvivors(survivors);
    }
}
