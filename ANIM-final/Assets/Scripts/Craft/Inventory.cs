using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Inventory
{
    [SerializeField]
    public ListMap<ResourceType> baseResources = new();

    [SerializeField]
    public ListMap<RessourceObjectType> objectResources = new();

    public bool CanCraft(CraftingRecipe recipe)
    {
        return baseResources.ContainsAll(recipe.inputResources)
            && !objectResources.Contains(recipe.outputObject, 1);
    }

    public bool Craft(CraftingRecipe recipe)
    {
        if (!CanCraft(recipe))
            return false;

        foreach (var item in recipe.inputResources.items)
        {
            baseResources.Remove(item.type, item.count);
        }

        objectResources.Add(recipe.outputObject, 1);

        return true;
    }

    public void AddItem(ResourceType item, int amount = 1)
    {
        baseResources.Add(item, amount);
    }

    public void AddFood(int amount)
    {
        baseResources.Add(ResourceType.Food, amount);
    }
}
