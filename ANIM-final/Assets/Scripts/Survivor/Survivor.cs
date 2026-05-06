using DigitalRuby.Tween;
using System;
using UnityEngine;

public enum SurvivorState
{
    Idle,
    Moving,
    InEvent
}

public class Survivor : MonoBehaviour
{
    [Header("Stats")]
    public int moveRange;
    public int foodPerTurn;
    public int visionRadius;

    [Header("Runtime")]
    public int remainingMoves;
    public HexCell currentCell;
    public SurvivorState state;

    [Header("Movement")]
    [SerializeField] private float moveDuration = 0.6f;
    [SerializeField] private float arcHeight = 1.5f;


    int _moveRange;
    public bool CanMove { get { return _moveRange > 0; }  }

    [Header("Ability")]
    public SurvivorAbility ability;

    void DecreaseMoveRange() 
    {
        _moveRange--;
    }

    public void ResetMoveRange() { 
        _moveRange = moveRange;
    }

    

    public void MoveTo(HexCell target, Action onComplete)
    {
        state = SurvivorState.Moving;

        Vector3 startPos = transform.position;
        Vector3 endPos = target.PlaceSurvivor(this);

        gameObject.Tween(
            "SurvivorMove",
            startPos,
            endPos,
            moveDuration,
            TweenScaleFunctions.SineEaseInOut,
            (t) =>
            {
                float progress = t.CurrentProgress;
                Vector3 pos = t.CurrentValue;
                pos.y += arcHeight * Mathf.Sin(progress * Mathf.PI);
                transform.position = pos;
            },
            (t) =>
            {
                DecreaseMoveRange();
                state = SurvivorState.Idle;
                onComplete?.Invoke();
            }
        );
    }

}
