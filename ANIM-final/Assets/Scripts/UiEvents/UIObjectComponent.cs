using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIObjectComponent : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI UIName;

    [SerializeField]
    Image UISprite;

    [SerializeField]
    Toggle toggle;

    [SerializeField]
    UICraftingSystem uics;

    CraftingRecipe receipe;

    public void Set(CraftingRecipe receipe, RessourceObject ressourceObject)
    {
        UIName.SetText(ressourceObject.name);

        this.receipe = receipe;

        if (ressourceObject != null)
        {
            UISprite.sprite = ressourceObject.sprite;
        }

        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(
            (isOn) =>
            {
                if (isOn)
                {
                    uics.OnToggleChanged(receipe);
                }
            }
        );
    }

    public CraftingRecipe Finish()
    {
        toggle.SetIsOnWithoutNotify(false);
        toggle.group = null;
        return receipe;
    }
}
