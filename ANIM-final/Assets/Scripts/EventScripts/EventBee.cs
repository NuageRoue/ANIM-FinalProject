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

/// <summary>
/// Handles the bee event sequence: walking to the tree, spinning the wheel,
/// and applying the outcome (food gain or poisoning) to the survivor.
/// </summary>
public class EventBee : EventBase
{
    [Header("UI")]
    [SerializeField]
    EventBeeUIWheel wheel;

    [SerializeField]
    NewUIDialog dialog;

    [Header("References")]
    [SerializeField]
    BeeTree tree;

    [SerializeField]
    CharacterAniamation character;

    [Header("Keypoints")]
    [SerializeField]
    Transform keyPointEntry;

    [SerializeField]
    Transform keyPointPoisoned;

    [SerializeField]
    Transform beeAttack;

    [Header("Wheel Segments")]
    [SerializeField]
    TaggedWheelSegments<EventBeeTag> defaultSegments = new();

    [SerializeField]
    TaggedWheelSegments<EventBeeTag> bowSegments = new();

    TagSegment<EventBeeTag> result = null;
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
    /// Sets up the character, checks for a bow in the inventory,
    /// builds the appropriate wheel, and starts the event dialogue.
    /// </summary>
    protected override void InternalStartEvent()
    {
        character.Set(GetSurvivorIndex());
        character.Look(tree.transform);

        wheel.Hide();
        dialog.Hide();

        hasBow = inventory.objectResources.Contains(RessourceObjectType.BOW, 1);

        // Use bow-specific segments if the survivor has a bow
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

    /// <summary>
    /// Hides the UI and applies the wheel result to the survivor:
    /// poisoning them or adding the appropriate amount of food.
    /// </summary>
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

    /// <summary>
    /// Shows the opening dialogue. If the survivor has a bow, routes to the bow message first.
    /// </summary>
    private void OnFirstMessage()
    {
        UnityAction next = OnStartWalking;

        if (hasBow)
        {
            next = OnBowMessage; // Insert bow message before walking
        }

        wheel.Hide();
        dialog.Launch(next, "You have discovered a beehive!");
    }

    /// <summary>
    /// Shows a dialogue informing the survivor they can use their bow,
    /// then proceeds to walking.
    /// </summary>
    private void OnBowMessage()
    {
        tree.StopBee();
        dialog.Launch(
            OnStartWalking,
            "You can use your bow.",
            ResouceLoader.instance.FindByType(RessourceObjectType.BOW).sprite
        );
    }

    /// <summary>
    /// Hides the dialog and walks the character to the tree entry point.
    /// </summary>
    private void OnStartWalking()
    {
        dialog.Hide();
        character.Walk(OnStartTree, keyPointEntry);
    }

    /// <summary>
    /// Launches the tree shake animation, which triggers the wheel on completion.
    /// </summary>
    private void OnStartTree()
    {
        tree.Launch(OnWheelTurn);
    }

    /// <summary>
    /// Launches the wheel spin and stores the result for use at the end of the event.
    /// </summary>
    private void OnWheelTurn()
    {
        result = wheel.Launch(() =>
        {
            StartCoroutine(OnWheelFinish());
        });
    }

    /// <summary>
    /// Waits briefly after the wheel stops, then routes to the poisoned or food outcome.
    /// </summary>
    private IEnumerator OnWheelFinish()
    {
        yield return new WaitForSeconds(2.0f);
        wheel.Hide();

        if (IsPoisened())
        {
            tree.StartFollow(beeAttack); // Bees follow the character as they flee
            character.Look(keyPointPoisoned);
            character.Run(OnPoisened, keyPointPoisoned);
        }
        else
        {
            OnGainFood();
        }
    }

    /// <summary>
    /// Shows a dialogue confirming the food reward and ends the event.
    /// </summary>
    private void OnGainFood()
    {
        dialog.Launch(EndEvent, "You won " + result.segment.name + "!", result.segment.sprite);
    }

    /// <summary>
    /// Stops the bee follow, shows the poisoned dialogue, and ends the event.
    /// </summary>
    private void OnPoisened()
    {
        tree.StopFollow();
        dialog.Launch(EndEvent, "How no, you are poisoned!", result.segment.sprite);
    }

    /// <summary>
    /// Returns true if the wheel result is the poisoned outcome.
    /// </summary>
    private bool IsPoisened()
    {
        return result != null && result.tag == EventBeeTag.POISONED;
    }
}
