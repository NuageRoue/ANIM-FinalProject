using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine.UIElements;

[System.Serializable]
public class ResourcePair
{
    public ResourceType type;
    public int amount;

    public static Dictionary<ResourceType, int> ToDict(List<ResourcePair> list)
    {
        Dictionary<ResourceType, int> res = new();

        foreach (var rp in list)
        {
            res[rp.type] = res.GetValueOrDefault(rp.type) + rp.amount;
        }

        return res;
    }

    public static List<ResourcePair> FromDict(Dictionary<ResourceType, int> dict)
    {
        return dict.Select((kv) => new ResourcePair { amount = kv.Value, type = kv.Key }).ToList();
    }

    public static Dictionary<ResourceType, int> Difference(
        Dictionary<ResourceType, int> from,
        Dictionary<ResourceType, int> with
    )
    {
        Dictionary<ResourceType, int> res = new();
        res.AddRange(from);

        foreach (var kv in with)
        {
            res[kv.Key] = res.GetValueOrDefault(kv.Key) - kv.Value;
        }

        return res;
    }

    public static Dictionary<ResourceType, int> Add(
        Dictionary<ResourceType, int> from,
        Dictionary<ResourceType, int> with
    )
    {
        Dictionary<ResourceType, int> res = new();
        res.AddRange(from);

        foreach (var kv in with)
        {
            res[kv.Key] = res.GetValueOrDefault(kv.Key) + kv.Value;
        }

        return res;
    }

    public static bool HasLessThanZero(Dictionary<ResourceType, int> dict)
    {
        foreach (var kv in dict)
        {
            if (kv.Value < 0)
                return true;
        }
        return false;
    }

    public static ResourcePair Find(List<ResourcePair> list, ResourceType type)
    {
        foreach (var rp in list)
        {
            if (rp.type == type)
                return rp;
        }
        return null;
    }

    public static ResourcePair FindOrCreate(List<ResourcePair> list, ResourceType type)
    {
        ResourcePair result = Find(list, type);
        if (result != null)
            return result;

        result = new ResourcePair { amount = 0, type = type };
        list.Add(result);

        return result;
    }

    public static void RemoveEmpty(List<ResourcePair> list)
    {
        list.RemoveAll((rp) => rp.amount <= 0);
    }
}
