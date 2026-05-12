using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventFoodTag
{
    FOOD_1,
    FOOD_2,
    FOOD_3,
}

/// <summary>
/// Handles the food tree event: walking the character to the tree,
/// shaking it, spinning the wheel, and awarding the resulting food amount.
/// </summary>
public class EventFood : EventBase
{
    [Header("References")]
    [SerializeField]
    CharacterAniamation character;

    [SerializeField]
    Transform destination;

    [SerializeField]
    FoodTree food;

    [Header("UI")]
    [SerializeField]
    NewUIDialog dialog;

    [SerializeField]
    EventFoodUIWheel wheel;

    [Header("Wheel Segments")]
    [SerializeField]
    TaggedWheelSegments<EventFoodTag> foodSegments = new();

    TagSegment<EventFoodTag> result;

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
    /// Builds the wheel, sets up the character, and shows the opening dialogue.
    /// </summary>
    protected override void InternalStartEvent()
    {
        wheel.Create(foodSegments);
        character.Set(GetSurvivorIndex());
        character.Look(food.transform);
        dialog.Launch(OnWalkAnimation, "You found food !");
    }

    /// <summary>
    /// Hides the UI and adds food to the inventory based on the wheel result.
    /// </summary>
    protected override void InernalEndEvent()
    {
        dialog.Hide();
        wheel.Hide();

        if (result != null)
        {
            switch (result.tag)
            {
                case EventFoodTag.FOOD_1:
                    inventory.AddFood(1);
                    SetCompletion(true);
                    break;

                case EventFoodTag.FOOD_2:
                    inventory.AddFood(2);
                    SetCompletion(true);
                    break;

                case EventFoodTag.FOOD_3:
                    inventory.AddFood(3);
                    SetCompletion(true);
                    break;
            }
        }
    }

    /// <summary>
    /// Hides the dialog and walks the character toward the food tree.
    /// </summary>
    private void OnWalkAnimation()
    {
        dialog.Hide();
        character.Walk(OnTreeAnimation, destination, false);
    }

    /// <summary>
    /// Triggers the food tree shake animation, which calls the wheel spin on completion.
    /// </summary>
    private void OnTreeAnimation()
    {
        food.Launch(OnWheelStartSpinning);
    }

    /// <summary>
    /// Launches the wheel spin and stores the result for use at the end of the event.
    /// </summary>
    private void OnWheelStartSpinning()
    {
        result = wheel.Launch(() =>
        {
            StartCoroutine(OnWheelEndSpinning());
        });
    }

    /// <summary>
    /// Waits after the wheel stops, hides it, then shows the reward dialogue.
    /// </summary>
    private IEnumerator OnWheelEndSpinning()
    {
        yield return new WaitForSeconds(2.0f);
        wheel.Hide();

        dialog.Launch(EndEvent, "You won: " + result.segment.name + "!", result.segment.sprite);
    }
}
