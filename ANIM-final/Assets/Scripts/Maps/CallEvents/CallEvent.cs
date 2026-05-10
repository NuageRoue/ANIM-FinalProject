using Unity.VisualScripting;
using UnityEngine;

public abstract class CallEvent : ScriptableObject
{
    public Sprite sprite;

    [SerializeField]
    bool oneTimer = true; // wether the events stays after or not (by default it disappears after)
    public bool Trigger() 
    {
        OnTrigger();

        return oneTimer;
    }

    protected abstract void OnTrigger();


}

