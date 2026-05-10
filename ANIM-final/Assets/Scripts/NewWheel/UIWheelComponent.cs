using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWheelComponent : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;

    [SerializeField]
    Image image;

    public void Set(SegmentAttribute attribute)
    {
        text.SetText(attribute.name);

        if (attribute.sprite != null)
            image.sprite = attribute.sprite;
    }
}
