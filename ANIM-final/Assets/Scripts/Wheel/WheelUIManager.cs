using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class WheelUIManager : MonoBehaviour
{
    [SerializeField]
    List<SegmentAttribute> segments = new();

    [SerializeField]
    float duration = 1f;

    [SerializeField]
    float speen = 2f;

    [SerializeField]
    AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField]
    WheelUIMeshBuilder wmb;

    [SerializeField]
    UnityEvent onSpinFinish = null;

    [SerializeField]
    CanvasGroup canvasGroup;

    public void Build(List<SegmentAttribute> segments)
    {
        if (segments != this.segments && segments.Count > 0)
        {
            this.segments.Clear();
            this.segments.AddRange(segments);
        }

        CleanSegments();
        wmb.Build(this.segments);
    }

    public SegmentAttribute Speen()
    {
        SegmentAttribute result = segments[0];

        float rand = Random.value;

        float endAngle = 0;

        float sum = 0;
        foreach (var segment in segments)
        {
            float nextSum = sum + segment.coef;

            if (rand <= nextSum)
            {
                endAngle = Mathf.Lerp(sum, nextSum, rand) * 360;
                result = segment;
                break;
            }

            sum = nextSum;
        }

        var speen = Speening(endAngle);
        StartCoroutine(speen);

        return result;
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

    public void OnSpinFinish(UnityAction call)
    {
        onSpinFinish.RemoveAllListeners();
        onSpinFinish.AddListener(call);
    }

    private IEnumerator Speening(float endAngle)
    {
        float elapsed = 0;

        wmb.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
        endAngle += speen * 360f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float current = curve.Evaluate(elapsed / duration);
            float angle = Mathf.Lerp(0, endAngle, current);
            wmb.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
            yield return null;
        }

        wmb.rectTransform.rotation = Quaternion.Euler(0, 0, endAngle);

        onSpinFinish?.Invoke();
    }

    private void CleanSegments()
    {
        float total = segments.Sum(seg => seg.coef);
        foreach (var segment in this.segments)
        {
            segment.coef /= total; // Normalize
            segment.color.a = 1.0f; // Set alpha
        }
    }
}
