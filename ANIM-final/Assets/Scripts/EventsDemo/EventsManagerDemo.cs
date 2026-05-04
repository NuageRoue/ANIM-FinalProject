using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class EventManagerDemo : MonoBehaviour
{
    public static EventManagerDemo instance { get; private set; }

    public EventContextDemo currentContext { get; private set; }
    public EventResultDemo currentResult { get; private set; }
    private string mainSceneName;

    private void Awake()
    {
        // singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        currentContext = new EventContextDemo { value = 0 };
        currentResult = null;
        mainSceneName = SceneManager.GetActiveScene().name;

        instance = this;
        DontDestroyOnLoad(this);
    }

    public async void TriggerEvent(EventTarget targetEvent, EventContextDemo context = null)
    {
        currentContext =
            context != null ? context : new EventContextDemo { value = currentContext.value };

        await SceneManager.LoadSceneAsync(targetEvent.sceneName);
        CallEventDemo.instance.StartEvent(currentContext);
    }

    public void OnEventCompleted(EventResultDemo result)
    {
        SceneManager.LoadScene(mainSceneName);

        currentResult = result;

        Assert.IsTrue(currentResult != null);
        currentContext.value += currentResult.add;
        currentContext.value -= currentResult.sub;
    }
}
