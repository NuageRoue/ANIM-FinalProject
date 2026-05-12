using UnityEngine;

[CreateAssetMenu(fileName = "RaftEvent", menuName = "Scriptable Objects/CallEvents/RaftEvent")]
public class RaftEvent : CallEvent
{
    protected override void OnTrigger(Survivor s, System.Action<bool> unloadEvent) 
    {
        Debug.Log($"adding a raft part to the inventory");
        PopUp.Instance.Display("you found a part of the raft!");

        GameManager.Instance.AddRaftPart();
        unloadEvent?.Invoke(true);

        InfoBar.Instance.UpdateInventory(GameManager.Instance.inv);

    }
}
