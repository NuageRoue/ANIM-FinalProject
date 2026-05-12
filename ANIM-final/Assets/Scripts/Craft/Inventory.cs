using System;
using UnityEngine;

/// <summary>
/// Holds a character's or player's resources and crafted objects.
/// Provides methods for crafting recipes and adding items or food.
/// </summary>
[Serializable]
public class Inventory
{
    [Header("Resources")]
    [SerializeField]
    public ListMap<ResourceType> baseResources = new();

    [SerializeField]
    public ListMap<RessourceObjectType> objectResources = new();

    /// <summary>
    /// Returns true if the inventory has all required input resources for the recipe
    /// and does not already own the output object.
    /// </summary>
    public bool CanCraft(CraftingRecipe recipe)
    {
        return baseResources.ContainsAll(recipe.inputResources)
            && !objectResources.Contains(recipe.outputObject, 1); // Prevent crafting duplicates
    }

    /// <summary>
    /// Attempts to craft the given recipe by consuming its input resources
    /// and adding the output object. Returns false if crafting requirements are not met.
    /// </summary>
    public bool Craft(CraftingRecipe recipe)
    {
        if (!CanCraft(recipe))
            return false;

        foreach (var item in recipe.inputResources.items)
        {
            baseResources.Remove(item.type, item.count); // Consume each required resource
        }

        objectResources.Add(recipe.outputObject, 1);

        return true;
    }

    /// <summary>
    /// Adds the given amount of a base resource to the inventory.
    /// </summary>
    public void AddItem(ResourceType item, int amount = 1)
    {
        baseResources.Add(item, amount);
    }

    /// <summary>
    /// Convenience method to add food directly to the base resources.
    /// </summary>
    public void AddFood(int amount)
    {
        baseResources.Add(ResourceType.Food, amount);
    }
}
