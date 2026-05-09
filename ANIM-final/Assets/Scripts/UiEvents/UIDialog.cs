using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIDialog : MonoBehaviour
{
    [SerializeField]
    Button button;

    [SerializeField]
    TextMeshProUGUI textMeshPro;

    [SerializeField]
    float timeToShowText = 0.5f;

    float currentElapsedTime = 0.0f;

    string currentText = null;

    UnityAction callOnNext = null;

    [SerializeField]
    CanvasGroup canvasGroup;

    [SerializeField]
    AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    RectTransform rt;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        button.onClick.AddListener(OnNext);
    }

    public void NextText(UnityAction onNext, string text)
    {
        callOnNext = onNext;
        textMeshPro.SetText("");

        currentText = text;
        currentElapsedTime = 0;

        StartCoroutine(PopAnimation());
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void Reveal()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    void Update()
    {
        if (currentText == null)
            return;

        if (currentElapsedTime >= timeToShowText)
        {
            textMeshPro.SetText(currentText);
            currentText = null;
        }
        else
        {
            int size = Mathf.CeilToInt(currentElapsedTime / timeToShowText * currentText.Length);
            textMeshPro.SetText(currentText[..size] + "...");
        }

        currentElapsedTime += Time.deltaTime;
    }

    private void OnNext()
    {
        if (currentText != null)
        {
            currentElapsedTime = timeToShowText;
        }
        else
        {
            callOnNext?.Invoke();
        }
    }

    private IEnumerator PopAnimation()
    {
        const float timeEnd = 0.2f,
            halfTimeEnd = timeEnd * 0.5f;
        float time = 0;

        while (time < timeEnd)
        {
            float r = 0;

            if (time <= halfTimeEnd)
            {
                r = curve.Evaluate(time / halfTimeEnd);
            }
            else
            {
                r = curve.Evaluate(1.0f - (time - halfTimeEnd) / halfTimeEnd);
            }

            const float ratio = 0.05f;
            r = 1.0f + ratio * r;

            rt.localScale = new Vector3(r, r, 1f);

            time += Time.deltaTime;
            yield return null;
        }

        rt.localScale = new Vector3(1f, 1f, 1f);
    }
}
