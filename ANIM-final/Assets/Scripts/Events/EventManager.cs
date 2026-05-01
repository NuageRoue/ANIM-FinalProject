using UnityEngine;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    private CallEvent _currentEvent;
    private Survivor _currentSurvivor;

    private void Awake()
    {
      // singleton
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void TriggerEvent(CallEvent cellEvent, Survivor survivor)
    {
    }

    public void OnEventCompleted(EventResult result)
    {
    }
}
