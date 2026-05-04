using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnClick : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;

    public void SwitchScene1()
    {
        EventManagerDemo.instance.TriggerEvent(new EventTarget { sceneName = "Event1" });
    }

    public void SwitchScene2()
    {
        EventManagerDemo.instance.TriggerEvent(new EventTarget { sceneName = "Event2" });
    }

    void Update()
    {
        var context = EventManagerDemo.instance.currentContext;

        if (context != null)
        {
            text.SetText(string.Format("Current amount {0}", context.value));
        }
    }
}
