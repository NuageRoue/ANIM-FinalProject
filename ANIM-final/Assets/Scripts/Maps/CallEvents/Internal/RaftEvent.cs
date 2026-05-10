using UnityEngine;

[CreateAssetMenu(fileName = "RaftEvent", menuName = "Scriptable Objects/CallEvents/RaftEvent")]
public class RaftEvent : CallEvent
{
    protected override void OnTrigger() 
    {
        Debug.Log($"adding a raft part to the inventory");
    }
}
