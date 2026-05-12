using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIObjectComponent : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI UIName;

    [SerializeField]
    Image UISprite;

    [SerializeField]
    Toggle toggle;

    public CraftingRecipe receipe { get; private set; }

    public void Set(CraftingRecipe receipe, ToggleGroup group, UnityAction onToggled)
    {
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(
            (isOn) =>
            {
                if (isOn)
                {
                    onToggled.Invoke();
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
