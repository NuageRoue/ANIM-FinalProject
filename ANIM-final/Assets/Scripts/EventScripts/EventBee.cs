using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventBeeTag
{
    POISONED,
    FOOD,
}

public class EventBee : EventBase
{
    [SerializeField]
    EventBeeUIWheel wheel;

    [SerializeField]
    NewUIDialog dialog;

    [SerializeField]
    Inventory inventory;

    [SerializeField]
    TaggedWheelSegments<EventBeeTag> defaultSegments = new();

    [SerializeField]
    TaggedWheelSegments<EventBeeTag> bowSegments = new();

    TagSegment<EventBeeTag> result = null;

    bool hasBow = false;

    void Start()
    {
        wheel.Hide();
        dialog.Hide();
        StartEvent();
    }

    public override void StartEvent()
    {
        base.StartEvent();

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

    public override void EndEvent()
    {
        wheel.Hide();
        dialog.Hide();

        base.EndEvent();
    }

    private void OnFirstMessage()
    {
        wheel.Hide();
        dialog.Launch(
            () =>
            {
                if (hasBow)
                {
                    OnBowMessage();
                }
                else
                {
                    OnNoBowMessage();
                }
            },
            "You have discovered a beehive!"
        );
    }

    private void OnBowMessage()
    {
        dialog.Launch(
            OnWheelTurn,
            "You can use your bow.",
            ResouceLoader.instance.FindByType(RessourceObjectType.BOW).sprite
        );
    }

    private void OnNoBowMessage()
    {
        OnWheelTurn();
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
