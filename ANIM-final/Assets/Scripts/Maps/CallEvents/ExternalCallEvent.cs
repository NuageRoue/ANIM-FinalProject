using UnityEngine;

[CreateAssetMenu(fileName = "ExternalCallEvent", menuName = "Scriptable Objects/CallEvents/ExternalCallEvent")]
public class ExternalCallEvent : CallEvent
{
    [SerializeField]
    string sceneName;
    protected override void OnTrigger()
    {
        Debug.Log($"launching the scene {sceneName}");
    }
}
