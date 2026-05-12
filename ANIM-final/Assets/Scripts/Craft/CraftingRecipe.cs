using System;
using UnityEngine;

[Serializable]
public class CraftingRecipe
{
    [SerializeField]
    public ListMap<ResourceType> inputResources = new();

    [SerializeField]
    public RessourceObjectType outputObject;
}
