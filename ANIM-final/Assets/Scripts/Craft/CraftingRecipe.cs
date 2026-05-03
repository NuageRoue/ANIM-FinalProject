using System;
using System.Collections.Generic;

[Serializable]
public class CraftingRecipe
{
    public List<ResourcePair> inputs = new();
    public List<ResourcePair> outputs = new();
}
