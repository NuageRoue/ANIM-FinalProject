using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class NightController : MonoBehaviour, MainController.IMapActions
{
    public CameraMovement cam;

    public UnityEvent OnDayEnded;

    private MainController _controls;
    private bool _isBumperLeftHeld;
    private bool _isBumperRightHeld;
    private HexCell _currentCell;
    private HexCell CurrentCell
    {
        get => _currentCell;
        set 
        {
            _currentCell?.SetHighlight(HighlightVisitedLayer);
            _currentCell = value;
            _currentCell.SetHighlight(HighlightCurrent);

            cam.Track(_currentCell.transform);
        }
    }

    HashSet<HexCell> _visitedCells;

    private static int DefaultLayer;
    private static int HighlightVisitedLayer;
    private static int HighlightCurrent;

    private void Awake()
    {
        DefaultLayer = LayerMask.NameToLayer("Default");
        HighlightVisitedLayer = LayerMask.NameToLayer("HighLight_Visited");
        HighlightCurrent = LayerMask.NameToLayer("HighLight");
        _controls = new MainController();
        _controls.Map.AddCallbacks(this);
        _visitedCells = new HashSet<HexCell>();
    }

    private void OnDestroy()
    {
        _controls.Map.RemoveCallbacks(this);
        _controls.Dispose();
    }

    public void EnableControls() => _controls.Map.Enable();
    public void DisableControls() => _controls.Map.Disable();

    public void StartNightPhase(HashSet<HexCell> visitedTiles, HexCell lastVisited)
    {
        _visitedCells.UnionWith(visitedTiles);
        HighlightVisitedCells();
        CurrentCell = lastVisited;
        Debug.Log("starting the night phase");
        EnableControls();
    }

    void TileSelected() 
    {
        Debug.Log($"Tile sélectionnée : {CurrentCell.coordinates}");
        DisableControls();
        ClearVisitedCells();
        OnDayEnded?.Invoke();
    }

    // --- IMapActions implementation ---

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        Vector2 input = ctx.ReadValue<Vector2>();
        HexDirection? direction = ResolveDirection(input);
        if (direction == null) return;

        Vector3Int directionCoordinates = HexDirectionExtensions.ToCoords((HexDirection)direction);
        Vector3Int currentPos = CurrentCell.coordinates.ToVector();

        HexCell candidate = null;
        foreach (var cell in CurrentCell.Neighbors)
        {
            if (cell != null && cell.coordinates.ToVector().Equals(currentPos + directionCoordinates))
            {
                candidate = cell; break;
            }
        }
        if (candidate == null || !candidate.hasBeenExplored) return;

        CurrentCell = candidate;
        cam.Track(CurrentCell.transform);
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
        if (CurrentCell == null) return;

        TileSelected();
    }
    // --- Direction resolution ---

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

    void HighlightVisitedCells() 
    {
        foreach (var cell in _visitedCells)
        {
            cell.SetHighlight(HighlightVisitedLayer);
        }
    }

    void ClearVisitedCells()
    {
        foreach (var cell in _visitedCells)
        {
            cell.SetHighlight(DefaultLayer);
        }
    }

    public HexCell UpdateStartingCell() => CurrentCell; 
    
}