using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Builds the wheel's pie-chart mesh using Unity's Graphic system
/// and instantiates a UIWheelComponent label for each segment.
/// </summary>
public class UIWheelMesh : Graphic
{
    [Header("Settings")]
    [SerializeField]
    float smoothness = 30;

    [Header("References")]
    [SerializeField]
    GameObject wheelComponent;

    [SerializeField]
    RectTransform wheelComponentContainer;

    List<SegmentAttribute> segments = null;

    /// <summary>
    /// Validates and stores the given segments, triggers a mesh rebuild,
    /// and recreates all segment labels in the container.
    /// </summary>
    public void Create(List<SegmentAttribute> segments)
    {
        Assert.IsTrue(segments.Count > 0);
        Assert.IsTrue(Mathf.Abs(segments.Sum(seg => seg.coef) - 1) < 1e-6); // Coefficients must sum to exactly 1

        this.segments = new List<SegmentAttribute>(segments);
        this.segments.Reverse();

        SetVerticesDirty();

        ClearContainer();
        BuildLabelWithSegments();
    }

    /// <summary>
    /// Called by Unity whenever the mesh needs to be rebuilt.
    /// Delegates to the appropriate builder based on whether segments exist.
    /// </summary>
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (segments == null || segments.Count == 0)
        {
            BuildMeshWithoutSegments(vh);
        }
        else
        {
            BuildMeshWithSegments(vh);
        }
    }

    /// <summary>
    /// Builds the pie-chart mesh by generating a fan of triangles for each segment.
    /// </summary>
    private void BuildMeshWithSegments(VertexHelper vh)
    {
        Assert.IsTrue(segments != null && segments.Count > 0);

        float radius = GetRadius();
        float startAngle = Mathf.Deg2Rad * 90.0f; // Start at the top of the circle

        foreach (var seg in segments)
        {
            float endAngle = Mathf.Deg2Rad * 360f * seg.coef + startAngle;
            int numberOfVertices = Mathf.Max(2, Mathf.RoundToInt(seg.coef * smoothness)); // More vertices for larger segments

            vh.AddVert(Vector3.zero, seg.color, Vector2.zero);
            int centerIndex = vh.currentVertCount - 1; // Center point shared by all triangles in this segment

            Vector2 prev = new Vector2(Mathf.Cos(startAngle), Mathf.Sin(startAngle)) * radius;
            vh.AddVert(prev, seg.color, Vector2.zero);
            int prevIndex = vh.currentVertCount - 1;

            for (int i = 1; i <= numberOfVertices; ++i)
            {
                float current_angle = Mathf.Lerp(startAngle, endAngle, (float)i / numberOfVertices);

                Vector2 vertex =
                    new Vector2(Mathf.Cos(current_angle), Mathf.Sin(current_angle)) * radius;
                vh.AddVert(vertex, seg.color, Vector2.zero);

                int currentIndex = vh.currentVertCount - 1;
                vh.AddTriangle(centerIndex, prevIndex, currentIndex); // Fan triangle from center to arc edge

                prevIndex = currentIndex;
            }

            startAngle = endAngle; // Advance angle for the next segment
        }
    }

    /// <summary>
    /// Builds a plain white circle mesh used as a fallback when no segments are defined.
    /// </summary>
    private void BuildMeshWithoutSegments(VertexHelper vh)
    {
        float radius = GetRadius();
        int numberOfVertices = (int)smoothness;

        vh.AddVert(Vector2.zero, Color.white, Vector2.zero);
        int centerIndex = vh.currentVertCount - 1;
        vh.AddVert(new Vector2(Mathf.Cos(0), Mathf.Sin(0)) * radius, Color.white, Vector2.zero);

        for (int i = 1; i <= numberOfVertices; ++i)
        {
            float current_angle = Mathf.Deg2Rad * Mathf.Lerp(0f, 360f, (float)i / numberOfVertices);
            vh.AddVert(
                new Vector2(Mathf.Cos(current_angle), Mathf.Sin(current_angle)) * radius,
                Color.white,
                Vector2.zero
            );
            vh.AddTriangle(centerIndex, vh.currentVertCount - 2, vh.currentVertCount - 1);
        }
    }

    /// <summary>
    /// Returns the largest radius that fits within the RectTransform's bounds.
    /// </summary>
    private float GetRadius()
    {
        return Mathf.Min(rectTransform.rect.height, rectTransform.rect.width) * 0.5f;
    }

    /// <summary>
    /// Destroys all existing UIWheelComponent children in the container.
    /// </summary>
    private void ClearContainer()
    {
        var components = wheelComponentContainer.GetComponentsInChildren<UIWheelComponent>();

        foreach (var component in components)
        {
            Destroy(component.gameObject);
        }
    }

    /// <summary>
    /// Instantiates a UIWheelComponent for each segment, positioned and rotated
    /// at the midpoint angle of that segment.
    /// </summary>
    private void BuildLabelWithSegments()
    {
        Assert.IsTrue(segments != null && segments.Count > 0);

        float radius = GetRadius();
        float startAngle = Mathf.Deg2Rad * 90.0f;

        foreach (var seg in segments)
        {
            float endAngle = Mathf.Deg2Rad * 360f * seg.coef + startAngle;
            float midAngle = (startAngle + endAngle) * 0.5f; // Place label at the center of the segment arc

            Vector2 pos = 0.7f * radius * new Vector2(Mathf.Cos(midAngle), Mathf.Sin(midAngle));

            GameObject go = Instantiate(wheelComponent, wheelComponentContainer);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;

            UIWheelComponent uiwc = go.GetComponent<UIWheelComponent>();

            rt.localScale = Vector3.one * 0.6f;
            rt.localRotation = Quaternion.Euler(0, 0, midAngle * Mathf.Rad2Deg - 90f); // Rotate label to align with segment direction

            uiwc.Set(seg);

            startAngle = endAngle;
        }
    }
}
