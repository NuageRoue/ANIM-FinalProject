using UnityEngine;

public abstract class EventBase : MonoBehaviour
{
    public static EventBase instance { get; private set; }

    public bool isFinish { get; protected set; } = false;
    public bool isRunning { get; protected set; } = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public virtual void StartEvent()
    {
        isRunning = true;
        isFinish = false;
    }

    public virtual void EndEvent()
    {
        isRunning = false;
        isFinish = true;
    }
}
