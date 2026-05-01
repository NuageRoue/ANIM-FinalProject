using UnityEngine;
using System;

public enum TurnState
{
    Survivor1Turn,
    Survivor2Turn,
    Survivor3Turn,
    NightTransition
}

public class TurnController : MonoBehaviour
{
    public event Action OnDayEnded;

    public TurnState CurrentTurnState { get; private set; }

    [SerializeField] private Survivor[] survivors = new Survivor[3];

    private int _activeSurvivorIndex;

    public Survivor ActiveSurvivor => survivors[_activeSurvivorIndex];

    public void StartDay()
    {
        _activeSurvivorIndex = 0;
        SetTurnState(TurnState.Survivor1Turn);
    }

    public void EndTurn()
    {
        NextSurvivor();
    }

    public void NextSurvivor()
    {
        _activeSurvivorIndex++;

        if (_activeSurvivorIndex >= survivors.Length)
        {
            SetTurnState(TurnState.NightTransition);
            return;
        }

        SetTurnState((TurnState)_activeSurvivorIndex);
    }

    private void SetTurnState(TurnState newState)
    {
        CurrentTurnState = newState;

        switch (newState)
        {
            case TurnState.Survivor1Turn:
            case TurnState.Survivor2Turn:
            case TurnState.Survivor3Turn:
                break;

            case TurnState.NightTransition:
                break;
        }
    }
}
