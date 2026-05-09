using UnityEngine;

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
    #region Singleton
    public static GameManager Instance { get; private set; }

    private void SetSingleton()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region References
    [Header("Managers")]
    [SerializeField] private DayController turnController;
    [SerializeField] private NightController nightController;
    [SerializeField] private EventManager eventManager;
    [SerializeField] private HexGrid hexGrid;
    // [SerializeField] private CampManager campManager;
    #endregion

    #region State
    public GameState CurrentState { get; private set; }
    int remainingDays;

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        switch (CurrentState)
        {
            case GameState.StartPhase:
                StartPhase();
                UpdateState();
                break;
            case GameState.DayPhase:
                DayPhase();
                break;
            case GameState.NightPhase:
                remainingDays--;
                UpdateState();
                break;
            case GameState.Victory:
                break;
            case GameState.Defeat:
                break;
        }
    }

    public void UpdateState()
    {
        switch (CurrentState)
        {
            case GameState.StartPhase: SetState(GameState.DayPhase); break;
            case GameState.DayPhase: SetState(GameState.NightPhase); break;
            case GameState.NightPhase: SetState(GameState.DayPhase); break;
        }
    }

    void DayPhase() 
    {
        if (remainingDays > 0)
        {
            Debug.Log($"il reste {remainingDays} jours");
            PlaceSurvivorsAtStart();
            turnController.StartDay();
        }
        else
        {
            Debug.Log("plus de jours...");
            SetState(GameState.Defeat);
        }
    }
    #endregion

    #region Survivors
    [Header("Survivors")]
    public HexCell startingCell;
    public Survivor[] survivors;

    void InstantiateSurvivors()
    {
        Debug.Log("Instantiating survivors");
        for (int i = 0; i < survivors.Length; i++)
            survivors[i] = Instantiate(survivors[i], Vector3.zero, Quaternion.identity);
    }

    void PlaceSurvivorsAtStart()
    {
        startingCell.PlaceSurvivors(survivors);
    }
    #endregion

    #region Phases
    private void Start() => SetState(GameState.StartPhase);

    void StartPhase()
    {
        InstantiateSurvivors();
        turnController.SetSurvivors(survivors);
    }

    public void EndDay()
    {
        Debug.Log("Day ended");
        startingCell = nightController.UpdateStartingCell();
        UpdateState();
    }
    #endregion


    #region startup

    public IslandMapData selectedLevel;
    private void Awake()
    {
        SetSingleton();

        startingCell = hexGrid?.Setup(selectedLevel);

        remainingDays = selectedLevel.totalDays;

    }

    #endregion
}