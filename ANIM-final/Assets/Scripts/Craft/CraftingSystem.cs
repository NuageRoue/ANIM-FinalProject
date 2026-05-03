using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    [SerializeField]
    public List<CraftingRecipe> recipes = new();

    [SerializeField]
    public Inventory inventoy = new();

    public bool CanCraft(CraftingRecipe recipe)
    {
        var inventoryDict = ResourcePair.ToDict(inventoy.ressources);
        var receipeInputDict = ResourcePair.ToDict(recipe.inputs);

        var differenceDict = ResourcePair.Difference(inventoryDict, receipeInputDict);

        return !ResourcePair.HasLessThanZero(differenceDict);
    }

    public bool Craft(CraftingRecipe recipe)
    {
        var inventoryDict = ResourcePair.ToDict(inventoy.ressources);
        var receipeInputDict = ResourcePair.ToDict(recipe.inputs);

        var differenceDict = ResourcePair.Difference(inventoryDict, receipeInputDict);

        if (ResourcePair.HasLessThanZero(differenceDict))
            return false;

        var receipeOutputDict = ResourcePair.ToDict(recipe.outputs);

        var newInventory = ResourcePair.Add(differenceDict, receipeOutputDict);

        inventoy.ressources = ResourcePair.FromDict(newInventory);
        ResourcePair.RemoveEmpty(inventoy.ressources);

        return true;
    }
}
