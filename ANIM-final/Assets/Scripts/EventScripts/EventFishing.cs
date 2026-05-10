using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum EventFishingTag
{
    FISH_2,
    FISH_1,
    NO_FISH,
}

public class EventFishing : EventBase
{
    [SerializeField]
    EventFishingUIWheel wheel;

    [SerializeField]
    NewUIDialog dialog;

    [SerializeField]
    TaggedWheelSegments<EventFishingTag> fishingRodSegments = new();

    [SerializeField]
    TaggedWheelSegments<EventFishingTag> abilitySegments = new();

    TagSegment<EventFishingTag> result = null;

    [SerializeField]
    Sprite fishingAbilitySprite;

    bool hasFishingRod = false;

    protected override void InternalStartEvent()
    {
        hasFishingRod = inventory.objectResources.Contains(RessourceObjectType.FISHING_ROD, 1);

        wheel.Hide();
        dialog.Hide();

        if (survivor.hasFishingAbility)
        {
            wheel.Create(abilitySegments);
        }
        else if (hasFishingRod)
        {
            wheel.Create(fishingRodSegments);
        }

        OnFirstMessage();
    }

    protected override void InernalEndEvent()
    {
        wheel.Hide();
        dialog.Hide();

        if (result != null)
        {
            switch (result.tag)
            {
                case EventFishingTag.FISH_2:
                    SetCompletion(true);
                    inventory.AddFood(2);
                    break;
                case EventFishingTag.FISH_1:
                    SetCompletion(true);
                    inventory.AddFood(1);
                    break;
                case EventFishingTag.NO_FISH:
                    break;
            }
        }
    }

    private void OnFirstMessage()
    {
        UnityAction next = OnHasNoFishingRod;

        if (survivor.hasFishingAbility)
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
        return result != null
            && (new EventFishingTag[] { EventFishingTag.FISH_1, EventFishingTag.FISH_2 }).Contains(
                result.tag
            );
    }
}
