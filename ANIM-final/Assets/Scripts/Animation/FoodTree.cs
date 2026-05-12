using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class FoodTree : MonoBehaviour
{
    /// <summary>
    /// Stores the initial transform state of a Rigidbody and controls
    /// when it gets released from kinematic mode to simulate falling physics.
    /// </summary>
    private class RigidbodyTransform
    {
        private Rigidbody rigidbody;
        private Vector3 position;
        private Quaternion rotation;

        private bool init = false;

        public RigidbodyTransform(Rigidbody r)
        {
            rigidbody = r;
            position = r.position; // Snapshot initial position for reset
            rotation = r.rotation; // Snapshot initial rotation for reset
        }

        /// <summary>
        /// Resets the rigidbody to its initial transform and makes it kinematic,
        /// marking it as ready to be launched.
        /// </summary>
        public void Init()
        {
            init = true;
            rigidbody.isKinematic = true;
            rigidbody.transform.SetPositionAndRotation(position, rotation);
        }

        /// <summary>
        /// Returns whether this rigidbody has been initialized and is ready to start.
        /// </summary>
        public bool CanStart()
        {
            return init;
        }

        /// <summary>
        /// Detaches the rigidbody from its parent, enables physics, waits for the given delay,
        /// then re-enables kinematic and restores the original parent.
        /// </summary>
        public IEnumerator Start(float delay)
        {
            init = false;

            Transform transform = rigidbody.transform.parent; // Save parent before detaching

            rigidbody.transform.SetParent(null); // Detach so physics aren't constrained by parent
            rigidbody.isKinematic = false;

            yield return new WaitForSeconds(delay);

            rigidbody.isKinematic = true;
            rigidbody.transform.SetParent(transform); // Restore original parent after physics settle
        }
    }

    [Header("References")]
    [SerializeField]
    MoovingTree tree;

    [SerializeField]
    GameObject food;

    [Header("Settings")]
    [SerializeField]
    float physicAnimationDuration = 3.0f;

    UnityAction onFinish = null;
    List<RigidbodyTransform> initals;
    int fallenIndex = 0;

    /// <summary>
    /// Collects all Rigidbody children of the food object and wraps them
    /// in RigidbodyTransform instances for later control.
    /// </summary>
    void Start()
    {
        initals = food.GetComponentsInChildren<Rigidbody>()
            .Select((r) => new RigidbodyTransform(r))
            .ToList();
    }

    /// <summary>
    /// Resets all food rigidbodies, then launches the tree animation.
    /// Each tree shake triggers the next food item to fall.
    /// Calls onFinish after all physics have settled.
    /// </summary>
    public void Launch(UnityAction onFinish)
    {
        fallenIndex = 0;
        this.onFinish = onFinish;
        initals.ForEach((r) => r.Init()); // Reset all food items to their initial positions
        tree.Launch(
            () => StartCoroutine(OnTreeAniamationEnd()), // Called when the full animation ends
            () => StartCoroutine(initals[fallenIndex++].Start(physicAnimationDuration)) // Called on each shake
        );
    }

    /// <summary>
    /// Starts physics on any food items that haven't fallen yet,
    /// then waits for the physics duration before invoking the finish callback.
    /// </summary>
    private IEnumerator OnTreeAniamationEnd()
    {
        initals
            .FindAll((r) => r.CanStart()) // Only start items that are still waiting
            .ForEach((r) => StartCoroutine(r.Start(physicAnimationDuration)));

        yield return new WaitForSeconds(physicAnimationDuration);

        onFinish?.Invoke();
    }
}
