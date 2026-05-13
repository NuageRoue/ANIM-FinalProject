using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

public enum TurnState
{
    Survivor1Turn,
    Survivor2Turn,
    Survivor3Turn,
    DirectionSelection,
    InMovement,
    Event,
    NightTransition
}

public class DayController : MonoBehaviour, MainController.IMapActions
{
    #region References
    public CameraMovement cam;
    [SerializeField] private NightController nightController;
    [SerializeField] private Survivor[] survivors = new Survivor[3];
    #endregion

    #region State
    public TurnState CurrentTurnState { get; private set; }
    private int _activeSurvivorIndex;
    public Survivor ActiveSurvivor => survivors[_activeSurvivorIndex];
    private HashSet<HexCell> visitedCells = new HashSet<HexCell>();
    #endregion

    #region Input
    private MainController _controls;
    private bool _isBumperLeftHeld;
    private bool _isBumperRightHeld;
    #endregion

    #region Highlight
    private static int DefaultLayer;
    private static int HighlightLayer;

    private HexCell _highlightedCell;
    private HexCell HighlightedCell
    {
        get => _highlightedCell;
        set
        {
            _highlightedCell?.SetHighlight(DefaultLayer);
            _highlightedCell = value;
            _highlightedCell?.SetHighlight(HighlightLayer);
        }
    }

    public HexCell LastVisitedCell => _highlightedCell;
    #endregion

    #region Lifecycle
    private void Awake()
    {
        DefaultLayer = LayerMask.NameToLayer("Default");
        HighlightLayer = LayerMask.NameToLayer("HighLight");
        _controls = new MainController();
        _controls.Map.AddCallbacks(this);
    }

    private void OnDestroy()
    {
        _controls.Map.RemoveCallbacks(this);
        _controls.Dispose();
    }
    #endregion

    #region Controls
    private bool _blockedByPause = false;

    public void DisableControls()
    {
        if (!_controls.Map.enabled) _blockedByPause = true;
        _controls.Map.Disable();
    }

    public void EnableControls()
    {
        if (_blockedByPause) { _blockedByPause = false; return; }
        _controls.Map.Enable();
    }
    public void SetSurvivors(Survivor[] s)
    {
        for (int i = 0; i < survivors.Length; i++)
            survivors[i] = s[i];
    }
    #endregion

    #region Day Flow
    public void StartDay()
    {
        Debug.Log("Starting a new day");
        EnableControls();
        ResetMoveRanges();
        cam.SnapTo(survivors[0].currentCell.transform.position);
        visitedCells.Add(survivors[0].currentCell);
        SelectSurvivor(-1);
    }

    public void EndTurn()
    {
        DisableControls();
        Debug.Log("Turn ended");
        PopUp.Instance.Display("Where will you camp tonight?");
        nightController.StartNightPhase(visitedCells, ActiveSurvivor.currentCell);
    }

    void ResetMoveRanges()
    {
        foreach (var survivor in survivors)
            survivor.ResetMoveRange();
    }
    #endregion

    #region Turn State
    private void SetTurnState(TurnState newState)
    {
        CurrentTurnState = newState;
        switch (CurrentTurnState)
        {
            case TurnState.Survivor1Turn:
                _activeSurvivorIndex = 0;
                SetTurnState(TurnState.DirectionSelection);
                break;
            case TurnState.Survivor2Turn:
                _activeSurvivorIndex = 1;
                SetTurnState(TurnState.DirectionSelection);
                break;
            case TurnState.Survivor3Turn:
                _activeSurvivorIndex = 2;
                SetTurnState(TurnState.DirectionSelection);
                break;
            case TurnState.DirectionSelection:
                cam.Track(survivors[_activeSurvivorIndex].transform);
                break;
            case TurnState.InMovement:
                survivors[_activeSurvivorIndex].MoveTo(HighlightedCell, () => PerformEvent());
                HighlightedCell = null;
                break;
            case TurnState.NightTransition:
                EndTurn();
                break;
        }
    }


