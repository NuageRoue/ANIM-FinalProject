using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CharacterAniamation : MonoBehaviour
{
    [SerializeField]
    Transform container;

    [SerializeField]
    Transform destination;

    [SerializeField]
    float speed = 1;

    Animator animator = null;

    GameObject survivor = null;

    Transform look = null;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public void Set(int survivorIndex)
    {
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
        survivor.gameObject.transform.LookAt(this.look, Vector3.up);
    }

    public void Launch(UnityAction onFinish)
    {
        StartCoroutine(InternalWalk(onFinish));
    }

    private IEnumerator InternalWalk(UnityAction onFinish)
    {
        StartWalk();

        survivor.gameObject.transform.LookAt(destination, Vector3.up);

        Vector3 start = survivor.transform.position;
        Vector3 end = destination.position;

        float distance = (start - end).sqrMagnitude;
        float time = 0.0f;

        float endTime = distance / speed;

        while (time < endTime)
        {
            survivor.gameObject.transform.position = Vector3.Lerp(start, end, time / endTime);

            time += Time.deltaTime;
            yield return null;
        }

        StopWalk();

        if (look != null)
        {
            survivor.gameObject.transform.LookAt(look, Vector3.up);
        }

        onFinish.Invoke();
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
}
