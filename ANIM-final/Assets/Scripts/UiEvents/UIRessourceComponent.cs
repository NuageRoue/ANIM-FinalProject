using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays a single resource entry in the UI, showing its count, name, and sprite.
/// </summary>
public class UIRessourceComponent : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    TextMeshProUGUI UICount;

    [SerializeField]
    TextMeshProUGUI UIName;

    [SerializeField]
    Image UISprite;

    /// <summary>
    /// Populates the UI fields with the count and visual data for the given resource type.
    /// </summary>
    public void Set(int count, ResourceType type)
    {
        RessourceBase ressource = ResouceLoader.instance.FindByType(type);

        UICount.SetText(count.ToString());
        UIName.SetText(ressource.name);

        if (ressource.sprite != null)
        {
            UISprite.sprite = ressource.sprite;
        }
    }
}