    void SelectSurvivor(int currentIndex)
    {
        // Debug.Log($"index: {currentIndex}, perso: {survivors[currentIndex].CanMove()}");
        if (currentIndex >= 0 && survivors[currentIndex].CanMove()) // si on est au début ou que le perso peut se déplacer
        {
            switch (currentIndex)
            {
                case 0: SetTurnState(TurnState.Survivor1Turn); break; //sinon, le perso considéré peut se déplacer : on va sur ce perso
                case 1: SetTurnState(TurnState.Survivor2Turn); break;
                case 2: SetTurnState(TurnState.Survivor3Turn); break;
                default: throw new IndexOutOfRangeException();
            }
        }
        else // si le perso considéré peut pas se déplacer : on va au suivant
        {
            switch (currentIndex)
            {
                case -1:
                case 0: 
                case 1:
                    PopUp.Instance.Display($"{survivors[currentIndex + 1].sName}'s turn");
                    SelectSurvivor(currentIndex + 1); break;
                case 2: SetTurnState(TurnState.NightTransition); break; // ou on skip à la nuit si on peut pas
                default: throw new IndexOutOfRangeException();
            }
        }
    }

    void PerformEvent()
    {
        if (survivors[_activeSurvivorIndex].currentCell.HasEvent())
        {
            DisableControls();
            ActiveSurvivor.currentCell.CallEvent(ActiveSurvivor, (bool b) => EndEvent(b));
        }
        else 
        {
            SelectSurvivor(_activeSurvivorIndex);
        }
    }

    void EndEvent(bool eventFinished)
    {
        Debug.Log("event completed");
        if (eventFinished)
            ActiveSurvivor.currentCell.ClearEvent();
        EnableControls();
        SelectSurvivor(_activeSurvivorIndex);
    }
    #endregion

    #region Input Handlers
    public void OnPause(InputAction.CallbackContext context) { }
    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (CurrentTurnState != TurnState.DirectionSelection) return;
        if (!ctx.performed) return;

        Vector2 input = ctx.ReadValue<Vector2>();
        HexDirection? direction = ResolveDirection(input);
        if (direction == null) return;

        Vector3Int directionCoordinates = HexDirectionExtensions.ToCoords((HexDirection)direction);
        Vector3Int currentPos = ActiveSurvivor.currentCell.coordinates.ToVector();

        HexCell candidate = null;
        foreach (var cell in ActiveSurvivor.currentCell.Neighbors)
        {
            if (cell != null && cell.coordinates.ToVector().Equals(currentPos + directionCoordinates))
            { candidate = cell; break; }
        }
        if (candidate == null || !candidate.IsTraversable) return;

        HighlightedCell = candidate;
    }

    public void OnMoveModifier(InputAction.CallbackContext ctx)
    {
        float value = ctx.ReadValue<float>();
        _isBumperLeftHeld = value < -0.5f;
        _isBumperRightHeld = value > 0.5f;
    }

    public void OnConfirmationMovement(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (CurrentTurnState != TurnState.DirectionSelection) return;
        if (HighlightedCell == null) return;
        if (HighlightedCell == survivors[_activeSurvivorIndex].currentCell) return;

        visitedCells.Add(HighlightedCell);
        SetTurnState(TurnState.InMovement);
    }

    private HexDirection? ResolveDirection(Vector2 input)
    {
        float threshold = 0.5f;
        bool isUp = input.y > threshold;
        bool isDown = input.y < -threshold;
        bool isLeft = input.x < -threshold;
        bool isRight = input.x > threshold;

        if (isRight && !isUp && !isDown) return HexDirection.E;
        if (isLeft && !isUp && !isDown) return HexDirection.W;
        if (isUp && _isBumperRightHeld && !_isBumperLeftHeld) return HexDirection.NE;
        if (isUp && _isBumperLeftHeld && !_isBumperRightHeld) return HexDirection.NW;
        if (isDown && _isBumperRightHeld && !_isBumperLeftHeld) return HexDirection.SE;
        if (isDown && _isBumperLeftHeld && !_isBumperRightHeld) return HexDirection.SW;

        return null;
    }
    #endregion
}