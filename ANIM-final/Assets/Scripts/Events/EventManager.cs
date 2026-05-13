using DigitalRuby.Tween;
using NUnit;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    [SerializeField] private GameObject mainSceneRoot;
    [SerializeField] private GameObject mainEventSystem;
    [SerializeField] private string loadedScene;
    [SerializeField] Image fader;

    private CallEvent _currentEvent;
    private Survivor _currentSurvivor;
    private System.Action<bool> _onUnloadComplete;

    private void Awake()
    {
      // singleton
        if (Instance != null && Instance != this) { Debug.Log("what in the fuck"); Destroy(gameObject); return; }
        Instance = this;
    }

    public void TriggerEvent(CallEvent cellEvent, Survivor survivor)
    {
    }

    public void OnEventCompleted(EventResult result)
    {
    }

    public void LoadEventScene(string sceneName, Survivor s = null, System.Action<bool> unloadCompleted = null)
    {
        loadedScene = sceneName;
        _onUnloadComplete = unloadCompleted;
        GameManager.Instance.DisablePause();

        System.Action<ITween<float>> onFadeCloseComplete = (t) =>
        {
            SceneManager.LoadSceneAsync(loadedScene, LoadSceneMode.Additive).completed += (_) =>
            {
                mainSceneRoot.SetActive(false);
            };
        };

        System.Action<ITween<float>> test = null;

        System.Action<ITween<float>> onFadeOpenComplete = (t) =>
        {
            Debug.Log("chargement ok");
            GameObject.FindAnyObjectByType<EventBase>()?.StartEvent(GameManager.Instance.inv, s);
        };

        gameObject.Tween("loadScene", 3200f, 0f, 1f, TweenScaleFunctions.CubicEaseIn,
            (t) => fader.rectTransform.sizeDelta = Vector2.one * t.CurrentValue,
            onFadeCloseComplete
        ).ContinueWith(new FloatTween().Setup(0f, 0f, .25f, TweenScaleFunctions.CubicEaseOut,
        test)).ContinueWith(new FloatTween().Setup(0f, 3200f, 1f, TweenScaleFunctions.CubicEaseOut, 
        (t) => fader.rectTransform.sizeDelta = Vector2.one * t.CurrentValue,
            onFadeOpenComplete));
    }

    public void UnloadEventScene(bool result)
    {
        Debug.Log("unload event scene");
        System.Action<ITween<float>> onFadeCloseComplete = (t) =>
        {
            SceneManager.UnloadSceneAsync(loadedScene).completed += (_) =>
            {
                GameManager.Instance.UpdateFoodUI();
                mainSceneRoot.SetActive(true);
            };
        };

        System.Action<ITween<float>> onFadeOpenComplete = (t) =>
        {
            Debug.Log("unload ok");
            _onUnloadComplete?.Invoke(result);
            _onUnloadComplete = null;
            GameManager.Instance.EnablePause();
        };

        gameObject.Tween("unloadScene", 3200f, 0f, 1f, TweenScaleFunctions.CubicEaseIn,
            (t) => fader.rectTransform.sizeDelta = Vector2.one * t.CurrentValue,
            onFadeCloseComplete
        ).ContinueWith(new FloatTween().Setup(0f, 3200f, 1f, TweenScaleFunctions.CubicEaseOut,
            (t) => fader.rectTransform.sizeDelta = Vector2.one * t.CurrentValue,
            onFadeOpenComplete));
    }

    public void LoadCraftingScene(System.Action<bool> unloadCompleted)
    {
        loadedScene = "Crafting Event";
        _onUnloadComplete = unloadCompleted;
        GameManager.Instance.DisablePause();

        System.Action<ITween<float>> onFadeCloseComplete = (t) =>
        {
            SceneManager.LoadSceneAsync(loadedScene, LoadSceneMode.Additive).completed += (_) =>
            {
                mainSceneRoot.SetActive(false);
                GameManager.Instance.ResetDay();
            };
        };

        System.Action<ITween<float>> test = null;

        System.Action<ITween<float>> onFadeOpenComplete = (t) =>
        {
            Debug.Log("chargement ok");
            GameObject.FindAnyObjectByType<EventBase>()?.StartEvent(GameManager.Instance.inv);
        };

        gameObject.Tween("loadScene", 3200f, 0f, 1f, TweenScaleFunctions.CubicEaseIn,
            (t) => fader.rectTransform.sizeDelta = Vector2.one * t.CurrentValue,
            onFadeCloseComplete
        ).ContinueWith(new FloatTween().Setup(0f, 0f, .25f, TweenScaleFunctions.CubicEaseOut,
        test)).ContinueWith(new FloatTween().Setup(0f, 3200f, 1f, TweenScaleFunctions.CubicEaseOut,
        (t) => fader.rectTransform.sizeDelta = Vector2.one * t.CurrentValue,
            onFadeOpenComplete));
    }

    public void LoadDefeatScene()
    {
        loadedScene = "Defeat";
        GameManager.Instance.DisablePause();
        System.Action<ITween<float>> onFadeCloseComplete = (t) =>
        {
            mainEventSystem?.SetActive(false);
            SceneManager.LoadSceneAsync(loadedScene, LoadSceneMode.Additive).completed += (_) => SceneManager.UnloadSceneAsync("Crafting Event");
            Destroy(GameManager.Instance.gameObject);
        };

        System.Action<ITween<float>> test = null;

        System.Action<ITween<float>> onFadeOpenComplete = (t) =>
        {
            Debug.Log("chargement ok");
            SceneManager.UnloadSceneAsync("HexMapLoader");
        };

        gameObject.Tween("loadScene", 3200f, 0f, 1f, TweenScaleFunctions.CubicEaseIn,
            (t) => fader.rectTransform.sizeDelta = Vector2.one * t.CurrentValue,
            onFadeCloseComplete
        ).ContinueWith(new FloatTween().Setup(0f, 0f, .25f, TweenScaleFunctions.CubicEaseOut,
        test)).ContinueWith(new FloatTween().Setup(0f, 3200f, 1f, TweenScaleFunctions.CubicEaseOut,
        (t) => fader.rectTransform.sizeDelta = Vector2.one * t.CurrentValue,
            onFadeOpenComplete));
    }

    public void LoadVictoryScene()
    {
        loadedScene = "Victory";
        GameManager.Instance.DisablePause();
        System.Action<ITween<float>> onFadeCloseComplete = (t) =>
        {
            SceneManager.LoadSceneAsync(loadedScene, LoadSceneMode.Additive).completed += (_) =>           
            // SceneManager.UnloadSceneAsync("Crafting Event");
            mainSceneRoot.SetActive(false);
            Destroy(GameManager.Instance.gameObject);
        };

        System.Action<ITween<float>> test = null;

        System.Action<ITween<float>> onFadeOpenComplete = (t) =>
        {
            Debug.Log("chargement ok");
            SceneManager.UnloadSceneAsync("HexMapLoader");
        };

        gameObject.Tween("loadScene", 3200f, 0f, 1f, TweenScaleFunctions.CubicEaseIn,
            (t) => fader.rectTransform.sizeDelta = Vector2.one * t.CurrentValue,
            onFadeCloseComplete
        ).ContinueWith(new FloatTween().Setup(0f, 0f, .25f, TweenScaleFunctions.CubicEaseOut,
        test)).ContinueWith(new FloatTween().Setup(0f, 3200f, 1f, TweenScaleFunctions.CubicEaseOut,
        (t) => fader.rectTransform.sizeDelta = Vector2.one * t.CurrentValue,
            onFadeOpenComplete));
    }

    public void BackToMainMenu()
    {
        System.Action<ITween<float>> onFadeCloseComplete = (t) =>
        {
            SceneManager.LoadSceneAsync("Main Menu", LoadSceneMode.Additive).completed += (_) =>
            mainSceneRoot.SetActive(false);
            Destroy(GameManager.Instance.gameObject);
        };

        System.Action<ITween<float>> onFadeOpenComplete = (t) =>
        {
            SceneManager.UnloadSceneAsync("HexMapLoader");
        };

        gameObject.Tween("loadLevel", 3200f, 0f, 1f, TweenScaleFunctions.CubicEaseIn,
            (t) => fader.rectTransform.sizeDelta = Vector2.one * t.CurrentValue,
            onFadeCloseComplete
        ).ContinueWith(new FloatTween().Setup(0f, 0f, .25f, TweenScaleFunctions.Linear,
        null)).ContinueWith(new FloatTween().Setup(0f, 3200f, 1f, TweenScaleFunctions.CubicEaseOut,
            (t) => fader.rectTransform.sizeDelta = Vector2.one * t.CurrentValue,
            onFadeOpenComplete));

    }
}
