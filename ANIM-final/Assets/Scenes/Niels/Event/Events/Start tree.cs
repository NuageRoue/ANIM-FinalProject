using UnityEngine;

public class Starttree : MonoBehaviour
{
    [SerializeField]
    MoovingTree mt;

    void Start()
    {
        mt.SetOnAnimationFinish(() =>
        {
            Debug.Log("animation end");
        });
        mt.StartAnimation();
    }
}
