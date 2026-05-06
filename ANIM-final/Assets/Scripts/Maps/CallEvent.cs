using UnityEngine;

[CreateAssetMenu(fileName = "CallEvent", menuName = "Scriptable Objects/Map/CallEvent")]
public class CallEvent : ScriptableObject // la classe est bateau mais à voir
{
    public string SceneName;
    public EventType eventType;
    public Sprite sprite;
}

public enum EventType { }
