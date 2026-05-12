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

/// <summary>
/// Handles the fishing event sequence: checking for a fishing rod or ability,
/// spinning the wheel, and awarding food if the survivor catches a fish.
/// </summary>
public class EventFishing : EventBase
{
    [Header("References")]
    [SerializeField]
    Fish fish;

    [SerializeField]
    CharacterAniamation character;

    [SerializeField]
    Transform fishEnd;

    [Header("UI")]
    [SerializeField]
    EventFishingUIWheel wheel;

    [SerializeField]
    NewUIDialog dialog;

    [Header("Wheel Segments")]
    [SerializeField]
    TaggedWheelSegments<EventFishingTag> fishingRodSegments = new();

    [SerializeField]
    TaggedWheelSegments<EventFishingTag> abilitySegments = new();

    [Header("Ability Sprites")]
    [SerializeField]
    Sprite fishingAbilitySprite;

    TagSegment<EventFishingTag> result = null;
    bool hasFishingRod = false;

    /// <summary>
    /// Hides the wheel and dialog on initialization.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        wheel.Hide();
        dialog.Hide();
    }

    /// <summary>
    /// Sets up the character, starts the fish loop, checks for fishing rod and ability,
    /// builds the appropriate wheel, and begins the dialogue sequence.
    /// </summary>
    protected override void InternalStartEvent()
    {
        character.Set(GetSurvivorIndex());
        character.Look(fish.transform);

        fish.LoopStart();

        hasFishingRod = inventory.objectResources.Contains(RessourceObjectType.FISHING_ROD, 1);

        wheel.Hide();
        dialog.Hide();

        // Pick segments based on ability or item availability
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

    /// <summary>
    /// Hides the UI and applies the wheel result, adding food if the survivor caught a fish.
    /// </summary>
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

    /// <summary>
    /// Shows the opening dialogue and routes to the appropriate next step
    /// based on ability or item availability.
    /// </summary>
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

    /// <summary>
    /// Shows a dialogue informing the survivor they cannot fish without a rod, then ends the event.
    /// </summary>
    private void OnHasNoFishingRod()
    {
        dialog.Launch(EndEvent, "You don't have a fishing rod, you can't get it.");
    }

    /// <summary>
    /// Shows a dialogue confirming the fishing rod is available, then spins the wheel.
    /// </summary>
    private void OnHasFishingRod()
    {
        dialog.Launch(
            OnSpinningWheel,
            "You have a fishing rod, you can catch it.",
            ResouceLoader.instance.FindByType(RessourceObjectType.FISHING_ROD).sprite
        );
    }

    /// <summary>
    /// Shows a dialogue confirming the fishing ability is active, then spins the wheel.
    /// </summary>
    private void OnHasAbility()
    {
        dialog.Launch(
            OnSpinningWheel,
            "Why use a fishing rog? You can catch the fish without it!",
            fishingAbilitySprite
        );
    }

    /// <summary>
    /// Hides the dialog and launches the wheel spin.
    /// </summary>
    private void OnSpinningWheel()
    {
        dialog.Hide();
        result = wheel.Launch(() =>
        {
            StartCoroutine(OnWheelFinish());
        });
    }

    /// <summary>
    /// Waits after the wheel stops, snaps the fish to the end position if caught,
    /// then shows the outcome dialogue.
    /// </summary>
    private IEnumerator OnWheelFinish()
    {
        bool gotFish = GotFish();

        if (gotFish)
        {
            fish.LoopStop();
            fish.SetPosition(fishEnd); // Snap fish to the caught position
        }

        yield return new WaitForSeconds(2.0f);
        wheel.Hide();

        if (gotFish)
        {
            dialog.Launch(EndEvent, "You got " + result.segment.name + "!", result.segment.sprite);
        }
        else
        {
            dialog.Launch(EndEvent, "You didn't get a fish.");
        }
    }

    /// <summary>
    /// Returns true if the wheel result is any fish outcome.
    /// </summary>
    private bool GotFish()
    {
        return result != null
            && (new EventFishingTag[] { EventFishingTag.FISH_1, EventFishingTag.FISH_2 }).Contains(
                result.tag
            );
    }
}
