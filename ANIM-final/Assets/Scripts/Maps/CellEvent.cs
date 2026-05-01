using UnityEngine;

public abstract class CellEvent : MonoBehaviour // la classe est bateau mais à voir
{
    public abstract bool CanTrigger(Survivor survivor);

    public abstract EventResult Trigger(Survivor survivor, EventContext context);

    public abstract string GetSceneToLoad();
}
