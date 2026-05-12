using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum EventHoleTag
{
    IN_HOLE,
    SAFE,
}

/// <summary>
/// Handles the hole event sequence: walking the character into the hole,
/// optionally using a ladder, spinning the wheel, and applying the outcome.
/// </summary>
public class EventHole : EventBase
{
    [Header("References")]
    [SerializeField]
    CharacterAniamation character;

    [Header("Keypoints")]
    [SerializeField]
    Transform breforeHole;

    [SerializeField]
    Transform inHole;

    [SerializeField]
    Transform afterHole;

    [Header("UI")]
    [SerializeField]
    EventHoleUIWHeel wheel;

    [SerializeField]
    NewUIDialog dialog;

    [Header("Wheel Segments")]
    [SerializeField]
    TaggedWheelSegments<EventHoleTag> defaultSegments = new();

    [SerializeField]
    TaggedWheelSegments<EventHoleTag> ladderSegments = new();

    TagSegment<EventHoleTag> result = null;
    bool hasLadder = false;

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
    /// Sets up the character, checks for a ladder, builds the appropriate wheel,
    /// and begins the walk-and-fall sequence.
    /// </summary>
    protected override void InternalStartEvent()
    {
        character.Set(GetSurvivorIndex());
        character.Look(breforeHole);

        dialog.Hide();
        wheel.Hide();

        hasLadder = inventory.objectResources.Contains(RessourceObjectType.LADDER, 1);

        // Use ladder segments if the survivor has one
        if (hasLadder)
        {
            wheel.Create(ladderSegments);
        }
        else
        {
            wheel.Create(defaultSegments);
        }

        OnWalkingAndFalling();
    }

    /// <summary>
    /// Hides the UI and applies the wheel result, incapacitating the survivor if stuck.
    /// </summary>
    protected override void InernalEndEvent()
    {
        dialog.Hide();
        wheel.Hide();

        if (result != null)
        {
            switch (result.tag)
            {
                case EventHoleTag.IN_HOLE:
                    survivor.isIncapacitated = true;
                    break;

                case EventHoleTag.SAFE:
                    break;
            }
        }
    }

    /// <summary>
    /// Walks the character to the hole edge, then down into the hole.
    /// </summary>
    private void OnWalkingAndFalling()
    {
        character.Walk(() => character.Walk(OnFirstMessage, inHole, false), breforeHole, false);
    }

    /// <summary>
    /// Resets the character's rotation and shows the fall dialogue.
    /// Routes to ladder message if applicable, otherwise goes straight to the wheel.
    /// </summary>
    private void OnFirstMessage()
    {
        character.ResetRotation();

        UnityAction next = hasLadder ? HasLadder : OnWheelStart;

        dialog.Launch(next, "Oh no you fall in a hole!");
    }

    /// <summary>
    /// Shows a dialogue informing the survivor they have a ladder, then starts the wheel.
    /// </summary>
    private void HasLadder()
    {
        dialog.Launch(
            OnWheelStart,
            "But you have the perfect tool for this: a ladder.",
            ResouceLoader.instance.FindByType(RessourceObjectType.LADDER).sprite
        );
    }

    /// <summary>
    /// Hides the dialog and launches the wheel spin.
    /// </summary>
    private void OnWheelStart()
    {
        dialog.Hide();

        result = wheel.Launch(() =>
        {
            StartCoroutine(OnWheelFinish());
        });
    }

    /// <summary>
    /// Waits after the wheel stops, then either shows the stuck dialogue
    /// or walks the character out of the hole.
    /// </summary>
    private IEnumerator OnWheelFinish()
    {
        yield return new WaitForSeconds(2.0f);
        wheel.Hide();

        if (IsStuck())
        {
            dialog.Launch(EndEvent, "You are stuck!", result.segment.sprite);
        }
        else
        {
            character.Walk(FreeMessage, afterHole, false); // Walk out of the hole on success
        }
    }

    /// <summary>
    /// Resets the character's rotation and shows the escape success dialogue.
    /// </summary>
    private void FreeMessage()
    {
        character.ResetRotation();
        dialog.Launch(EndEvent, "You are free!");
    }

    /// <summary>
    /// Returns true if the wheel result leaves the survivor stuck in the hole.
    /// </summary>
    private bool IsStuck()
    {
        return result != null && result.tag == EventHoleTag.IN_HOLE;
    }
}
