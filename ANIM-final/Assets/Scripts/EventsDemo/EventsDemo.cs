using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventDemo : MonoBehaviour
{
    [System.Serializable]
    public class PassedData
    {
        public Color color;
    }

    public static EventDemo Instance { get; private set; }

    [SerializeField]
    public List<SceneAsset> scenes;
    int sceneIndex = -1;

    [SerializeField]
    public PassedData data;

    private void Awake()
    {
        // singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(this);

        NextScene();
    }

    public void NextScene()
    {
        sceneIndex = (sceneIndex + 1) % scenes.Count;
        var sceneName = scenes[sceneIndex].name;
        var scene = SceneManager.GetSceneByName(sceneName);
        SceneManager.LoadScene(sceneName);
        SceneManager.SetActiveScene(scene);
    }
}
