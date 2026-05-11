using UnityEngine;
using UnityEngine.Events;

public class BeeTree : MonoBehaviour
{
    [SerializeField]
    MoovingTree tree;

    [SerializeField]
    ParticleSystem bees;

    void Start()
    {
        // bees.Play();
    }

    public void Launch(UnityAction onFinish)
    {
        tree.Launch(onFinish);
    }
}
