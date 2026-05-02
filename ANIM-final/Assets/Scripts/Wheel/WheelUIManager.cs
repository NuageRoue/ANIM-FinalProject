using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class WheelUIManager : Graphic
{
    [SerializeField]
    List<SegmentAttribute> segments = new();

    [SerializeField]
    float duration = 1f;

    [SerializeField]
    float speen = 2f;

    [SerializeField]
    AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    WheelUIMeshBuilder wmb;

    protected override void Awake()
    {
        wmb = GetComponentInChildren<WheelUIMeshBuilder>();
        CleanSegments();
        wmb.Build(segments);
    }

    public void Build(List<SegmentAttribute> segments)
    {
        if (segments != this.segments && segments.Count > 0)
        {
            this.segments.Clear();
            this.segments.AddRange(segments);
            CleanSegments();
        }

        wmb.Build(this.segments);
    }

    public async Task<SegmentAttribute> Speen()
    {
        SegmentAttribute result = segments[0];

        float rand = Random.value;

        float endAngle = 0;

        float sum = 0;
        foreach (var segment in segments)
        {
            float nextSum = sum + segment.coef;

            if (rand <= nextSum)
            {
                endAngle = Mathf.Lerp(sum, nextSum, rand) * 360;
                result = segment;
                break;
            }

            sum = nextSum;
        }

        float startAngle = wmb.rectTransform.rotation.eulerAngles.z;
        await Speening(startAngle, endAngle);

        return result;
    }

    private async Task Speening(float startAngle, float endAngle)
    {
        endAngle += speen * 360f + startAngle;

        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float current = curve.Evaluate(elapsed / duration);
            float angle = Mathf.Lerp(startAngle, endAngle, current);
            wmb.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
            await Task.Yield();
        }

        wmb.rectTransform.rotation = Quaternion.Euler(0, 0, endAngle);

        float dt = Time.deltaTime;
    }

    private void CleanSegments()
    {
        float total = segments.Sum(seg => seg.coef);
        foreach (var segment in this.segments)
        {
            segment.coef /= total; // Normalize
            segment.color.a = 1.0f; // Set alpha
        }
    }
}
