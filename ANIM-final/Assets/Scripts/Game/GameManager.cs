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

    [SerializeField]
    Transform root;

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

    [SerializeField] public Inventory inv;
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
            survivors[i] = Instantiate(survivors[i], Vector3.zero, Quaternion.identity, root);
    }

    void PlaceSurvivorsAtStart()
    {
        startingCell.PlaceSurvivors(survivors);
    }
    #endregion

    #region Phases

    void StartPhase()
    {
        InstantiateSurvivors();
        turnController.SetSurvivors(survivors);
    }

    public void EndDay()
    {
        Debug.Log("Day ended");
        EventManager.Instance.LoadCraftingScene(unloadCompleted: (bool b) => OnNightPhaseEnded(b));
    }


    public void OnNightPhaseEnded(bool gameFinished)
    {
        startingCell = nightController.UpdateStartingCell();
        UpdateState();
    }

    public void UpdateDayUI() 
    {
        InfoBar.Instance.IncreaseDay();
    }

    public void UpdateFoodUI()
    {
        InfoBar.Instance.UpdateInventory(inv);
    }
    public void ResetDay()
    {
        UpdateDayUI();
        startingCell = nightController.UpdateStartingCell();
        PlaceSurvivorsAtStart();
    }

    #endregion


    #region startup

    public IslandMapData selectedLevel;
    private void Awake()
    {
        SetSingleton();

        // inv = new Inventory();
    }

    void Start()
    {
        startingCell = hexGrid?.Setup(selectedLevel);

        remainingDays = selectedLevel.totalDays;

        InfoBar.Instance.SetDays(remainingDays);
        InfoBar.Instance.UpdateInventory(inv);

        SetState(GameState.StartPhase);
    }


    public void AddItem(ResourceType res, int amount = 1) 
    {
        inv.AddItem(res, amount);
    }
    

    #endregion
}