using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Manages a dialog UI panel that animates text in character by character,
/// optionally displays a sprite, and waits for a button press to invoke a callback.
/// </summary>
public class NewUIDialog : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    Image image;

    [SerializeField]
    Button button;

    [SerializeField]
    TextMeshProUGUI text;

    [SerializeField]
    RectTransform animatedPopDestination;

    [Header("Canvas Groups")]
    [SerializeField]
    CanvasGroup imageCanva;

    [SerializeField]
    CanvasGroup canva;

    [Header("Animation Settings")]
    [SerializeField]
    float timeToShowText = 1.0f;

    [SerializeField]
    float timeToAnimatedPop = 0.2f;

    [SerializeField]
    AnimationCurve animatedPopCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    RectTransform rectTransform;
    Vector3 originalPosition;
    Vector3 targetPosition;
    bool isHidden = false;

    /// <summary>
    /// Caches the RectTransform and stores the original and target positions for the pop animation.
    /// </summary>
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.position;
        targetPosition = animatedPopDestination.position;
    }

    /// <summary>
    /// Hides the dialog and disables all interaction. Does nothing if already hidden.
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
    /// Hides the sprite image panel and disables its interaction.
    /// </summary>
    private void HideSnippet()
    {
        imageCanva.alpha = 0;
        imageCanva.interactable = false;
        imageCanva.blocksRaycasts = false;
    }

    /// <summary>
    /// Reveals the dialog and re-enables interaction. Does nothing if already visible.
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
    /// Reveals the sprite image panel and enables its interaction.
    /// </summary>
    private void RevealSnippet()
    {
        imageCanva.alpha = 1;
        imageCanva.interactable = true;
        imageCanva.blocksRaycasts = true;
    }

    /// <summary>
    /// Reveals the dialog, animates the text and pop effect, then waits for a button press
    /// before invoking the callback. An optional sprite is shown alongside the text.
    /// </summary>
    public void Launch(UnityAction onFinish, string text, Sprite sprite = null)
    {
        StartCoroutine(InternalDraw(onFinish, text, sprite));
    }

    /// <summary>
    /// Runs the dialog animation: reveals the panel, shows or hides the sprite,
    /// animates the text typewriter and pop effect, then switches the button to call onFinish.
    /// Clicking the button during animation skips to the full text immediately.
    /// </summary>
    private IEnumerator InternalDraw(UnityAction onFinish, string text, Sprite sprite)
    {
        Assert.IsTrue(timeToAnimatedPop <= timeToShowText); // Pop must finish before text does

        Reveal();

        // Show or hide the sprite panel based on whether a sprite was provided
        if (sprite != null)
        {
            RevealSnippet();
            image.sprite = sprite;
        }
        else
        {
            HideSnippet();
        }

        bool byPassed = false;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            byPassed = true; // Skip animation on button press
        });

        float currentTime = 0.0f;
        while (currentTime < timeToShowText && !byPassed)
        {
            AnimatePop(currentTime);
            AnimateText(currentTime, ref text);

            currentTime += Time.deltaTime;
            yield return null;
        }

        this.text.SetText(text); // Ensure full text is shown after animation or skip

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            onFinish.Invoke(); // Button now advances to the next step
        });
    }

    /// <summary>
    /// Progressively reveals the text character by character based on elapsed time.
    /// Appends "..." to indicate the text is still being revealed.
    /// </summary>
    private void AnimateText(float time, ref string text)
    {
        if (time >= timeToShowText)
            return;

        int index = Mathf.FloorToInt(text.Length * time / timeToShowText); // Characters revealed so far
        string current = text[..index] + "...";
        this.text.SetText(current);
    }

    /// <summary>
    /// Animates the dialog's position with a bounce pop effect during the opening phase.
    /// Uses a mirrored curve so the panel bounces out and back to its original position.
    /// </summary>
    private void AnimatePop(float time)
    {
        if (time >= timeToAnimatedPop)
            return;

        float t = time / timeToAnimatedPop;

        // Mirror t so the animation goes out then returns: 0 -> 1 -> 0
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
