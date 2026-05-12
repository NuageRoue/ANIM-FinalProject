using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum EventEnemyTag
{
    SAFE,
    SKIP,
    HURT,
    NOSK,
}

/// <summary>
/// Handles the enemy encounter event: optionally attempting a sneak,
/// then entering combat with ability-based wheel modifiers and applying the outcome.
/// </summary>
public class EventEnemy : EventBase
{
    [Header("Characters")]
    [SerializeField]
    CharacterAniamation character;

    [SerializeField]
    CharacterAniamation enemy;

    [Header("Keypoints")]
    [SerializeField]
    Transform sneakDestination;

    [SerializeField]
    Transform combatDestinationCharacter;

    [SerializeField]
    Transform combatDestinationEnemy;

    [Header("References")]
    [SerializeField]
    MoovingTree tree;

    [Header("UI")]
    [SerializeField]
    EventEnemyUIWheel wheel;

    [SerializeField]
    NewUIDialog dialog;

    [Header("Wheel Segments")]
    [SerializeField]
    TaggedWheelSegments<EventEnemyTag> defaultSegments = new();

    [SerializeField]
    TaggedWheelSegments<EventEnemyTag> sneakySegments = new();

    [SerializeField]
    TaggedWheelSegments<EventEnemyTag> bowSegments = new();

    [Header("Ability Sprites")]
    [SerializeField]
    Sprite sneakyAbilitySprite;

    [SerializeField]
    Sprite strongAbilitySprite;

    TagSegment<EventEnemyTag> result = null;
    bool hasBow = false;

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
    /// Sets up the character, checks for a bow, launches the tree animation,
    /// then begins the event dialogue sequence.
    /// </summary>
    protected override void InternalStartEvent()
    {
        wheel.Hide();
        dialog.Hide();

        character.Set(GetSurvivorIndex());
        character.Look(tree.transform);

        hasBow = inventory.objectResources.Contains(RessourceObjectType.BOW, 1);

        tree.Launch(OnFirstMessage);
    }

    /// <summary>
    /// Hides the UI and applies the wheel result: marking the event complete,
    /// skipping, or hurting the survivor.
    /// </summary>
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

    /// <summary>
    /// Shows the opening dialogue. Routes to sneak if the survivor has the sneaky ability,
    /// otherwise goes straight to combat.
    /// </summary>
    private void OnFirstMessage()
    {
        UnityAction next = survivor.hasSneakyAbility ? OnStartSneak : OnStartFighting;

        dialog.Launch(next, "Oh no, something is moving in the bush.");
    }

    /// <summary>
    /// Builds the sneak wheel and shows the sneak ability dialogue before spinning.
    /// </summary>
    private void OnStartSneak()
    {
        wheel.Create(sneakySegments);

        dialog.Launch(
            OnStartSneakWheel,
            "You are sneaky, you can avoid fighting.",
            sneakyAbilitySprite
        );
    }

    /// <summary>
    /// Hides the dialog and launches the sneak wheel spin.
    /// </summary>
    private void OnStartSneakWheel()
    {
        dialog.Hide();
        result = wheel.Launch(() =>
        {
            StartCoroutine(OnEndSneakWheel());
        });
    }

    /// <summary>
    /// Waits after the sneak wheel stops, then either avoids combat or falls through to fighting.
    /// </summary>
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
            OnStartFighting(); // Sneak failed, proceed to combat
        }
    }

    /// <summary>
    /// Runs the character away from the encounter to avoid combat.
    /// </summary>
    private void OnAvoidCombat()
    {
        dialog.Hide();
        character.Run(EndEvent, sneakDestination, false);
    }

    /// <summary>
    /// Sets up the enemy character and shows the combat start dialogue.
    /// </summary>
    private void OnStartFighting()
    {
        enemy.Set(GetEnemyIndex());
        dialog.Launch(OnAnimationCombatStart, "Oh no, the enemy saw you!");
    }

    /// <summary>
    /// Hides the dialog and plays the approach animations for both characters,
    /// then proceeds to bow or default segment selection.
    /// </summary>
    private void OnAnimationCombatStart()
    {
        dialog.Hide();

        enemy.Run(
            () => enemy.Look(combatDestinationCharacter), // Enemy faces the character on arrival
            combatDestinationEnemy,
            false
        );

        character.Walk(
            () =>
            {
                character.Look(combatDestinationEnemy); // Character faces the enemy on arrival
                OnSelectBow();
            },
            combatDestinationCharacter,
            false
        );
    }

    /// <summary>
    /// Shows a bow dialogue if the survivor has one, then proceeds to strong ability selection.
    /// Otherwise skips directly to strong ability selection with default segments.
    /// </summary>
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

    /// <summary>
    /// Applies the strong ability modifier to the HURT segment if applicable,
    /// builds the combat wheel, and optionally shows the strong ability dialogue.
    /// </summary>
    private void OnSelectStrong(TaggedWheelSegments<EventEnemyTag> segments)
    {
        if (survivor.hasStrongAbility)
        {
            segments = segments.Copy();
            segments.ModifyWithConstrains(EventEnemyTag.HURT, (f) => 0.5f * f); // Halve the hurt chance
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

    /// <summary>
    /// Hides the dialog and launches the combat wheel spin.
    /// </summary>
    private void OnStartFightingWheel()
    {
        dialog.Hide();

        result = wheel.Launch(() =>
        {
            StartCoroutine(OnEndFightingWheel());
        });
    }

    /// <summary>
    /// Waits after the combat wheel stops, then plays the winner's attack animation
    /// and shows the outcome dialogue.
    /// </summary>
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

    /// <summary>
    /// Returns true if the sneak wheel result allows skipping combat.
    /// </summary>
    private bool IsSneakyEnough()
    {
        return result != null && result.tag == EventEnemyTag.SKIP;
    }

    /// <summary>
    /// Returns true if the combat wheel result is the hurt outcome.
    /// </summary>
    private bool IsHurt()
    {
        return result != null && result.tag == EventEnemyTag.HURT;
    }
}
