using UnityEngine;

[CreateAssetMenu(fileName = "TowerEvent", menuName = "Scriptable Objects/CallEvents/TowerEvent")]
public class TowerEvent : CallEvent
{
    int view = 10;

    protected override void OnTrigger()
    {
        Debug.Log($"Unveilling a {view} radius around the tower");
    }
}
