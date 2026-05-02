using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class WheelUIMeshBuilder : Graphic
{
    [SerializeField]
    float smoothness = 30;

    List<SegmentAttribute> segments;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        BuildMesh(vh);
    }

    public void Build(List<SegmentAttribute> segments)
    {
        Assert.IsTrue(segments.Count > 0);
        Assert.IsTrue(Mathf.Abs(segments.Sum(seg => seg.coef) - 1) < 1e-6);

        this.segments = segments;

        SetVerticesDirty();
    }

    private void BuildMesh(VertexHelper vh)
    {
        float radius = Mathf.Min(rectTransform.rect.height, rectTransform.rect.width) * 0.5f; // Get radius
        float startAngle = Mathf.Deg2Rad * (90f - segments[0].coef * 360f);

        foreach (var seg in segments)
        {
            float endAngle = Mathf.Deg2Rad * 360f * seg.coef + startAngle; // Current end angle

            int numberOfVertices = Mathf.Max(2, Mathf.RoundToInt(seg.coef * smoothness)); // Number of points for triangle

            int centerIndex = vh.currentVertCount; // Create center point
            vh.AddVert(Vector2.zero, seg.color, Vector2.zero);

            Vector2 last = new Vector2(Mathf.Cos(startAngle), Mathf.Sin(startAngle)) * radius; // Last point for triangle
            vh.AddVert(last, seg.color, Vector2.zero);

            for (int i = 1; i <= numberOfVertices; ++i)
            {
                float current_angle = Mathf.Lerp(startAngle, endAngle, (float)i / numberOfVertices); // Curent angle

                Vector2 vertex =
                    new Vector2(Mathf.Cos(current_angle), Mathf.Sin(current_angle)) * radius; // Current vertex
                vh.AddVert(vertex, seg.color, Vector2.zero); // Add it
                last = vertex; // Update last

                int index = vh.currentVertCount;
                vh.AddTriangle(centerIndex, index - 2, index - 1); // Create triangle with 2 last and center
            }

            startAngle = endAngle; // Update start angle
        }
    }
}
