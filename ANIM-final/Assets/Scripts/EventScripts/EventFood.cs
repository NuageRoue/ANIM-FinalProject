using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventFoodTag
{
    FOOD_1,
    FOOD_2,
    FOOD_3,
}

public class EventFood : EventBase
{
    [SerializeField]
    CharacterAniamation character;

    [SerializeField]
    Transform destination;

    [SerializeField]
    FoodTree food;

    [SerializeField]
    NewUIDialog dialog;

    [SerializeField]
    EventFoodUIWheel wheel;

    [SerializeField]
    TaggedWheelSegments<EventFoodTag> foodSegments = new();

    TagSegment<EventFoodTag> result;

    protected override void Awake()
    {
        base.Awake();

        wheel.Hide();
        dialog.Hide();
    }

    protected override void InternalStartEvent()
    {
        wheel.Create(foodSegments);
        character.Set(GetSurvivorIndex());
        character.Look(food.transform);
        dialog.Launch(OnWalkAnimation, "You found food !");
    }

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

    private void OnWalkAnimation()
    {
        dialog.Hide();
        character.Walk(OnTreeAnimation, destination, false);
    }

    private void OnTreeAnimation()
    {
        food.Launch(OnWheelStartSpinning);
    }

    private void OnWheelStartSpinning()
    {
        result = wheel.Launch(() =>
        {
            StartCoroutine(OnWheelEndSpinning());
        });
    }

    private IEnumerator OnWheelEndSpinning()
    {
        yield return new WaitForSeconds(2.0f);
        wheel.Hide();

        dialog.Launch(EndEvent, "You won: " + result.segment.name + "!", result.segment.sprite);
    }
}
