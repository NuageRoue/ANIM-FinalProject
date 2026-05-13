using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a single entry in a ListMap, associating an enum type with a count.
/// </summary>
[Serializable]
public class ListMapItem<T>
    where T : Enum
{
    [SerializeField]
    public T type;

    [SerializeField]
    public int count;
}

/// <summary>
/// A serializable dictionary-like structure backed by a list, mapping enum values to integer counts.
/// Supports adding, removing, and querying entries by type.
/// </summary>
[Serializable]
public class ListMap<T>
    where T : Enum
{
    [SerializeField]
    public List<ListMapItem<T>> items = new();

    public ListMap<T> Clone()
    {
        ListMap<T> clone = new();
        foreach (var item in items)
            clone.items.Add(new ListMapItem<T> { type = item.type, count = item.count });
        return clone;
    }

    /// <summary>
    /// Adds the given amount to an existing entry of the matching type,
    /// or creates a new entry if none exists.
    /// </summary>
    public void Add(T type, int amount)
    {
        foreach (var rm in items)
        {
            if (EqualityComparer<T>.Default.Equals(rm.type, type))
            {
                rm.count += amount;
                return;
            }
        }

        items.Add(new ListMapItem<T> { type = type, count = amount }); // No existing entry, create one
    }

    /// <summary>
    /// Removes the given amount from the matching entry.
    /// Returns false if the type is not found or the count is insufficient.
    /// Removes the entry entirely if the count reaches zero.
    /// </summary>
    public bool Remove(T type, int amount)
    {
        foreach (var rm in items)
        {
            if (EqualityComparer<T>.Default.Equals(rm.type, type))
            {
                if (rm.count < amount)
                    return false; // Not enough to remove

                if (rm.count == amount)
                {
                    items.Remove(rm); // Count hits zero, remove the entry entirely
                    return true;
                }

                rm.count -= amount;
                return true;
            }
        }

        return false; // Type not found
    }

    /// <summary>
    /// Returns true if the list contains at least the given amount of the specified type.
    /// </summary>
    public bool Contains(T type, int amount)
    {
        foreach (var rm in items)
        {
            if (EqualityComparer<T>.Default.Equals(rm.type, type))
            {
                return rm.count >= amount;
            }
        }
        return false; // Type not found
    }

    /// <summary>
    /// Returns true if this list contains all entries from the given ListMap
    /// with at least the required counts.
    /// </summary>
    public bool ContainsAll(ListMap<T> listMap)
    {
        foreach (var item in listMap.items)
        {
            if (!Contains(item.type, item.count))
                return false;
        }

        return true;
    }
}
