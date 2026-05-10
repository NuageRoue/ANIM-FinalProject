using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class TagSegment<T>
    where T : Enum
{
    [SerializeField]
    public T tag;

    [SerializeField]
    public SegmentAttribute segment;
}

[Serializable]
public class TaggedWheelSegments<T>
    where T : Enum
{
    [SerializeField]
    public List<TagSegment<T>> items = new();

    public void Normalize()
    {
        float total = items.Sum((item) => item.segment.coef);

        foreach (var item in items)
        {
            item.segment.coef /= total;
            item.segment.color.a = 1.0f;
        }
    }

    public TaggedWheelSegments<T> Copy()
    {
        TaggedWheelSegments<T> result = new();

        foreach (var item in items)
        {
            result.items.Add(new TagSegment<T> { tag = item.tag, segment = item.segment });
        }

        return result;
    }

    public List<SegmentAttribute> AsListOfSegments()
    {
        return new(items.Select((item) => item.segment));
    }
}
