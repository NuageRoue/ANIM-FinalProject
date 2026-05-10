using UnityEngine;

[CreateAssetMenu(fileName = "ResourceEvent", menuName = "Scriptable Objects/CallEvents/ResourceEvent")]
public class ResourceEvent : CallEvent
{
    [SerializeField]
    ResourceType resourceType;
    int amount = 1;

    protected override void OnTrigger(System.Action unloadEvent)
    {
        Debug.Log($"adding {amount} {resourceType} to the inventory");
        GameManager.Instance.AddItem(resourceType, amount);
        unloadEvent?.Invoke();
    }
}
