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
    BeeTree tree;

    [SerializeField]
    CharacterAniamation character;

    [SerializeField]
    Transform keyPointEntry;

    [SerializeField]
    Transform keyPointPoisoned;

    [SerializeField]
    Transform beeAttack;

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
        character.Set(GetSurvivorIndex());
        character.Look(tree.transform);

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
        UnityAction next = OnStartWalking;

        if (hasBow)
        {
            next = OnBowMessage;
        }

        wheel.Hide();
        dialog.Launch(next, "You have discovered a beehive!");
    }

    private void OnBowMessage()
    {
        tree.StopBee();
        dialog.Launch(
            OnStartWalking,
            "You can use your bow.",
            ResouceLoader.instance.FindByType(RessourceObjectType.BOW).sprite
        );
    }

    private void OnStartWalking()
    {
        dialog.Hide();
        character.Walk(OnStartTree, keyPointEntry);
    }

    private void OnStartTree()
    {
        tree.Launch(OnWheelTurn);
    }

    private void OnWheelTurn()
    {
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
            tree.StartFollow(beeAttack);
            character.Look(keyPointPoisoned);
            character.Run(OnPoisened, keyPointPoisoned);
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

    private void OnPoisened()
    {
        tree.StopFollow();
        dialog.Launch(EndEvent, "How no, you are poisoned!", result.segment.sprite);
    }

    private bool IsPoisened()
    {
        return result != null && result.tag == EventBeeTag.POISONED;
    }
}
