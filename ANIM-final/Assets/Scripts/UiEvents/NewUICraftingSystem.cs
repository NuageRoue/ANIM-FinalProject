using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Manages the crafting UI: displays available recipes and current resources,
/// handles recipe selection, and triggers crafting on confirmation.
/// </summary>
public class NewUICraftingSystem : MonoBehaviour
{
    [Header("Recipes")]
    [SerializeField]
    List<CraftingRecipe> recipes;

    [Header("Inventory")]
    [SerializeField]
    public Inventory inventory;

    [Header("Containers")]
    [SerializeField]
    GameObject ressourcesContainer;

    [SerializeField]
    GameObject objectsContainer;

    [SerializeField]
    GameObject neededRessourceContainer;

    [Header("References")]
    [SerializeField]
    ToggleGroup toggleGroup;

    [SerializeField]
    GameObject ressourcePrefab;

    [SerializeField]
    GameObject objectPrefab;

    [SerializeField]
    Button craftButton;

    [SerializeField]
    CanvasGroup canva;

    public CraftingRecipe craft { get; private set; } = null;

    UnityAction onFinish;
    bool isHidden = false;

    /// <summary>
    /// Hides the crafting UI and disables all interaction. Does nothing if already hidden.
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
    /// Reveals the crafting UI and re-enables interaction. Does nothing if already visible.
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
    /// Reveals the UI, resets the selected recipe, and refreshes all panels.
    /// Stores the callback to invoke when the player finishes or cancels.
    /// </summary>
    public void Launch(UnityAction onFinish)
    {
        craft = null;

        Reveal();

        UpdateCrafts();
        UpdateRessources();
        UpdateNeeds();

        this.onFinish = onFinish;
    }

    /// <summary>
    /// Cancels crafting without performing any action and invokes the finish callback.
    /// </summary>
    public void Finish()
    {
        craft = null;
        onFinish?.Invoke();
    }

    /// <summary>
    /// Crafts the currently selected recipe if one is selected,
    /// refreshes all UI panels, then invokes the finish callback.
    /// </summary>
    public void Craft()
    {
        if (craft != null)
        {
            inventory.Craft(craft);
        }

        UpdateCrafts();
        UpdateRessources();
        UpdateNeeds();

        onFinish?.Invoke();
    }

    /// <summary>
    /// Destroys all existing resource entries and rebuilds them from the current inventory.
    /// </summary>
    private void UpdateRessources()
    {
        ressourcesContainer
            .GetComponentsInChildren<UIRessourceComponent>()
            .ToList()
            .ForEach((e) => Destroy(e.gameObject)); // Clear existing entries before rebuilding

        inventory.baseResources.items.ForEach(
            (item) =>
                Instantiate(ressourcePrefab, ressourcesContainer.transform, false)
                    .GetComponent<UIRessourceComponent>()
                    .Set(item.count, item.type)
        );
    }

    /// <summary>
    /// Destroys all existing recipe entries, disables the craft button,
    /// and rebuilds the list with only currently craftable recipes.
    /// </summary>
    private void UpdateCrafts()
    {
        objectsContainer
            .GetComponentsInChildren<UIObjectComponent>()
            .ToList()
            .ForEach((e) => Destroy(e.gameObject)); // Clear existing entries before rebuilding

        craftButton.interactable = false; // Disable until a recipe is selected

        recipes
            .FindAll((r) => inventory.CanCraft(r))
            .ForEach(
                (r) =>
                    Instantiate(objectPrefab, objectsContainer.transform, false)
                        .GetComponent<UIObjectComponent>()
                        .Set(r, toggleGroup, OnCraftChange)
            );
    }

    /// <summary>
    /// Destroys all existing need entries and rebuilds them from the selected recipe's inputs.
    /// Does nothing if no recipe is selected.
    /// </summary>
    private void UpdateNeeds()
    {
        neededRessourceContainer
            .GetComponentsInChildren<UIRessourceComponent>()
            .ToList()
            .ForEach((e) => Destroy(e.gameObject)); // Clear existing entries before rebuilding

        if (craft == null)
            return;

        craft.inputResources.items.ForEach(
            (item) =>
                Instantiate(ressourcePrefab, neededRessourceContainer.transform, false)
                    .GetComponent<UIRessourceComponent>()
                    .Set(item.count, item.type)
        );
    }

    /// <summary>
    /// Called when the recipe toggle selection changes. Updates the craft button state
    /// and refreshes the needed resources panel for the newly selected recipe.
    /// </summary>
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
        UpdateNeeds(); // Refresh required resources for the newly selected recipe
    }

    /// <summary>
    /// Returns true if at least one recipe in the list can currently be crafted.
    /// </summary>
    public bool CanBuild()
    {
        return recipes.FindAll((r) => inventory.CanCraft(r)).Count != 0;
    }
}
