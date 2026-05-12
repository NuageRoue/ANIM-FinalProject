using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CharacterAniamation : MonoBehaviour
{
    [SerializeField]
    Transform container;

    [SerializeField]
    float walkSpeed = 0.4f;

    [SerializeField]
    float runSpeed = 1f;

    Animator animator = null;

    GameObject survivor = null;

    Transform look = null;

    public void Set(int survivorIndex)
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (survivor != null)
        {
            Destroy(survivor);
        }

        survivor = Instantiate(ResouceLoader.instance.GetSurvivor(survivorIndex), container, false);
        animator.Rebind();
    }

    public void Look(Transform look)
    {
        this.look = look;
        container.LookAt(this.look, Vector3.up);
    }

    public void ResetRotation()
    {
        container.rotation = Quaternion.Euler(0, container.rotation.eulerAngles.y, 0);
    }

    public void Walk(UnityAction onFinish, Transform destination, bool keepLooking = true)
    {
        StartCoroutine(InternalWalk(onFinish, destination, keepLooking));
    }

    public void Run(UnityAction onFinish, Transform destination, bool keepLooking = true)
    {
        StartCoroutine(InternalRun(onFinish, destination, keepLooking));
    }

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

    private IEnumerator InternalMove(Transform destination, float speed, bool keepLooking)
    {
        container.LookAt(destination, Vector3.up);

        Vector3 start = container.position;
        Vector3 end = destination.position;

        float distance = (start - end).sqrMagnitude;
        float time = 0.0f;

        float endTime = distance / speed;

        while (time < endTime)
        {
            container.position = Vector3.Lerp(start, end, time / endTime);

            time += Time.deltaTime;
            yield return null;
        }

        if (look != null && keepLooking)
        {
            container.LookAt(look, Vector3.up);
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

    public void Attack(UnityAction onFinish)
    {
        StartCoroutine(InternalAttack(onFinish));
    }

    private IEnumerator InternalAttack(UnityAction onFinish)
    {
        animator.SetBool("IsAttacking", true);

        animator.Play("Attack");
        yield return new WaitForSeconds(2);

        animator.SetBool("IsAttacking", false);

        onFinish?.Invoke();
    }
}
