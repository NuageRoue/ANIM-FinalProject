using DigitalRuby.Tween;
using System;
using UnityEngine;
using UnityEngine.Windows;

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
        if (Instance != null && Instance != this) { Destroy(gameObject);
            Debug.Log("what in the fuck");  return; }
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
    [SerializeField] AudioSource musicSource;

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
                EventManager.Instance.LoadDefeatScene();
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
            PopUp.Instance.Display("you have no remaining days...");
            SetState(GameState.Defeat);
        }
    }
    #endregion

    #region Survivors
    [Header("Survivors")]
    public HexCell startingCell;
    public Survivor[] survivors;
    internal MainController _input;

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
        Debug.Log($"name: {name}");
        SetSingleton();
        // inv = new Inventory();
        _input = new MainController();
        _input.Map.Pause.performed += _ => Pause();
        _input.Enable();
    }

    public void Setup(IslandMapData map)
    {
        selectedLevel = map;

        if (map.music != null)
        {
            musicSource.clip = map.music;
            musicSource.Play();
            musicSource.loop = true;
        }
        
        startingCell = hexGrid?.Setup(selectedLevel);

        remainingDays = selectedLevel.totalDays;
        if (selectedLevel.inventory != null)
            inv = selectedLevel.inventory.Clone();

        InfoBar.Instance.SetDays(remainingDays);
        InfoBar.Instance.UpdateInventory(inv);
    }
    public void StartGame()
    {
        SetState(GameState.StartPhase);
    }


    public void AddItem(ResourceType res, int amount = 1) 
    {
        inv.AddItem(res, amount);
    }

    internal void AddRaftPart()
    {
        inv.raftParts++;
        if (inv.hasRaft)
            EventManager.Instance.LoadVictoryScene();
    }

    bool paused = false;

    public UnityEngine.Events.UnityEvent onPause;
    public UnityEngine.Events.UnityEvent onUnpause;
    public void Pause()
    {
        Debug.Log("Pause");
        if (paused) 
                DoUnPause();
        else
            DoPause();
    }

    void DoPause()
    {
        TweenFactory.DefaultTimeFunc = TweenFactory.TimeFuncUnscaledDeltaTimeFunc;
        paused = true;
        Time.timeScale = 0;

        nightController.DisableControls();
        turnController.DisableControls();
        onPause?.Invoke();
    }

    void DoUnPause() 
    {
        TweenFactory.DefaultTimeFunc = TweenFactory.TimeFuncDeltaTimeFunc;
        paused = false;
        Time.timeScale = 1;
        nightController.EnableControls();
        turnController.EnableControls();
        onUnpause?.Invoke();
    }

    #endregion

    public void EnablePause() 
    {
        _input.Enable();
    }
    public void DisablePause() 
    { 
        _input.Disable();
    }
}