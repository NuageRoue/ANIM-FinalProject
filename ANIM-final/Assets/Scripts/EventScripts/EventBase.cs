using UnityEngine;

public abstract class EventBase : MonoBehaviour
{
    public static EventBase instance { get; private set; }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public abstract void StartEvent();

    public abstract void EndEvent();
}
