using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Represents a single craftable object entry in the crafting UI.
/// Displays the recipe's output name and sprite, and notifies the parent when selected.
/// </summary>
public class UIObjectComponent : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    TextMeshProUGUI UIName;

    [SerializeField]
    Image UISprite;

    [SerializeField]
    Toggle toggle;

    public CraftingRecipe receipe { get; private set; }

    /// <summary>
    /// Initializes the component with a recipe, assigns it to a toggle group,
    /// and registers the callback to invoke when this entry is selected.
    /// </summary>
    public void Set(CraftingRecipe receipe, ToggleGroup group, UnityAction onToggled)
    {
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(
            (isOn) =>
            {
                if (isOn)
                {
                    onToggled.Invoke(); // Notify parent only when this toggle is turned on
                }
            }
        );

        toggle.group = group;
        this.receipe = receipe;

        RessourceObject ressource = ResouceLoader.instance.FindByType(receipe.outputObject);

        UIName.SetText(ressource.name);

        if (ressource != null)
        {
            UISprite.sprite = ressource.sprite;
        }
    }
}
