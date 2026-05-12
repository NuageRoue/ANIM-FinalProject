using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CharacterAniamation : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    Transform container;

    [Header("Movement Settings")]
    [SerializeField]
    float walkSpeed = 0.4f;

    [SerializeField]
    float runSpeed = 1f;

    Animator animator = null;
    GameObject survivor = null;
    Transform look = null;

    /// <summary>
    /// Initializes the animator and instantiates the survivor model for the given index.
    /// Destroys any previously loaded survivor before creating the new one.
    /// </summary>
    public void Set(int survivorIndex)
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (survivor != null)
        {
            Destroy(survivor); // Remove previous survivor model before loading a new one
        }

        survivor = Instantiate(ResouceLoader.instance.GetSurvivor(survivorIndex), container, false);
        animator.Rebind(); // Reset animator state to match the new model
    }

    /// <summary>
    /// Rotates the container to face the given transform and stores it as the look target.
    /// </summary>
    public void Look(Transform look)
    {
        this.look = look;
        container.LookAt(this.look, Vector3.up);
    }

    /// <summary>
    /// Resets the container's rotation to keep only the Y axis, removing any tilt.
    /// </summary>
    public void ResetRotation()
    {
        container.rotation = Quaternion.Euler(0, container.rotation.eulerAngles.y, 0);
    }

    /// <summary>
    /// Starts a walk coroutine that moves the character to the destination at walk speed.
    /// </summary>
    public void Walk(UnityAction onFinish, Transform destination, bool keepLooking = true)
    {
        StartCoroutine(InternalWalk(onFinish, destination, keepLooking));
    }

    /// <summary>
    /// Starts a run coroutine that moves the character to the destination at run speed.
    /// </summary>
    public void Run(UnityAction onFinish, Transform destination, bool keepLooking = true)
    {
        StartCoroutine(InternalRun(onFinish, destination, keepLooking));
    }

    /// <summary>
    /// Plays the walk animation, moves to the destination, then stops and calls onFinish.
    /// </summary>
    private IEnumerator InternalWalk(
        UnityAction onFinish,
        Transform destination,
        bool keepLooking = true
    )
    {
        StartWalk();
        yield return InternalMove(destination, walkSpeed, keepLooking);
        StopWalk();
        onFinish.Invoke();
    }

    /// <summary>
    /// Plays the run animation, moves to the destination, then stops and calls onFinish.
    /// </summary>
    private IEnumerator InternalRun(
        UnityAction onFinish,
        Transform destination,
        bool keepLooking = true
    )
    {
        StartRunning();
        yield return InternalMove(destination, runSpeed, keepLooking);
        StopRunning();
        onFinish.Invoke();
    }

    /// <summary>
    /// Moves the container toward the destination using linear interpolation over time.
    /// Restores the look target rotation once movement is complete, if applicable.
    /// </summary>
    private IEnumerator InternalMove(Transform destination, float speed, bool keepLooking)
    {
        container.LookAt(destination, Vector3.up); // Face the destination before moving

        Vector3 start = container.position;
        Vector3 end = destination.position;

        float distance = (start - end).sqrMagnitude;
        float time = 0.0f;
        float endTime = distance / speed; // Total duration based on squared distance and speed

        while (time < endTime)
        {
            container.position = Vector3.Lerp(start, end, time / endTime); // Interpolate position
            time += Time.deltaTime;
            yield return null;
        }

        if (look != null && keepLooking)
        {
            container.LookAt(look, Vector3.up); // Restore look target after movement
        }
    }

    private void StartWalk()
    {
        animator.SetBool("IsWalking", true);
        animator.Play("Walking");
    }

    private void StopWalk()
    {
        animator.SetBool("IsWalking", false);
    }

    private void StartRunning()
    {
        animator.SetBool("IsRunning", true);
        animator.Play("Running");
    }

    private void StopRunning()
    {
        animator.SetBool("IsRunning", false);
    }

    /// <summary>
    /// Plays the attack animation and calls onFinish once it completes.
    /// </summary>
    public void Attack(UnityAction onFinish)
    {
        StartCoroutine(InternalAttack(onFinish));
    }

    /// <summary>
    /// Triggers the attack animation, waits for it to finish, then invokes the callback.
    /// </summary>
    private IEnumerator InternalAttack(UnityAction onFinish)
    {
        animator.SetBool("IsAttacking", true);
        animator.Play("Attack");

        yield return new WaitForSeconds(2); // Wait for the attack animation to complete

        animator.SetBool("IsAttacking", false);
        onFinish?.Invoke();
    }
}
