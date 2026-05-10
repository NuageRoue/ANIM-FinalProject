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
    NewUIDialog dialog;

    [SerializeField]
    EventFoodUIWheel wheel;

    [SerializeField]
    TaggedWheelSegments<EventFoodTag> foodSegments = new();

    TagSegment<EventFoodTag> result;

    protected override void InternalStartEvent()
    {
        StartCoroutine(OnEventStarted());
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

    private IEnumerator OnEventStarted()
    {
        dialog.Hide();
        wheel.Hide();

        wheel.Create(foodSegments);

        float time = 0;

        while (time < 1.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        dialog.Launch(OnWheelStartSpinning, "You found food !");
    }

    private void OnWheelStartSpinning()
    {
        dialog.Hide();
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
