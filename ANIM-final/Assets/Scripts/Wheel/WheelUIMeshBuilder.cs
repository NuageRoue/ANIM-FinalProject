using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class WheelUIMeshBuilder : Graphic
{
    [SerializeField]
    float smoothness = 30;

    [SerializeField]
    private GameObject labelPrefab;

    [SerializeField]
    private RectTransform labelContainer;

    List<SegmentAttribute> segments = null;

    bool createLabel = false;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        BuildMesh(vh);
    }

    public void Build(List<SegmentAttribute> segments)
    {
        Assert.IsTrue(segments.Count > 0);
        Assert.IsTrue(Mathf.Abs(segments.Sum(seg => seg.coef) - 1) < 1e-6);

        this.segments = new List<SegmentAttribute>(segments);
        this.segments.Reverse();

        SetVerticesDirty();
        createLabel = true;
    }

    private void BuildMesh(VertexHelper vh)
    {
        float radius = Mathf.Min(rectTransform.rect.height, rectTransform.rect.width) * 0.5f; // Get radius
        float startAngle = Mathf.Deg2Rad * 90.0f;

        if (segments != null && segments.Count > 0)
        {
            foreach (var seg in segments)
            {
                float endAngle = Mathf.Deg2Rad * 360f * seg.coef + startAngle; // Current end angle

                int numberOfVertices = Mathf.Max(2, Mathf.RoundToInt(seg.coef * smoothness)); // Number of points for triangle

                vh.AddVert(Vector2.zero, seg.color, Vector2.zero);
                int centerIndex = vh.currentVertCount - 1; // Create center point

                Vector2 prev = new Vector2(Mathf.Cos(startAngle), Mathf.Sin(startAngle)) * radius; // Last point for triangle
                vh.AddVert(prev, seg.color, Vector2.zero);
                int prevIndex = vh.currentVertCount - 1;

                for (int i = 1; i <= numberOfVertices; ++i)
                {
                    float current_angle = Mathf.Lerp(
                        startAngle,
                        endAngle,
                        (float)i / numberOfVertices
                    ); // Curent angle

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
        else
        {
            int numberOfVertices = (int)smoothness;

            vh.AddVert(Vector2.zero, Color.white, Vector2.zero);
            int centerIndex = vh.currentVertCount - 1;
            vh.AddVert(new Vector2(Mathf.Cos(0), Mathf.Sin(0)) * radius, Color.white, Vector2.zero);

            for (int i = 1; i <= numberOfVertices; ++i)
            {
                float current_angle =
                    Mathf.Deg2Rad * Mathf.Lerp(0f, 360f, (float)i / numberOfVertices);
                vh.AddVert(
                    new Vector2(Mathf.Cos(current_angle), Mathf.Sin(current_angle)) * radius,
                    Color.white,
                    Vector2.zero
                );
                vh.AddTriangle(centerIndex, vh.currentVertCount - 2, vh.currentVertCount - 1);
            }
        }
    }

    void LateUpdate()
    {
        if (createLabel)
        {
            BuildLabel();
            createLabel = false;
        }
    }

    private void BuildLabel()
    {
        // Clear anciens labels
        for (int i = labelContainer.childCount - 1; i >= 0; --i)
        {
            Destroy(labelContainer.GetChild(i).gameObject);
        }

        if (segments.Count <= 0)
            return;

        float radius = Mathf.Min(rectTransform.rect.height, rectTransform.rect.width) * 0.40f;
        float startAngle = Mathf.Deg2Rad * 90.0f;

        foreach (var seg in segments)
        {
            float endAngle = Mathf.Deg2Rad * 360f * seg.coef + startAngle;

            float midAngle = (startAngle + endAngle) * 0.5f;

            Vector2 pos = 0.75f * radius * new Vector2(Mathf.Cos(midAngle), Mathf.Sin(midAngle));

            GameObject go = Instantiate(labelPrefab, labelContainer);
            RectTransform rt = go.GetComponent<RectTransform>();

            rt.anchoredPosition = pos;

            // Fill UI
            var img = go.transform.GetComponentInChildren<Image>();
            var txt = go.transform.GetComponentInChildren<TextMeshProUGUI>();

            rt.localScale = Vector3.one * 0.75f;
            rt.localRotation = Quaternion.Euler(0, 0, midAngle * Mathf.Rad2Deg - 90f);

            if (img != null)
                img.sprite = seg.sprite;
            if (txt != null)
                txt.text = seg.name;

            startAngle = endAngle;
        }
    }
}
