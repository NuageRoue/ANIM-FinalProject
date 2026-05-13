using DigitalRuby.Tween;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // if (GameManager.Instance != null)
        //     Destroy(GameManager.Instance.gameObject);
    }

    public void OpenPanel(GameObject panel)
    {
        TweenFactory.RemoveTweenKey("Close" + panel.name, TweenStopBehavior.Complete);
        panel.SetActive(true);
        panel.transform.localScale = Vector3.zero;

        gameObject.Tween("Open" + panel.name, 0f, 1f, 0.5f, TweenScaleFunctions.CubicEaseOut,
            (t) => panel.transform.localScale = Vector3.one * t.CurrentValue,
            null
        );
    }

    public void ClosePanel(GameObject panel)
    {
        if (!panel.activeSelf)
            return;
        TweenFactory.RemoveTweenKey("Open" + panel.name, TweenStopBehavior.Complete);

        gameObject.Tween("Close" + panel.name, 1f, 0f, 0.5f, TweenScaleFunctions.CubicEaseIn,
            (t) => panel.transform.localScale = Vector3.one * t.CurrentValue,
            (t) => panel.SetActive(false)
        );
    }

    public void LoadLevel(int index)
    {
        LevelLoader.Instance.LoadLevel(index);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
