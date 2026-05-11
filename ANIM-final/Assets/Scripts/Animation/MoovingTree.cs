using UnityEngine;
using UnityEngine.Events;

public class MoovingTree : MonoBehaviour
{
    [SerializeField]
    Animator animator;

    [SerializeField]
    ParticleSystem leafs;

    [SerializeField]
    int numberOfAnimation = 3;

    UnityAction onFinish;

    UnityAction onCycleFinish;

    void Start()
    {
        leafs.Stop();
    }

    public void Launch(UnityAction onFinish, UnityAction onCycleFinish = null)
    {
        this.onFinish = onFinish;
        this.onCycleFinish = onCycleFinish;

        animator.SetBool("On", true);
        animator.SetInteger("Count", numberOfAnimation);

        leafs.Play();
    }

    public void OnCycleEnd()
    {
        int count = animator.GetInteger("Count");

        if (count <= 0)
        {
            animator.SetBool("On", false);
            leafs.Stop();
            onFinish.Invoke();
        }
        else
        {
            onCycleFinish?.Invoke();
            animator.SetInteger("Count", count - 1);
        }
    }
}
