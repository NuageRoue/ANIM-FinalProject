using System;
using System.Collections.Generic;

[Serializable]
public class Inventory
{
    public List<ResourcePair> ressources = new();

    public void Add(ResourceType type, int amount)
    {
        ResourcePair.FindOrCreate(ressources, type).amount += amount;
    }

    public bool Remove(ResourceType type, int amount)
    {
        ResourcePair current = ResourcePair.Find(ressources, type);
        if (current == null)
            return false;

        if (current.amount < amount)
            return false;

        current.amount -= amount;

        if (current.amount <= 0)
            ressources.Remove(current);

        return true;
    }
}
