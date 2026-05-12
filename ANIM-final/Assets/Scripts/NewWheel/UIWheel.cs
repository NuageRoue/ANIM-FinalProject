using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Generic UI wheel component that displays tagged segments, handles spin animation,
/// and returns the winning segment. The spin starts on button press and can be skipped.
/// </summary>
[Serializable]
public class UIWheel<T> : MonoBehaviour
    where T : Enum
{
    [Header("Spin Settings")]
    [SerializeField]
    float durationOfSpin = 2f;

    [SerializeField]
    int numberOfSpin = 2;

    [SerializeField]
    AnimationCurve curveOfRotationOfSpin = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("References")]
    [SerializeField]
    Button startRotation;

    [SerializeField]
    UIWheelMesh wheelMesh;

    [SerializeField]
    CanvasGroup canva;

    bool isHidden = false;
    TaggedWheelSegments<T> segments = null;

    /// <summary>
    /// Hides the wheel and disables all interaction. Does nothing if already hidden.
    /// </summary>
    public void Hide()
    {
        if (isHidden)
            return;
        isHidden = true;

        canva.alpha = 0;
        canva.interactable = false;
        canva.blocksRaycasts = false;
    }

    /// <summary>
    /// Reveals the wheel and re-enables interaction. Does nothing if already visible.
    /// </summary>
    public void Reveal()
    {
        if (!isHidden)
            return;
        isHidden = false;

        canva.alpha = 1;
        canva.interactable = true;
        canva.blocksRaycasts = true;
    }

    /// <summary>
    /// Copies and normalizes the given segments, then builds the wheel mesh.
    /// Does nothing if the same segments reference or an empty list is passed.
    /// </summary>
    public void Create(TaggedWheelSegments<T> segments)
    {
        if (this.segments == segments || segments.items.Count == 0)
            return;

        this.segments = segments.Copy();
        this.segments.Normalize();

        wheelMesh.Create(this.segments.AsListOfSegments());

        wheelMesh.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
    }

    /// <summary>
    /// Picks a random weighted segment as the result, reveals the wheel,
    /// and waits for the player to press the spin button before animating.
    /// Returns the winning segment immediately so the caller can prepare for it.
    /// </summary>
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
                endAngle = Mathf.Lerp(sum, nextSum, rand) * 360; // Target angle within the winning segment
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
            StartCoroutine(InternalDraw(onFinish, endAngle)); // Begin spin when button is pressed
        });

        return result;
    }

    /// <summary>
    /// Animates the wheel rotating to the target angle over the configured duration.
    /// A second button press during the spin skips the animation and snaps to the result.
    /// Invokes onFinish once the final angle is reached.
    /// </summary>
    private IEnumerator InternalDraw(UnityAction onFinish, float endAngle)
    {
        bool byPassed = false;

        startRotation.onClick.RemoveAllListeners();
        startRotation.onClick.AddListener(() => byPassed = true); // Allow skipping the animation

        float elapsed = 0;

        wheelMesh.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
        endAngle += 360f * numberOfSpin; // Add full rotations for visual effect

        while (elapsed < durationOfSpin && !byPassed)
        {
            elapsed += Time.deltaTime;
            float current = curveOfRotationOfSpin.Evaluate(elapsed / durationOfSpin); // Eased progress
            float angle = Mathf.Lerp(0, endAngle, current);
            wheelMesh.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
            yield return null;
        }

        startRotation.onClick.RemoveAllListeners();
        startRotation.enabled = false;

        wheelMesh.rectTransform.rotation = Quaternion.Euler(0, 0, endAngle); // Snap to exact final angle

        onFinish.Invoke();
    }
}
