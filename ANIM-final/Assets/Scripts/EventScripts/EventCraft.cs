using UnityEngine;
using UnityEngine.Events;

public class EventCraft : EventBase
{
    [SerializeField]
    NewUICraftingSystem uics;

    [SerializeField]
    NewUIDialog dialog;

    protected override void Awake()
    {
        base.Awake();

        uics.Hide();
        dialog.Hide();
    }

    protected override void InternalStartEvent()
    {
        uics.inventory = inventory;
        uics.Hide();

        if (uics.CanBuild())
        {
            dialog.Launch(OnStartCrafting, "You want to create something?");
        }
        else
        {
            OnEating();
        }
    }

    protected override void InernalEndEvent()
    {
        uics.Hide();
        dialog.Hide();
    }

    private void OnStartCrafting()
    {
        dialog.Hide();
        uics.Launch(OnCraftingEnd);
    }

    private void OnCraftingEnd()
    {
        uics.Hide();

        if (uics.craft != null)
        {
            RessourceObject ressource = ResouceLoader.instance.FindByType(uics.craft.outputObject);

            UnityAction next = OnStartCrafting;

            if (!uics.CanBuild())
            {
                next = OnEating;
            }

            dialog.Launch(next, "You craft a " + ressource.name + ".", ressource.sprite);
        }
        else
        {
            OnEating();
        }
    }

    private void OnEating()
    {
        int needed = GetFoodNeeded();

        if (!inventory.baseResources.Contains(ResourceType.Food, needed))
        {
            SetCompletion(true);
            dialog.Launch(EventManager.Instance.LoadDefeatScene, "Oh no, it seems like you don't have enough food");
        }
        else
        {
            inventory.baseResources.Remove(ResourceType.Food, needed);
            dialog.Launch(
                EndEvent,
                "You eat " + needed + " food",
                ResouceLoader.instance.FindByType(ResourceType.Food).sprite
            );
        }
    }
}
