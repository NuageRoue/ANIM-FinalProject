using UnityEngine;
using UnityEngine.Events;

public class MoovingTree : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    Animator animator;

    [SerializeField]
    ParticleSystem leafs;

    [Header("Settings")]
    [SerializeField]
    int numberOfAnimation = 3;

    UnityAction onFinish;
    UnityAction onCycleFinish;

    /// <summary>
    /// Ensures the leaf particles are stopped on initialization.
    /// </summary>
    void Start()
    {
        leafs.Stop();
    }

    /// <summary>
    /// Starts the tree shake animation for the configured number of cycles,
    /// plays the leaf particles, and stores the callbacks for cycle and finish events.
    /// </summary>
    public void Launch(UnityAction onFinish, UnityAction onCycleFinish = null)
    {
        this.onFinish = onFinish;
        this.onCycleFinish = onCycleFinish;

        animator.SetBool("On", true);
        animator.SetInteger("Count", numberOfAnimation); // Set the number of shake cycles

        leafs.Play();
    }

    /// <summary>
    /// Called by the animator at the end of each shake cycle.
    /// Decrements the remaining count, invokes the cycle callback, or ends the animation
    /// and triggers onFinish when all cycles are complete.
    /// </summary>
    public void OnCycleEnd()
    {
        int count = animator.GetInteger("Count");

        if (count <= 0)
        {
            animator.SetBool("On", false); // Stop the shake animation
            leafs.Stop();
            onFinish.Invoke();
        }
        else
        {
            onCycleFinish?.Invoke();
            animator.SetInteger("Count", count - 1); // Decrement remaining cycles
        }
    }
}
