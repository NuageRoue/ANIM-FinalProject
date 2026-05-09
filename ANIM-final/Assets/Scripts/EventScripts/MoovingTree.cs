using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MoovingTree : MonoBehaviour
{
    [SerializeField]
    GameObject foodContainer;

    [SerializeField]
    float animationDuration = 5;

    [SerializeField]
    float numberOfSubAnimation = 3;

    [SerializeField]
    float rotationFactor = 10;

    [SerializeField]
    AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    UnityAction onAnimationFinish;

    void Awake()
    {
        foreach (var item in foodContainer.GetComponentsInChildren<Rigidbody>())
        {
            item.isKinematic = true;
        }
    }

    public void SetOnAnimationFinish(UnityAction call)
    {
        onAnimationFinish = call;
    }

    public void StartAnimation()
    {
        StartCoroutine(Animation());
    }

    private IEnumerator Animation()
    {
        float timePerAnimation = (numberOfSubAnimation + 1.0f) / animationDuration;
        float halfTimePerAnimation = timePerAnimation * 0.5f;

        float time = 0f;

        while (time < halfTimePerAnimation)
        {
            float u = curve.Evaluate(time / halfTimePerAnimation);
            float r = rotationFactor * u;
            transform.localRotation = Quaternion.Euler(0, 0, r);
            time += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < numberOfSubAnimation; ++i)
        {
            time = 0;
            while (time < halfTimePerAnimation)
            {
                float u = curve.Evaluate(time / halfTimePerAnimation);
                float r = rotationFactor * (1 - 2 * u);
                transform.localRotation = Quaternion.Euler(0, 0, r);
                time += Time.deltaTime;
                yield return null;
            }

            time = 0;
            while (time < halfTimePerAnimation)
            {
                float u = curve.Evaluate(time / halfTimePerAnimation);
                float r = rotationFactor * (2 * u - 1);
                transform.localRotation = Quaternion.Euler(0, 0, r);
                time += Time.deltaTime;
                yield return null;
            }
        }

        time = 0;
        while (time < halfTimePerAnimation)
        {
            float u = curve.Evaluate(time / halfTimePerAnimation);
            float r = rotationFactor * (1 - u);
            transform.localRotation = Quaternion.Euler(0, 0, r);
            time += Time.deltaTime;
            yield return null;
        }

        foreach (var item in foodContainer.GetComponentsInChildren<Rigidbody>())
        {
            item.isKinematic = false;
        }

        transform.localRotation = Quaternion.Euler(0, 0, 0);
        onAnimationFinish?.Invoke();
    }
}
