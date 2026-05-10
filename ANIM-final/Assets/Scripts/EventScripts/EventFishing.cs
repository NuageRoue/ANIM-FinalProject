using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum EventFishingTag
{
    FISH,
    NO_FISH,
}

public class EventFishing : EventBase
{
    [SerializeField]
    EventFishingUIWheel wheel;

    [SerializeField]
    NewUIDialog dialog;

    [SerializeField]
    Inventory inventory;

    [SerializeField]
    TaggedWheelSegments<EventFishingTag> fishingRodSegments = new();

    [SerializeField]
    TaggedWheelSegments<EventFishingTag> abilitySegments = new();

    TagSegment<EventFishingTag> result = null;

    [SerializeField]
    Sprite fishingAbilitySprite;

    bool hasFishingRod = false;

    [SerializeField]
    bool hasFishingAbility = false;

    void Start()
    {
        StartEvent();
    }

    public override void StartEvent()
    {
        hasFishingRod = inventory.objectResources.Contains(RessourceObjectType.FISHING_ROD, 1);

        wheel.Hide();
        dialog.Hide();

        if (hasFishingAbility)
        {
            wheel.Create(abilitySegments);
        }
        else if (hasFishingRod)
        {
            wheel.Create(fishingRodSegments);
        }

        OnFirstMessage();
    }

    public override void EndEvent()
    {
        wheel.Hide();
        dialog.Hide();
    }

    private void OnFirstMessage()
    {
        UnityAction next = OnHasNoFishingRod;

        if (hasFishingAbility)
        {
            next = OnHasAbility;
        }
        else if (hasFishingRod)
        {
            next = OnHasFishingRod;
        }

        dialog.Launch(next, "You found a fish!");
    }

    private void OnHasNoFishingRod()
    {
        dialog.Launch(EndEvent, "You don't have a fishing rod, you can't get it.");
    }

    private void OnHasFishingRod()
    {
        dialog.Launch(
            OnSpinningWheel,
            "You have a fishing rod, you can catch it.",
            ResouceLoader.instance.FindByType(RessourceObjectType.FISHING_ROD).sprite
        );
    }

    private void OnHasAbility()
    {
        dialog.Launch(
            OnSpinningWheel,
            "Why use a fishing rog? You can catch the fish without it!",
            fishingAbilitySprite
        );
    }

    private void OnSpinningWheel()
    {
        dialog.Hide();
        result = wheel.Launch(() =>
        {
            StartCoroutine(OnWheelFinish());
        });
    }

    private IEnumerator OnWheelFinish()
    {
        yield return new WaitForSeconds(2.0f);
        wheel.Hide();

        if (GotFish())
        {
            dialog.Launch(
                EndEvent,
                "You got a " + result.segment.name + "!",
                result.segment.sprite
            );
        }
        else
        {
            dialog.Launch(EndEvent, "You didn't get a fish.");
        }
    }

    private bool GotFish()
    {
        return result != null && result.tag == EventFishingTag.FISH;
    }
}
