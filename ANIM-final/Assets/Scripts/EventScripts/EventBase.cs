using UnityEngine;

public abstract class EventBase : MonoBehaviour
{
    [SerializeField]
    protected Inventory inventory;

    [SerializeField]
    protected Survivor survivor;

    public static EventBase instance { get; private set; }

    public bool isCompleted { get; private set; }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        StartEvent(); // Debug
    }

    public void StartEvent(Inventory inventory = null, Survivor survivor = null)
    {
        Debug.Log("Started");

        if (inventory != null)
            this.inventory = inventory;

        if (survivor != null)
            this.survivor = survivor;
        else
        {
            this.survivor = gameObject.AddComponent<Survivor>();
        }

        InternalStartEvent();
    }

    protected void EndEvent()
    {
        InernalEndEvent();

        Debug.Log("Finished");

        // EventManager.Instance.UnloadEventScene();
    }

    protected abstract void InternalStartEvent();

    protected abstract void InernalEndEvent();

    protected void SetCompletion(bool value = true)
    {
        isCompleted = value;
    }
}
