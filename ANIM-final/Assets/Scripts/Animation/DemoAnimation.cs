using UnityEngine;

public class DemoAnimation : MonoBehaviour
{
    [SerializeField]
    MoovingTree moovingTree;

    [SerializeField]
    FoodTree foodTree;

    [SerializeField]
    BeeTree beeTree;

    public void StartMoovingTree()
    {
        Debug.Log("Mooving Tree Start");
        moovingTree.Launch(() => Debug.Log("Mooving Tree Finished"));
    }

    public void StartFoodTree()
    {
        Debug.Log("Food Tree Start");
        foodTree.Launch(() => Debug.Log("Food Tree Finished"));
    }

    public void StartBeeTree()
    {
        Debug.Log("Food Bee Start");
        beeTree.Launch(() => Debug.Log("Food Bee Finished"));
    }
}
