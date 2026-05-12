using UnityEngine;
using UnityEngine.Events;

public class BeeTree : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    MoovingTree tree;

    [Header("Particles")]
    [SerializeField]
    ParticleSystem bees;

    [SerializeField]
    ParticleSystem angryBees;

    Transform parent; // Original parent of angryBees, saved to restore it later

    /// <summary>
    /// Saves the original parent of angryBees, starts the idle bee particles
    /// and ensures the angry bees are stopped on initialization.
    /// </summary>
    void Start()
    {
        parent = angryBees.transform.parent; // Store original parent before any reparenting
        bees.Play();
        angryBees.Stop();
    }

    /// <summary>
    /// Triggers the tree's launch sequence and calls the given callback when finished.
    /// </summary>
    public void Launch(UnityAction onFinish)
    {
        tree.Launch(onFinish);
    }

    /// <summary>
    /// Stops the idle bee particle system.
    /// </summary>
    public void StopBee()
    {
        bees.Stop();
    }

    /// <summary>
    /// Attaches the angry bees to the given transform and starts their particle system,
    /// making them follow the target.
    /// </summary>
    public void StartFollow(Transform transform)
    {
        angryBees.Play();
        angryBees.transform.SetParent(transform, false); // Reparent to target so bees follow it
    }

    /// <summary>
    /// Detaches the angry bees from their current target, restores their original parent,
    /// stops and clears the particle system.
    /// </summary>
    public void StopFollow()
    {
        angryBees.transform.SetParent(parent, false); // Restore original parent
        angryBees.Stop();
        angryBees.Clear(); // Remove any remaining particles immediately
    }
}
