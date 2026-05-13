using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using DigitalRuby.Tween;
using UnityEngine.UI;
using System;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance { get; private set; }

    [SerializeField] private List<IslandMapData> levels;
    [SerializeField] private Image fader;
    [SerializeField] GameObject eventSystem;
    [SerializeField] GameObject root;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadLevel(int index)
    {
        eventSystem = GameObject.FindGameObjectWithTag("EventSystem");
        Debug.Log(GameObject.FindGameObjectWithTag("Fader"));
        fader = GameObject.FindGameObjectWithTag("Fader")?.GetComponent<Image>();
        root = GameObject.FindGameObjectWithTag("Root");



        eventSystem = GameObject.FindGameObjectWithTag("EventSystem");

        eventSystem?.SetActive(false);
        TweenFactory.ClearTweensOnLevelLoad = false;

        System.Action<ITween<float>> onFadeOpenComplete = (t) =>
        {
            Debug.Log("avant startDay: " + GameManager.Instance.name);
            GameManager.Instance.StartGame();
            Debug.Log("après startDay: " + GameManager.Instance.name);

            SceneManager.UnloadSceneAsync("Main Menu");

            Destroy(gameObject);
        };

        System.Action<ITween<float>> onFadeCloseComplete = (t) =>
        {
            
            SceneManager.LoadSceneAsync("HexMapLoader", LoadSceneMode.Additive).completed += (_) =>
            {
                root.SetActive(false);
                Debug.Log("test: " + GameManager.Instance.name);
                GameManager.Instance.Setup(levels[index]);
                Debug.Log("test: apres " + GameManager.Instance.name);

                gameObject.Tween("loadLevel2", 0f, 3200f, 3f, TweenScaleFunctions.CubicEaseOut,
            (t) => fader.rectTransform.sizeDelta = Vector2.one * t.CurrentValue, onFadeOpenComplete);
            };
        };


        gameObject.Tween("loadLevel", 3200f, 0f, 1f, TweenScaleFunctions.CubicEaseIn,
            (t) => fader.rectTransform.sizeDelta = Vector2.one * t.CurrentValue,
            onFadeCloseComplete
        );
    }

    public void BackToMainMenu(string current)
    {
        System.Action<ITween<float>> onFadeCloseComplete = (t) =>
        {
            root.SetActive(false);
            SceneManager.LoadSceneAsync("Main Menu", LoadSceneMode.Additive).completed += (_) => root.SetActive(false);
        };

        System.Action<ITween<float>> onFadeOpenComplete = (t) =>
        {
            SceneManager.UnloadSceneAsync(current);
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