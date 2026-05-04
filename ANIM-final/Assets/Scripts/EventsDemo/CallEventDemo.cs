using JetBrains.Annotations;
using UnityEngine;

public abstract class CallEventDemo : MonoBehaviour
{
    public static CallEventDemo instance { get; private set; }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public abstract void StartEvent(EventContextDemo context);

    public void EndEvent(EventResultDemo result)
    {
        EventManagerDemo.instance.OnEventCompleted(result);
    }
}
