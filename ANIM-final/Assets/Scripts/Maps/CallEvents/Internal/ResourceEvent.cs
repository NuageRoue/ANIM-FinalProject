using NUnit;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceEvent", menuName = "Scriptable Objects/CallEvents/ResourceEvent")]
public class ResourceEvent : CallEvent
{
    [SerializeField]
    ResourceType resourceType;
    int amount = 1;

    protected override void OnTrigger(Survivor s, System.Action<bool> unloadEvent)
    {
        Debug.Log($"adding {amount} {resourceType} to the inventory");
        GameManager.Instance.AddItem(resourceType, amount);
        PopUp.Instance.Display($"you found one {resourceType}!");
        unloadEvent?.Invoke(true);

        InfoBar.Instance.UpdateInventory(GameManager.Instance.inv);
    }
}
