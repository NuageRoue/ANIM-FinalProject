using UnityEngine;

[CreateAssetMenu(fileName = "TowerEvent", menuName = "Scriptable Objects/CallEvents/TowerEvent")]
public class TowerEvent : CallEvent
{
    int view = 10;

    protected override void OnTrigger(Survivor s, System.Action<bool> unloadEvent)
    {
        s.currentCell.CleanFog(10);
        Debug.Log($"Unveilling a {view} radius around the tower");
        PopUp.Instance.Display("the tower reveals its surroundings.");
        unloadEvent?.Invoke(false);
    }
}
