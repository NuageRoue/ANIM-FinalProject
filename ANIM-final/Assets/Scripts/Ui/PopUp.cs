using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DigitalRuby.Tween;

public class PopUp : MonoBehaviour
{
    public static PopUp Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image image;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        gameObject.SetActive(false);
        text.text = "";
    }

    public void Display(string message)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);

        gameObject.SetActive(true);
        if (text.text.Length > 0)
            text.text +=  " - " + message;
        else
            text.text = message;

        gameObject.Tween("PopUpWait", 0f, 1f, 2f, TweenScaleFunctions.Linear, null)
            .ContinueWith(new FloatTween().Setup(1f, 0f, 1f, TweenScaleFunctions.Linear,
                (t) =>
                {
                    Color imgColor = image.color;
                    imgColor.a = t.CurrentValue;
                    image.color = imgColor;

                    Color txtColor = text.color;
                    txtColor.a = t.CurrentValue;
                    text.color = txtColor;
                },
                (t) =>
                {
                    Color imgColor = image.color;
                    imgColor.a = 1f;
                    image.color = imgColor;

                    Color txtColor = text.color;
                    txtColor.a = 1f;
                    text.color = txtColor;
                    text.text = "";

                    gameObject.SetActive(false);
                }
            ));
    }
}