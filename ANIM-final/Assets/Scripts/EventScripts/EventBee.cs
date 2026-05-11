using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum EventBeeTag
{
    POISONED,
    FOOD_1,
    FOOD_2,
    FOOD_3,
}

public class EventBee : EventBase
{
    [SerializeField]
    EventBeeUIWheel wheel;

    [SerializeField]
    NewUIDialog dialog;

    [SerializeField]
    TaggedWheelSegments<EventBeeTag> defaultSegments = new();

    [SerializeField]
    TaggedWheelSegments<EventBeeTag> bowSegments = new();

    TagSegment<EventBeeTag> result = null;

    bool hasBow = false;

    protected override void Awake()
    {
        base.Awake();

        wheel.Hide();
        dialog.Hide();
    }

    protected override void InternalStartEvent()
    {
        wheel.Hide();
        dialog.Hide();

        hasBow = inventory.objectResources.Contains(RessourceObjectType.BOW, 1);

        if (hasBow)
        {
            wheel.Create(bowSegments);
        }
        else
        {
            wheel.Create(defaultSegments);
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
                case EventBeeTag.POISONED:
                    survivor.isHurt = true;
                    break;

                case EventBeeTag.FOOD_1:
                    inventory.AddFood(1);
                    SetCompletion(true);
                    break;

                case EventBeeTag.FOOD_2:
                    inventory.AddFood(2);
                    SetCompletion(true);
                    break;

                case EventBeeTag.FOOD_3:
                    inventory.AddFood(3);
                    SetCompletion(true);
                    break;
            }
        }
    }

    private void OnFirstMessage()
    {
        UnityAction next = OnWheelTurn;

        if (hasBow)
        {
            next = OnBowMessage;
        }

        wheel.Hide();
        dialog.Launch(next, "You have discovered a beehive!");
    }

    private void OnBowMessage()
    {
        dialog.Launch(
            OnWheelTurn,
            "You can use your bow.",
            ResouceLoader.instance.FindByType(RessourceObjectType.BOW).sprite
        );
    }

    private void OnWheelTurn()
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

        if (IsPoisened())
        {
            StartCoroutine(OnPoisened());
        }
        else
        {
            OnGainFood();
        }
    }

    private void OnGainFood()
    {
        dialog.Launch(EndEvent, "You won " + result.segment.name + "!", result.segment.sprite);
    }

    private IEnumerator OnPoisened()
    {
        yield return new WaitForSeconds(1.0f);

        dialog.Launch(EndEvent, "How no, you are poisoned!", result.segment.sprite);
    }

    private bool IsPoisened()
    {
        return result != null && result.tag == EventBeeTag.POISONED;
    }
}
