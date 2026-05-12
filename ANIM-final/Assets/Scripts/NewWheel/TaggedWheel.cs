using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Associates an enum tag with a single wheel segment,
/// allowing segments to be identified and modified by type.
/// </summary>
[Serializable]
public class TagSegment<T>
    where T : Enum
{
    [SerializeField]
    public T tag;

    [SerializeField]
    public SegmentAttribute segment;
}

/// <summary>
/// A serializable collection of tagged wheel segments.
/// Provides utilities to normalize, copy, convert, and modify segments by tag.
/// </summary>
[Serializable]
public class TaggedWheelSegments<T>
    where T : Enum
{
    [SerializeField]
    public List<TagSegment<T>> items = new();

    /// <summary>
    /// Normalizes all segment coefficients so they sum to 1
    /// and ensures each segment color is fully opaque.
    /// </summary>
    public void Normalize()
    {
        float total = items.Sum((item) => item.segment.coef);

        foreach (var item in items)
        {
            item.segment.coef /= total; // Normalize so all coefficients sum to 1
            item.segment.color.a = 1.0f;
        }
    }

    /// <summary>
    /// Returns a shallow copy of this collection with the same tags and segment references.
    /// </summary>
    public TaggedWheelSegments<T> Copy()
    {
        TaggedWheelSegments<T> result = new();

        foreach (var item in items)
        {
            result.items.Add(new TagSegment<T> { tag = item.tag, segment = item.segment });
        }

        return result;
    }

    /// <summary>
    /// Returns the segments as a plain list, stripping the tag data.
    /// </summary>
    public List<SegmentAttribute> AsListOfSegments()
    {
        return new(items.Select((item) => item.segment));
    }

    /// <summary>
    /// Applies the given modifier function to the coefficient of the segment matching the tag.
    /// </summary>
    public void ModifyWithConstrains(T tag, Func<float, float> modifier)
    {
        foreach (var item in items)
        {
            if (EqualityComparer<T>.Default.Equals(item.tag, tag))
            {
                item.segment.coef = modifier(item.segment.coef); // Apply modifier to matching segment only
            }
        }
    }
}
