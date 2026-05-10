using UnityEngine;

public enum EventFishingTag
{
    FISH,
    NO_FISH,
}

public class EventFishing : EventBase
{
    [SerializeField]
    EventFishingUIWheel wheel;

    [SerializeField]
    NewUIDialog dialog;

    [SerializeField]
    Inventory inventory;

    [SerializeField]
    TaggedWheelSegments<EventFishingTag> defaultSegments = new();

    [SerializeField]
    TaggedWheelSegments<EventFishingTag> fishingRodSegments = new();

    TagSegment<EventFishingTag> result = null;

    bool hasFishingRod = false;

    void Start()
    {
        StartEvent();
    }

    public override void StartEvent()
    {
        hasFishingRod = inventory.objectResources.Contains(RessourceObjectType.FISHING_ROD, 1);

        wheel.Hide();
        dialog.Hide();
    }

    public override void EndEvent() { }



    
}
