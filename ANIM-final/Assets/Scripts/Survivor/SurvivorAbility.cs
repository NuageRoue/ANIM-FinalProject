using UnityEngine;

public abstract class SurvivorAbility : MonoBehaviour
{
    public virtual void OnTurnStart(Survivor survivor) { }

    public virtual void OnMove(Survivor survivor, HexCell target) { }

    public virtual void OnEventEntered(Survivor survivor, CallEvent callEvent) { }

    public virtual bool CanUseSpecialAction(Survivor survivor) => false;

    public virtual void UseSpecialAction(Survivor survivor) { }
}
