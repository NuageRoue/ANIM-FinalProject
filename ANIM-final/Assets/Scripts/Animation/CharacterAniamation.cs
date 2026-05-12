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

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Set(int survivorIndex)
    {
        Debug.Log(animator);

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

    public void Walk(UnityAction onFinish, Transform destination)
    {
        StartCoroutine(InternalWalk(onFinish, destination));
    }

    public void Run(UnityAction onFinish, Transform destination)
    {
        StartCoroutine(InternalRun(onFinish, destination));
    }

    private IEnumerator InternalWalk(UnityAction onFinish, Transform destination)
    {
        StartWalk();

        yield return InternalMove(destination, walkSpeed);

        StopWalk();

        onFinish.Invoke();
    }

    private IEnumerator InternalRun(UnityAction onFinish, Transform destination)
    {
        StartRunning();

        yield return InternalMove(destination, runSpeed);

        StopRunning();

        onFinish.Invoke();
    }

    private IEnumerator InternalMove(Transform destination, float speed)
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

        if (look != null)
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
}
