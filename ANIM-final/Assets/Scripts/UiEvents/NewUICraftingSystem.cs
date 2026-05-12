using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NewUICraftingSystem : MonoBehaviour
{
    [SerializeField]
    List<CraftingRecipe> recipes;

    [SerializeField]
    public Inventory inventory;

    [SerializeField]
    GameObject ressourcesContainer;

    [SerializeField]
    GameObject objectsContainer;

    [SerializeField]
    GameObject neededRessourceContainer;

    [SerializeField]
    ToggleGroup toggleGroup;

    [SerializeField]
    GameObject ressourcePrefab;

    [SerializeField]
    GameObject objectPrefab;

    [SerializeField]
    Button craftButton;

    public CraftingRecipe craft { get; private set; } = null;

    UnityAction onFinish;

    [SerializeField]
    CanvasGroup canva;

    bool isHidden = false;

    public void Hide()
    {
        if (isHidden)
            return;
        isHidden = true;

        canva.alpha = 0;
        canva.interactable = false;
        canva.blocksRaycasts = false;
    }

    public void Reveal()
    {
        if (!isHidden)
            return;
        isHidden = false;

        canva.alpha = 1;
        canva.interactable = true;
        canva.blocksRaycasts = true;
    }

    public void Launch(UnityAction onFinish)
    {
        craft = null;

        Reveal();

        UpdateCrafts();
        UpdateRessources();
        UpdateNeeds();

        this.onFinish = onFinish;
    }

    public void Finish()
    {
        craft = null;
        onFinish?.Invoke();
    }

    public void Craft()
    {
        if (craft != null)
        {
            inventory.Craft(craft);
        }

        // Debug
        UpdateCrafts();
        UpdateRessources();
        UpdateNeeds();

        onFinish?.Invoke();
    }

    private void UpdateRessources()
    {
        ressourcesContainer
            .GetComponentsInChildren<UIRessourceComponent>()
            .ToList()
            .ForEach((e) => Destroy(e.gameObject));

        inventory.baseResources.items.ForEach(
            (item) =>
                Instantiate(ressourcePrefab, ressourcesContainer.transform, false)
                    .GetComponent<UIRessourceComponent>()
                    .Set(item.count, item.type)
        );
    }

    private void UpdateCrafts()
    {
        objectsContainer
            .GetComponentsInChildren<UIObjectComponent>()
            .ToList()
            .ForEach((e) => Destroy(e.gameObject));

        craftButton.interactable = false;

        recipes
            .FindAll((r) => inventory.CanCraft(r))
            .ForEach(
                (r) =>
                    Instantiate(objectPrefab, objectsContainer.transform, false)
                        .GetComponent<UIObjectComponent>()
                        .Set(r, toggleGroup, OnCraftChange)
            );
    }

    private void UpdateNeeds()
    {
        neededRessourceContainer
            .GetComponentsInChildren<UIRessourceComponent>()
            .ToList()
            .ForEach((e) => Destroy(e.gameObject));

        if (craft == null)
            return;

        craft.inputResources.items.ForEach(
            (item) =>
                Instantiate(ressourcePrefab, neededRessourceContainer.transform, false)
                    .GetComponent<UIRessourceComponent>()
                    .Set(item.count, item.type)
        );
    }

    private void OnCraftChange()
    {
        var current = toggleGroup.ActiveToggles().FirstOrDefault();

        if (current == null)
        {
            craftButton.interactable = false;
            return;
        }

        craftButton.interactable = true;

        CraftingRecipe recipe = current.GetComponent<UIObjectComponent>().receipe;

        if (recipe == craft)
            return;

        craft = recipe;
        current.Select();
        UpdateNeeds();
    }

    public bool CanBuild()
    {
        return recipes.FindAll((r) => inventory.CanCraft(r)).Count != 0;
    }
}
