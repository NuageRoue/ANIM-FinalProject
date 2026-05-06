using UnityEngine;
using UnityEngine.InputSystem;
using System;

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

public class TurnController : MonoBehaviour, MainController.IMapActions
{
    public event Action OnDayEnded;
    public TurnState CurrentTurnState { get; private set; }
    public CameraController cam;


    [SerializeField] private Survivor[] survivors = new Survivor[3];
    private int _activeSurvivorIndex;
    public Survivor ActiveSurvivor => survivors[_activeSurvivorIndex];

    // Input
    private MainController _controls;

    // Direction selection state
    private bool _isBumperLeftHeld;
    private bool _isBumperRightHeld;
    private HexCell _highlightedCell;

    private static int DefaultLayer;
    private static int HighlightLayer;

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

    public void EnableControls() => _controls.Map.Enable();
    public void DisableControls() => _controls.Map.Disable();

    public void SetSurvivors(Survivor[] s)
    {
        for (int i = 0; i < survivors.Length; i++) 
        {        
            survivors[i] = s[i];
        }
    }

    void ResetMoveRanges()
    {
        foreach (var survivor in survivors)
        {
            survivor.ResetMoveRange();
        }
    }

    public void StartDay()
    {
        Debug.Log("starting a new day");
        EnableControls();
        ResetMoveRanges();
        cam.SnapTo(survivors[0].currentCell.transform.position);
        SetTurnState(TurnState.Survivor1Turn);
    }

    public void EndTurn() 
    { 
        DisableControls();
        Debug.Log("turn ended");
    }

    void NextSurvivor() 
    {
        if (survivors[_activeSurvivorIndex].CanMove)
        {
            switch (_activeSurvivorIndex)
            {
                case 0:
                    SetTurnState(TurnState.Survivor1Turn);
                    break;
                case 1:
                    SetTurnState(TurnState.Survivor2Turn);
                    break;
                case 2:
                    SetTurnState(TurnState.Survivor3Turn);
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
        else
        {
            Debug.Log($"no movement available for the survivor {_activeSurvivorIndex}");
            switch (_activeSurvivorIndex)
            {
                case 0:
                    SetTurnState(TurnState.Survivor2Turn);
                    break;
                case 1:
                    SetTurnState(TurnState.Survivor3Turn);
                    break;
                case 2:
                    SetTurnState(TurnState.NightTransition);
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }

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

            case TurnState.NightTransition:
                EndTurn();
                break;

            case TurnState.InMovement:
                
                break;
        }
    }

    

    // --- IMapActions implementation ---

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (CurrentTurnState != TurnState.DirectionSelection) return;
        if (!ctx.performed) return;

        Vector2 input = ctx.ReadValue<Vector2>();
        HexDirection? direction = ResolveDirection(input);
        
        if (direction == null) return;
        Vector3Int directionCoordinates = HexDirectionExtensions.ToCoords((HexDirection)direction);
        Vector3Int currentPos = ActiveSurvivor.currentCell.coordinates.ToVector();

        Debug.Log($"on va vers le {direction}, soit vers {currentPos + directionCoordinates}");

        HexCell candidate = null;
        foreach (var cell in ActiveSurvivor.currentCell.Neighbors) 
        {
            if (cell != null && cell.coordinates.ToVector().Equals(currentPos + directionCoordinates)) 
            {
                candidate = cell; break;
            }
        }
        if (candidate == null || !candidate.IsTraversable){
            return;
        }

        SetHighlight(candidate);
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
        if (_highlightedCell == null) return;

        survivors[_activeSurvivorIndex].MoveTo(_highlightedCell, () => NextSurvivor());
        ClearHighlight();
        
        SetTurnState(TurnState.InMovement);
        
    }

    // --- Direction resolution ---

    private HexDirection? ResolveDirection(Vector2 input)
    {
        float threshold = 0.5f;

        bool isUp = input.y > threshold;
        bool isDown = input.y < -threshold;
        bool isLeft = input.x < -threshold;
        bool isRight = input.x > threshold;

        // les directions sont pétées ??
        if (isRight && !isUp && !isDown) return HexDirection.E;
        if (isLeft && !isUp && !isDown) return HexDirection.W;

        if (isUp && _isBumperRightHeld && !_isBumperLeftHeld) return HexDirection.NE;
        if (isUp && _isBumperLeftHeld && !_isBumperRightHeld) return HexDirection.NW;
        if (isDown && _isBumperRightHeld && !_isBumperLeftHeld) return HexDirection.SE;
        if (isDown && _isBumperLeftHeld && !_isBumperRightHeld) return HexDirection.SW;

        return null;
    }

    // --- Highlight helpers ---

    private void SetHighlight(HexCell cell)
    {
        Debug.Log("wat");
        ClearHighlight();
        _highlightedCell = cell;
        SetLayerRecursive(cell.gameObject, HighlightLayer);
    }

    private void ClearHighlight()
    {
        if (_highlightedCell == null) return;
        SetLayerRecursive(_highlightedCell.gameObject, DefaultLayer);
        _highlightedCell = null;
    }

    private void SetLayerRecursive(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
            child.gameObject.layer = layer;
    }
}