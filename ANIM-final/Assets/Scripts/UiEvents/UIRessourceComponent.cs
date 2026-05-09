using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRessourceComponent : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI UICount;

    [SerializeField]
    TextMeshProUGUI UIName;

    [SerializeField]
    Image UISprite;

    public void Set(int count, RessourceBase ressource)
    {
        UICount.SetText(count.ToString());
        UIName.SetText(ressource.name);

        if (ressource.sprite != null)
        {
            UISprite.sprite = ressource.sprite;
        }
    }
}
