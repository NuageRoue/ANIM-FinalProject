using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICraftingSystem : MonoBehaviour
{
    [SerializeField]
    List<CraftingRecipe> recipes;

    Inventory inventory;

    [SerializeField]
    GameObject ressourcesContainer;
    UIRessourceComponent[] ressourcesContainerList;

    [SerializeField]
    GameObject objectsContainer;
    UIObjectComponent[] objectsContainerList;

    [SerializeField]
    GameObject craftingNeedsContainer;
    UIRessourceComponent[] craftingNeedsContainerList;

    [SerializeField]
    CanvasGroup canvasGroup;

    [SerializeField]
    ToggleGroup toggleGroup;

    UIObjectComponent activeToggle;

    [SerializeField]
    Button craftingButton;

    [SerializeField]
    Button finishButton;

    public void Awake()
    {
        ressourcesContainerList = ressourcesContainer
            .GetComponentsInChildren<UIRessourceComponent>()
            .ToArray();

        objectsContainerList = objectsContainer
            .GetComponentsInChildren<UIObjectComponent>()
            .ToArray();

        craftingNeedsContainerList = craftingNeedsContainer
            .GetComponentsInChildren<UIRessourceComponent>()
            .ToArray();

        craftingButton.onClick.AddListener(Crafting);
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void Reveal()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;
    }

    public IEnumerator StartCrafting()
    {
        UpdateRessource();
        UpdateCraft();
        UpdateNeededResources();

        activeToggle = null;

        while (true)
        {
            var toggle = toggleGroup.ActiveToggles().FirstOrDefault();

            if (toggle != null)
            {
                activeToggle = toggle.gameObject.GetComponent<UIObjectComponent>();
                craftingButton.enabled = true;
            }
            else
            {
                craftingButton.enabled = false;
            }

            yield return null;
        }
    }

    public void OnFinish(UnityAction call)
    {
        finishButton.onClick.RemoveAllListeners();
        finishButton.onClick.AddListener(call);
    }

    private void UpdateRessource()
    {
        int max = ressourcesContainerList.Length;
        int count = inventory.baseResources.items.Count;

        for (int i = 0; i < max; ++i)
        {
            ressourcesContainerList[i].gameObject.SetActive(i < count);
        }

        for (int i = 0; i < count; ++i)
        {
            var map = inventory.baseResources.items[i];
            var refer = ResouceLoader.instance.FindByType(map.type);
            // ressourcesContainerList[i].Set(map.count, refer);
        }
    }

    private void UpdateCraft()
    {
        int max = objectsContainerList.Length;
        var craftable = recipes.FindAll((r) => inventory.CanCraft(r));
        int count = craftable.Count;

        for (int i = 0; i < max; ++i)
        {
            objectsContainerList[i].gameObject.SetActive(i < count);
        }

        for (int i = 0; i < count; ++i)
        {
            var recipe = recipes[i];
            var refer = ResouceLoader.instance.FindByType(recipe.outputObject);
            // objectsContainerList[i].Set(recipe, refer);
        }

        EventSystem.current.SetSelectedGameObject(null);
    }

    private void UpdateNeededResources()
    {
        var rcList = craftingNeedsContainer.GetComponentsInChildren<UIRessourceComponent>();

        foreach (var rc in rcList)
        {
            rc.gameObject.SetActive(false);
        }
    }

    public void OnToggleChanged(CraftingRecipe recipe)
    {
        int max = craftingNeedsContainerList.Length;
        int count = recipe.inputResources.items.Count;

        for (int i = 0; i < max; ++i)
        {
            craftingNeedsContainerList[i].gameObject.SetActive(i < count);
        }

        for (int i = 0; i < count; ++i)
        {
            var map = recipe.inputResources.items[i];
            var refer = ResouceLoader.instance.FindByType(map.type);

            // craftingNeedsContainerList[i].Set(map.count, refer);
        }
    }

    public void Crafting()
    {
        // var receipe = activeToggle.Finish();

        // inventory.Craft(receipe);

        UpdateRessource();
        UpdateCraft();
        UpdateNeededResources();
    }
}
