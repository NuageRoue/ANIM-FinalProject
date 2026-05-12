using UnityEngine;
using UnityEngine.Events;

public class BeeTree : MonoBehaviour
{
    [SerializeField]
    MoovingTree tree;

    [SerializeField]
    ParticleSystem bees;

    [SerializeField]
    ParticleSystem angryBees;

    Transform parent;

    void Start()
    {
        parent = angryBees.transform.parent;

        bees.Play();
        angryBees.Stop();
    }

    public void Launch(UnityAction onFinish)
    {
        tree.Launch(onFinish);
    }

    public void StopBee()
    {
        bees.Stop();
    }

    public void StartFollow(Transform transform)
    {
        angryBees.Play();
        angryBees.transform.SetParent(transform, false);
    }

    public void StopFollow()
    {
        angryBees.transform.SetParent(parent, false);
        angryBees.Stop();
        angryBees.Clear();
    }
}
