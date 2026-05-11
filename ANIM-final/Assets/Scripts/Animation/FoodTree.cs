using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class FoodTree : MonoBehaviour
{
    private class RigidbodyTransform
    {
        public Rigidbody rigidbody;
        public Vector3 position;
        public Quaternion rotation;
    }

    [SerializeField]
    MoovingTree tree;

    [SerializeField]
    GameObject food;

    UnityAction onFinish;

    List<RigidbodyTransform> initals;

    int fallenIndex = 0;

    void Start()
    {
        initals = food.GetComponentsInChildren<Rigidbody>()
            .Select(
                (r) =>
                    new RigidbodyTransform
                    {
                        position = r.position,
                        rotation = r.rotation,
                        rigidbody = r,
                    }
            )
            .ToList();
    }

    public void Launch(UnityAction onFinish)
    {
        this.onFinish = onFinish;
        initals.ForEach(
            (r) =>
            {
                r.rigidbody.isKinematic = true;
                r.rigidbody.linearVelocity = Vector3.zero;
                r.rigidbody.angularVelocity = Vector3.zero;
                r.rigidbody.position = r.position;
                r.rigidbody.rotation = r.rotation;
            }
        );
        tree.Launch(
            () => StartCoroutine(OnTreeAniamationEnd()),
            () => StartCoroutine(OnTreeAnimationCycle())
        );
    }

    private IEnumerator OnTreeAnimationCycle()
    {
        if (fallenIndex < initals.Count)
        {
            initals[fallenIndex].rigidbody.transform.SetParent(null);
            initals[fallenIndex].rigidbody.isKinematic = false;
            yield return new WaitForSeconds(3.0f);
            initals[fallenIndex].rigidbody.isKinematic = true;

            fallenIndex++;
        }
    }

    private IEnumerator OnTreeAniamationEnd()
    {
        for (int i = fallenIndex; i < initals.Count; ++i)
        {
            initals[i].rigidbody.isKinematic = false;
        }

        yield return new WaitForSeconds(3.0f);

        for (int i = fallenIndex; i < initals.Count; ++i)
        {
            initals[i].rigidbody.isKinematic = true;
        }

        initals.ForEach((i) => i.rigidbody.transform.SetParent(food.transform));

        onFinish.Invoke();
    }
}
