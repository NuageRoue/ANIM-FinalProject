using Unity.VisualScripting;
using UnityEngine;

public abstract class CallEvent : ScriptableObject
{
    public Sprite sprite;

    [SerializeField]
    bool oneTimer = true; // wether the events stays after or not (by default it disappears after)
    public bool Trigger(System.Action unloadEvent) 
    {
        OnTrigger(unloadEvent);

        return oneTimer;
    }

    protected abstract void OnTrigger(System.Action unloadEvent);


}

