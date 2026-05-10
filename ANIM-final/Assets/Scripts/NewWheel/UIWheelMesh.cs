using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class UIWheelMesh : Graphic
{
    [SerializeField]
    float smoothness = 30;

    [SerializeField]
    GameObject wheelComponent;

    [SerializeField]
    RectTransform wheelComponentContainer;

    List<SegmentAttribute> segments = null;

    public void Create(List<SegmentAttribute> segments)
    {
        Assert.IsTrue(segments.Count > 0);
        Assert.IsTrue(Mathf.Abs(segments.Sum(seg => seg.coef) - 1) < 1e-6);

        this.segments = new List<SegmentAttribute>(segments);
        this.segments.Reverse();

        SetVerticesDirty();

        ClearContainer();
        BuildLabelWithSegments();
    }

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

    private void BuildMeshWithSegments(VertexHelper vh)
    {
        Assert.IsTrue(segments != null && segments.Count > 0);

        float radius = GetRadius();
        float startAngle = Mathf.Deg2Rad * 90.0f;

        foreach (var seg in segments)
        {
            float endAngle = Mathf.Deg2Rad * 360f * seg.coef + startAngle; // Current end angle

            int numberOfVertices = Mathf.Max(2, Mathf.RoundToInt(seg.coef * smoothness)); // Number of points for triangle

            vh.AddVert(Vector3.zero, seg.color, Vector2.zero);
            int centerIndex = vh.currentVertCount - 1; // Create center point

            Vector2 prev = new Vector2(Mathf.Cos(startAngle), Mathf.Sin(startAngle)) * radius; // Last point for triangle
            vh.AddVert(prev, seg.color, Vector2.zero);
            int prevIndex = vh.currentVertCount - 1;

            for (int i = 1; i <= numberOfVertices; ++i)
            {
                float current_angle = Mathf.Lerp(startAngle, endAngle, (float)i / numberOfVertices); // Curent angle

                Vector2 vertex =
                    new Vector2(Mathf.Cos(current_angle), Mathf.Sin(current_angle)) * radius; // Current vertex
                vh.AddVert(vertex, seg.color, Vector2.zero); // Add it

                int currentIndex = vh.currentVertCount - 1;
                vh.AddTriangle(centerIndex, prevIndex, currentIndex); // Create triangle with 2 last and center

                prevIndex = currentIndex;
            }

            startAngle = endAngle; // Update start angle
        }
    }

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

    private float GetRadius()
    {
        return Mathf.Min(rectTransform.rect.height, rectTransform.rect.width) * 0.5f;
    }

    private void ClearContainer()
    {
        var components = wheelComponentContainer.GetComponentsInChildren<UIWheelComponent>();

        foreach (var component in components)
        {
            Destroy(component.gameObject);
        }
    }

    private void BuildLabelWithSegments()
    {
        Assert.IsTrue(segments != null && segments.Count > 0);

        float radius = GetRadius();

        float startAngle = Mathf.Deg2Rad * 90.0f;

        foreach (var seg in segments)
        {
            float endAngle = Mathf.Deg2Rad * 360f * seg.coef + startAngle;

            float midAngle = (startAngle + endAngle) * 0.5f;

            Vector2 pos = 0.7f * radius * new Vector2(Mathf.Cos(midAngle), Mathf.Sin(midAngle));

            GameObject go = Instantiate(wheelComponent, wheelComponentContainer);
            RectTransform rt = go.GetComponent<RectTransform>();

            rt.anchoredPosition = pos;

            UIWheelComponent uiwc = go.GetComponent<UIWheelComponent>();

            rt.localScale = Vector3.one * 0.6f;
            rt.localRotation = Quaternion.Euler(0, 0, midAngle * Mathf.Rad2Deg - 90f);

            uiwc.Set(seg);

            startAngle = endAngle;
        }
    }
}
