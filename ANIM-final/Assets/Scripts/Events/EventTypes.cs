using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EventTarget
{
    public string sceneName;
}

public class EventContext
{
    public Survivor ActiveSurvivor;

    // Inventory à brancher
    // public Inventory CampInventory;
}

public class EventResult
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
    Food,
}
