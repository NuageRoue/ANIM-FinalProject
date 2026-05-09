using UnityEngine;

public class Cloud : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Animator animator;
    
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void CleanCloud()
    {
        animator.SetTrigger("Destroy");
    }

    public void OnDestroyAnimation() 
    { 
        Destroy(gameObject);
    }
}
