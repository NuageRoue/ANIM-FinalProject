using System.Collections.Generic;

public class EventContext
{
    public Survivor ActiveSurvivor;

    // Inventory à brancher
    // public Inventory CampInventory;
}

public struct EventResult
{
    public Dictionary<ResourceType, int> ResourcesGained;
    public int DamageDealt;
    public bool RevealCell;
}

// Placeholder
public enum ResourceType
{
    Wood,
    Stone,
    Food
}
