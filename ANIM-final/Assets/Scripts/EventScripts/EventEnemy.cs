using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public enum EventEnemyTag
{
    SAFE,
    SKIP,
    HURT,
    NOSK,
}

public class EventEnemy : EventBase
{
    [SerializeField]
    EventEnemyUIWheel wheel;

    [SerializeField]
    NewUIDialog dialog;

    [SerializeField]
    TaggedWheelSegments<EventEnemyTag> defaultSegments = new();

    [SerializeField]
    TaggedWheelSegments<EventEnemyTag> sneakySegments = new();

    [SerializeField]
    TaggedWheelSegments<EventEnemyTag> bowSegments = new();

    TagSegment<EventEnemyTag> result = null;

    [SerializeField]
    Sprite sneakyAbilitySprite;

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

        if (survivor.hasSneakyAbility)
        {
            wheel.Create(sneakySegments);
        }
        else if (hasBow)
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
                case EventEnemyTag.SAFE:
                    SetCompletion(true);
                    break;
                case EventEnemyTag.SKIP:
                    break;
                case EventEnemyTag.HURT:
                    survivor.isHurt = true;
                    break;
            }
        }
    }

    private void OnFirstMessage()
    {
        dialog.Launch(OnSelectSneaky, "Oh no, something is moving in the bush!");
    }

    private void OnSelectSneaky()
    {
        if (survivor.hasSneakyAbility)
        {
            dialog.Launch(
                OnStartSneakWheel,
                "You are sneaky, you can avoid fighting.",
                sneakyAbilitySprite
            );
        }
        else if (hasBow)
        {
            dialog.Launch(
                OnStartFightingWheel,
                "You have the perfect tool for this kind of situation!",
                ResouceLoader.instance.FindByType(RessourceObjectType.BOW).sprite
            );
        }
        else
        {
            OnStartFightingWheel();
        }
    }

    private void OnStartSneakWheel()
    {
        dialog.Hide();

        result = wheel.Launch(() =>
        {
            StartCoroutine(OnEndSneakWheel());
        });
    }

    private IEnumerator OnEndSneakWheel()
    {
        yield return new WaitForSeconds(2.0f);
        wheel.Hide();

        if (IsSneakyEnough())
        {
            dialog.Launch(EndEvent, "You avoid successfully the fight.", sneakyAbilitySprite);
        }
        else
        {
            if (hasBow)
            {
                wheel.Create(bowSegments);
                dialog.Launch(
                    OnStartFightingWheel,
                    "Oh no, the enemy saw you! But you have the perfect tool for this kind of situation.",
                    ResouceLoader.instance.FindByType(RessourceObjectType.BOW).sprite
                );
            }
            else
            {
                wheel.Create(defaultSegments);
                dialog.Launch(OnStartFightingWheel, "Oh no, the enemy saw you!");
            }
        }
    }

    private void OnStartFightingWheel()
    {
        dialog.Hide();

        result = wheel.Launch(() =>
        {
            StartCoroutine(OnEndFightingWheel());
        });
    }

    private IEnumerator OnEndFightingWheel()
    {
        yield return new WaitForSeconds(2.0f);
        wheel.Hide();

        if (IsHurt())
        {
            dialog.Launch(EndEvent, "The enemy hurt you.", result.segment.sprite);
        }
        else
        {
            dialog.Launch(EndEvent, "You won!", result.segment.sprite);
        }
    }

    private bool IsSneakyEnough()
    {
        return result != null && result.tag == EventEnemyTag.SKIP;
    }

    private bool IsHurt()
    {
        return result != null && result.tag == EventEnemyTag.HURT;
    }
}
