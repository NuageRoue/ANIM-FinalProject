using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles the crafting event sequence: setting up characters around the fire,
/// optionally launching the crafting UI, then consuming food to end the event.
/// </summary>
public class EventCraft : EventBase
{
    [Header("References")]
    [SerializeField]
    List<CharacterAniamation> aniamations;

    [SerializeField]
    Transform fire;

    [Header("UI")]
    [SerializeField]
    NewUICraftingSystem uics;

    [SerializeField]
    NewUIDialog dialog;

    /// <summary>
    /// Hides the crafting UI and dialog on initialization.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        uics.Hide();
        dialog.Hide();
    }

    /// <summary>
    /// Sets up each character around the fire, then either opens the crafting prompt
    /// or skips directly to eating if no recipes are available.
    /// </summary>
    protected override void InternalStartEvent()
    {
        uics.inventory = inventory;
        uics.Hide();

        for (int i = 0; i < aniamations.Count; ++i)
        {
            aniamations[i].Set(i);
            aniamations[i].Look(fire); // Face each character toward the fire
        }

        if (uics.CanBuild())
        {
            dialog.Launch(OnStartCrafting, "You want to create something?");
        }
        else
        {
            OnEating(); // No recipes available, skip straight to food consumption
        }
    }

    /// <summary>
    /// Hides the crafting UI and dialog at the end of the event.
    /// </summary>
    protected override void InernalEndEvent()
    {
        uics.Hide();
        dialog.Hide();
    }

    /// <summary>
    /// Hides the dialog and opens the crafting UI.
    /// </summary>
    private void OnStartCrafting()
    {
        dialog.Hide();
        uics.Launch(OnCraftingEnd);
    }

    /// <summary>
    /// Called when the crafting UI closes. Shows a confirmation dialogue if something
    /// was crafted, then loops back to crafting or proceeds to eating.
    /// </summary>
    private void OnCraftingEnd()
    {
        uics.Hide();

        if (uics.craft != null)
        {
            RessourceObject ressource = ResouceLoader.instance.FindByType(uics.craft.outputObject);

            // Return to crafting if more recipes are available, otherwise move to eating
            UnityAction next = uics.CanBuild() ? OnStartCrafting : OnEating;

            dialog.Launch(next, "You craft a " + ressource.name + ".", ressource.sprite);
        }
        else
        {
            OnEating(); // Player cancelled crafting
        }
    }

    /// <summary>
    /// Checks if the inventory has enough food for the group.
    /// Triggers defeat if not, otherwise consumes the food and ends the event.
    /// </summary>
    private void OnEating()
    {
        int needed = GetFoodNeeded();

        if (!inventory.baseResources.Contains(ResourceType.Food, needed))
        {
            SetCompletion(true);
            dialog.Launch(
                EventManager.Instance.LoadDefeatScene,
                "Oh no, it seems like you don't have enough food"
            );
        }
        else
        {
            inventory.baseResources.Remove(ResourceType.Food, needed); // Consume required food
            dialog.Launch(
                EndEvent,
                "You eat " + needed + " food",
                ResouceLoader.instance.FindByType(ResourceType.Food).sprite
            );
        }
    }
}
