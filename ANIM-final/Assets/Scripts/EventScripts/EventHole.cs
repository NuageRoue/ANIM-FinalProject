using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum EventHoleTag
{
    IN_HOLE,
    SAFE,
}

public class EventHole : EventBase
{
    [SerializeField]
    CharacterAniamation character;

    [SerializeField]
    Transform breforeHole;

    [SerializeField]
    Transform inHole;

    [SerializeField]
    Transform afterHole;

    [SerializeField]
    EventHoleUIWHeel wheel;

    [SerializeField]
    NewUIDialog dialog;

    [SerializeField]
    TaggedWheelSegments<EventHoleTag> defaultSegments = new();

    [SerializeField]
    TaggedWheelSegments<EventHoleTag> ladderSegments = new();

    TagSegment<EventHoleTag> result = null;

    bool hasLadder = false;

    protected override void Awake()
    {
        base.Awake();

        wheel.Hide();
        dialog.Hide();
    }

    protected override void InternalStartEvent()
    {
        character.Set(GetSurvivorIndex());
        character.Look(breforeHole);

        dialog.Hide();
        wheel.Hide();
        hasLadder = inventory.objectResources.Contains(RessourceObjectType.LADDER, 1);

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

    private void OnWalkingAndFalling()
    {
        character.Walk(() => character.Walk(OnFirstMessage, inHole, false), breforeHole, false);
    }

    private void OnFirstMessage()
    {
        character.ResetRotation();
        UnityAction next = OnWheelStart;

        if (hasLadder)
        {
            next = HasLadder;
        }

        dialog.Launch(next, "Oh no you fall in a hole!");
    }

    private void HasLadder()
    {
        dialog.Launch(
            OnWheelStart,
            "But you have the perfect tool for this: a ladder.",
            ResouceLoader.instance.FindByType(RessourceObjectType.LADDER).sprite
        );
    }

    private void OnWheelStart()
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

        if (IsStuck())
        {
            dialog.Launch(EndEvent, "You are stuck!", result.segment.sprite);
        }
        else
        {
            character.Walk(FreeMessage, afterHole, false);
        }
    }

    private void FreeMessage()
    {
        character.ResetRotation();
        dialog.Launch(EndEvent, "You are free!");
    }

    private bool IsStuck()
    {
        return result != null && result.tag == EventHoleTag.IN_HOLE;
    }
}
