using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ListMapItem<T>
    where T : Enum
{
    [SerializeField]
    public T type;

    [SerializeField]
    public int count;
}

[Serializable]
public class ListMap<T>
    where T : Enum
{
    [SerializeField]
    public List<ListMapItem<T>> items = new();

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

        items.Add(new ListMapItem<T> { type = type, count = amount });
    }

    public bool Remove(T type, int amount)
    {
        foreach (var rm in items)
        {
            if (EqualityComparer<T>.Default.Equals(rm.type, type))
            {
                if (rm.count < amount)
                {
                    return false;
                }

                if (rm.count == amount)
                {
                    items.Remove(rm);
                    return true;
                }

                rm.count -= amount;
                return true;
            }
        }

        return false;
    }

    public bool Contains(T type, int amount)
    {
        foreach (var rm in items)
        {
            if (EqualityComparer<T>.Default.Equals(rm.type, type))
            {
                return rm.count >= amount;
            }
        }
        return false;
    }

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
