using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

public class NewUIDialog : MonoBehaviour
{
    [SerializeField]
    Image image;

    [SerializeField]
    CanvasGroup imageCanva;

    [SerializeField]
    Button button;

    [SerializeField]
    TextMeshProUGUI text;

    [SerializeField]
    CanvasGroup canva;

    [SerializeField]
    float timeToShowText = 1.0f;

    [SerializeField]
    float timeToAnimatedPop = 0.2f;

    [SerializeField]
    AnimationCurve animatedPopCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField]
    RectTransform animatedPopDestination;

    RectTransform rectTransform;

    Vector3 originalPosition;
    Vector3 targetPosition;

    bool isHidden = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.position;
        targetPosition = animatedPopDestination.position;
    }

    public void Hide()
    {
        if (isHidden)
            return;
        isHidden = true;

        canva.alpha = 0;
        canva.interactable = false;
        canva.blocksRaycasts = false;
    }

    private void HideSnippet()
    {
        imageCanva.alpha = 0;
        imageCanva.interactable = false;
        imageCanva.blocksRaycasts = false;
    }

    public void Reveal()
    {
        if (!isHidden)
            return;
        isHidden = false;

        canva.alpha = 1;
        canva.interactable = true;
        canva.blocksRaycasts = true;
    }

    private void RevealSnippet()
    {
        imageCanva.alpha = 1;
        imageCanva.interactable = true;
        imageCanva.blocksRaycasts = true;
    }

    public void Launch(UnityAction onFinish, string text, Sprite sprite = null)
    {
        StartCoroutine(InternalDraw(onFinish, text, sprite));
    }

    private IEnumerator InternalDraw(UnityAction onFinish, string text, Sprite sprite)
    {
        Assert.IsTrue(timeToAnimatedPop <= timeToShowText);

        Reveal();

        if (sprite != null)
        {
            RevealSnippet();
        }
        else
        {
            HideSnippet();
        }

        if (sprite != null)
        {
            image.sprite = sprite;
        }

        bool byPassed = false;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            byPassed = true;
        });

        float currentTime = 0.0f;
        while (currentTime < timeToShowText && !byPassed)
        {
            AnimatePop(currentTime);
            AnimateText(currentTime, ref text);

            currentTime += Time.deltaTime;
            yield return null;
        }

        this.text.SetText(text);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            onFinish.Invoke();
        });
    }

    private void AnimateText(float time, ref string text)
    {
        if (time >= timeToShowText)
            return;

        int index = Mathf.FloorToInt(text.Length * time / timeToShowText);
        string current = text[..index] + "...";
        this.text.SetText(current);
    }

    private void AnimatePop(float time)
    {
        if (time >= timeToAnimatedPop)
            return;

        float t = time / timeToAnimatedPop;

        if (t < 0.5)
        {
            t *= 2;
        }
        else
        {
            t = 1.0f - (t - 0.5f) * 2;
        }

        float u = animatedPopCurve.Evaluate(t);

        rectTransform.position = Vector3.Lerp(originalPosition, targetPosition, u);
    }
}
