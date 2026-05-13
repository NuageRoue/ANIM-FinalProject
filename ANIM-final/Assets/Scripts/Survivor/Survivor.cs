using System;
using DigitalRuby.Tween;
using UnityEngine;

public enum SurvivorState
{
    Idle,
    Moving,
    InEvent,
}

public class Survivor : MonoBehaviour
{
    #region Stats
    [Header("Stats")]
    public int moveRange;
    public int foodPerTurn;
    public int visionRadius;
    public bool hasFishingAbility = false;
    public bool hasSneakyAbility = false;
    public string sName;

    public bool isHurt = false; // si battu par un ennemi, son tour prend fin + ne joue pas au tour suivant
    public bool isIncapacitated = false; // si coinc� dans un trou, son tour prend fin

    public bool hasStrongAbility = false;
    public int id;

    #endregion

    #region Runtime
    [Header("Runtime")]
    public HexCell currentCell;
    public SurvivorState state;
    #endregion

    #region Move Range
    [SerializeField]
    private int _moveRange;

    public bool CanMove()
    {
        if (_moveRange <= 0)
            return false;

        if (isIncapacitated)
        {
            isIncapacitated = false;
            return false;
        }

        if (isHurt)
        {
            if (_moveRange == moveRange)
                isHurt = false;
            return false;
        }

        return true;
    }
    public void ResetMoveRange() 
    {
        _moveRange = moveRange;
        isIncapacitated = false; // in case we are incapacitated at the end of our moveRange
    }
    private void DecreaseMoveRange() => _moveRange--;
    #endregion

    #region Movement
    [Header("Movement")]
    [SerializeField]
    private float moveDuration = 0.6f;

    [SerializeField]
    private float arcHeight = 1.5f;

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
                currentCell.CleanFog(visionRadius);
            }
        );
    }
    #endregion

    #region Ability
    [Header("Ability")]
    public SurvivorAbility ability;
    #endregion
}
