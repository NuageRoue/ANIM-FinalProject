using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class UIWheel<T> : MonoBehaviour
    where T : Enum
{
    [SerializeField]
    float durationOfSpin = 2f;

    [SerializeField]
    int numberOfSpin = 2;

    [SerializeField]
    AnimationCurve curveOfRotationOfSpin = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField]
    Button startRotation;

    [SerializeField]
    UIWheelMesh wheelMesh;

    [SerializeField]
    CanvasGroup canva;

    bool isHidden = false;

    TaggedWheelSegments<T> segments = null;

    public void Hide()
    {
        if (isHidden)
            return;
        isHidden = true;

        canva.alpha = 0;
        canva.interactable = false;
        canva.blocksRaycasts = false;
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

    public void Create(TaggedWheelSegments<T> segments)
    {
        if (this.segments == segments || segments.items.Count == 0)
            return;

        this.segments = segments.Copy();
        this.segments.Normalize();

        wheelMesh.Create(this.segments.AsListOfSegments());

        wheelMesh.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public TagSegment<T> Launch(UnityAction onFinish)
    {
        TagSegment<T> result = segments.items.First();

        float rand = UnityEngine.Random.value;

        float endAngle = 0;

        float sum = 0;
        foreach (var item in segments.items)
        {
            float nextSum = sum + item.segment.coef;

            if (rand <= nextSum)
            {
                endAngle = Mathf.Lerp(sum, nextSum, rand) * 360;
                result = item;
                break;
            }

            sum = nextSum;
        }

        Reveal();

        startRotation.enabled = true;

        startRotation.onClick.RemoveAllListeners();
        startRotation.onClick.AddListener(() =>
        {
            StartCoroutine(InternalDraw(onFinish, endAngle));
        });

        return result;
    }

    private IEnumerator InternalDraw(UnityAction onFinish, float endAngle)
    {
        bool byPassed = false;

        startRotation.onClick.RemoveAllListeners();
        startRotation.onClick.AddListener(() => byPassed = true);

        float elapsed = 0;

        wheelMesh.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
        endAngle += 360f * numberOfSpin;

        while (elapsed < durationOfSpin && !byPassed)
        {
            elapsed += Time.deltaTime;
            float current = curveOfRotationOfSpin.Evaluate(elapsed / durationOfSpin);
            float angle = Mathf.Lerp(0, endAngle, current);
            wheelMesh.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
            yield return null;
        }

        startRotation.onClick.RemoveAllListeners();
        startRotation.enabled = false;

        wheelMesh.rectTransform.rotation = Quaternion.Euler(0, 0, endAngle);

        onFinish.Invoke();
    }
}
