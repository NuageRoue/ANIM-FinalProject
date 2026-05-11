using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class FoodTree : MonoBehaviour
{
    private class RigidbodyTransform
    {
        private Rigidbody rigidbody;
        private Vector3 position;
        private Quaternion rotation;

        private bool init = false;

        public RigidbodyTransform(Rigidbody r)
        {
            rigidbody = r;
            position = r.position;
            rotation = r.rotation;
        }

        public void Init()
        {
            init = true;
            rigidbody.isKinematic = true;
            rigidbody.transform.SetPositionAndRotation(position, rotation);
        }

        public bool CanStart()
        {
            return init;
        }

        public IEnumerator Start(float delay)
        {
            init = false;

            Transform transform = rigidbody.transform.parent;

            rigidbody.transform.SetParent(null);
            rigidbody.isKinematic = false;

            yield return new WaitForSeconds(delay);

            rigidbody.isKinematic = true;
            rigidbody.transform.SetParent(transform);
        }
    }

    [SerializeField]
    MoovingTree tree;

    [SerializeField]
    GameObject food;

    [SerializeField]
    float physicAnimationDuration = 3.0f;

    UnityAction onFinish;

    List<RigidbodyTransform> initals;

    int fallenIndex = 0;

    void Start()
    {
        initals = food.GetComponentsInChildren<Rigidbody>()
            .Select((r) => new RigidbodyTransform(r))
            .ToList();
    }

    public void Launch(UnityAction onFinish)
    {
        fallenIndex = 0;
        this.onFinish = onFinish;
        initals.ForEach((r) => r.Init());
        tree.Launch(
            () => StartCoroutine(OnTreeAniamationEnd()),
            () => StartCoroutine(initals[fallenIndex++].Start(physicAnimationDuration))
        );
    }

    private IEnumerator OnTreeAniamationEnd()
    {
        initals
            .FindAll((r) => r.CanStart())
            .ForEach((r) => StartCoroutine(r.Start(physicAnimationDuration)));

        yield return new WaitForSeconds(physicAnimationDuration);

        onFinish.Invoke();
    }
}
