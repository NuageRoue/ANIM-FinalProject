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

    [Header("Ability")]
    public SurvivorAbility ability;

    public void MoveTo(HexCell target)
    {
    }

    public void ApplyDamage(int amount)
    {
    }

    public void ConsumeFood(int amount)
    {
    }
}
