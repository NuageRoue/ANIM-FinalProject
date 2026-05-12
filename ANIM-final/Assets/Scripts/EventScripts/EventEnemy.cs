using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

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
    CharacterAniamation character;

    [SerializeField]
    Transform sneakDestination;

    [SerializeField]
    Transform combatDestinationCharacter;

    [SerializeField]
    CharacterAniamation enemy;

    [SerializeField]
    Transform combatDestinationEnemy;

    [SerializeField]
    MoovingTree tree;

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

    [SerializeField]
    Sprite strongAbilitySprite;

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

        character.Set(GetSurvivorIndex());
        character.Look(tree.transform);

        hasBow = inventory.objectResources.Contains(RessourceObjectType.BOW, 1);

        tree.Launch(OnFirstMessage);
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
        UnityAction next = OnStartFighting;

        if (survivor.hasSneakyAbility)
        {
            next = OnStartSneak;
        }

        dialog.Launch(next, "Oh no, something is moving in the bush.");
    }

    private void OnStartSneak()
    {
        wheel.Create(sneakySegments);

        dialog.Launch(
            OnStartSneakWheel,
            "You are sneaky, you can avoid fighting.",
            sneakyAbilitySprite
        );
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
            dialog.Launch(OnAvoidCombat, "You avoid successfully the fight.", sneakyAbilitySprite);
        }
        else
        {
            OnStartFighting();
        }
    }

    private void OnAvoidCombat()
    {
        dialog.Hide();
        character.Run(EndEvent, sneakDestination, false);
    }

    private void OnStartFighting()
    {
        enemy.Set(GetEnemyIndex());
        dialog.Launch(OnAnimationCombatStart, "Oh no, the enemy saw you!");
    }

    private void OnAnimationCombatStart()
    {
        dialog.Hide();
        enemy.Run(
            () =>
            {
                enemy.Look(combatDestinationCharacter);
            },
            combatDestinationEnemy,
            false
        );

        character.Walk(
            () =>
            {
                character.Look(combatDestinationEnemy);
                OnSelectBow();
            },
            combatDestinationCharacter,
            false
        );
    }

    private void OnSelectBow()
    {
        if (hasBow)
        {
            dialog.Launch(
                () => OnSelectStrong(bowSegments),
                "You have the perfect tool for this kind of situation!",
                ResouceLoader.instance.FindByType(RessourceObjectType.BOW).sprite
            );
        }
        else
        {
            OnSelectStrong(defaultSegments);
        }
    }

    private void OnSelectStrong(TaggedWheelSegments<EventEnemyTag> segments)
    {
        if (survivor.hasStrongAbility)
        {
            segments = segments.Copy();
            segments.ModifyWithConstrains(EventEnemyTag.HURT, (f) => 0.5f * f);
        }

        wheel.Create(segments);

        if (survivor.hasStrongAbility)
        {
            dialog.Launch(
                OnStartFightingWheel,
                "The enemy doesn't stand a chance against you!",
                strongAbilitySprite
            );
        }
        else
        {
            OnStartFightingWheel();
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
            enemy.Attack(() =>
                dialog.Launch(EndEvent, "The enemy hurt you.", result.segment.sprite)
            );
        }
        else
        {
            character.Attack(() => dialog.Launch(EndEvent, "You won!", result.segment.sprite));
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
