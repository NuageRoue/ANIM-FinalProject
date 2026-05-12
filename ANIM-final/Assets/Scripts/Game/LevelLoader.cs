using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using DigitalRuby.Tween;
using UnityEngine.UI;

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
        eventSystem.SetActive(false);
        TweenFactory.ClearTweensOnLevelLoad = false;

        System.Action<ITween<float>> onFadeOpenComplete = (t) =>
        {

            Debug.Log("the");
            GameManager.Instance.StartGame();
            SceneManager.UnloadSceneAsync("Main Menu");

            Destroy(gameObject);
        };

        System.Action<ITween<float>> onFadeCloseComplete = (t) =>
        {
            
            SceneManager.LoadSceneAsync("HexMapLoader", LoadSceneMode.Additive).completed += (_) =>
            {
                Debug.Log("wat");
                root.SetActive(false);
                GameManager.Instance.Setup(levels[index]);

                gameObject.Tween("loadLevel", 0f, 3200f, 3f, TweenScaleFunctions.CubicEaseOut,
            (t) => fader.rectTransform.sizeDelta = Vector2.one * t.CurrentValue, onFadeOpenComplete);
            };
        };


        gameObject.Tween("loadLevel", 3200f, 0f, 1f, TweenScaleFunctions.CubicEaseIn,
            (t) => fader.rectTransform.sizeDelta = Vector2.one * t.CurrentValue,
            onFadeCloseComplete
        );
    }
}