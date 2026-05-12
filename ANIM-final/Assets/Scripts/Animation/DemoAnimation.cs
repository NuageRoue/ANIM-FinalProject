using System.Collections;
using UnityEngine;

public class DemoAnimation : MonoBehaviour
{
    [SerializeField]
    MoovingTree moovingTree;

    [SerializeField]
    FoodTree foodTree;

    [SerializeField]
    BeeTree beeTree;

    [SerializeField]
    Transform beeTarget;

    [SerializeField]
    CharacterAniamation character;

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

    public void StartFollowing()
    {
        StartCoroutine(FollowCoroutine());
    }

    public void SetSurvivor()
    {
        character.Set(1);

        StartCoroutine(SurvivorSequence());
    }

    private IEnumerator SurvivorSequence()
    {
        character.Look(beeTarget);

        yield return new WaitForSeconds(1);

        character.Launch(() => Debug.Log("Finish"));
    }

    private IEnumerator FollowCoroutine()
    {
        var original = beeTarget.transform.position;

        beeTree.StartFollow(beeTarget);

        float time = 0.0f;

        while (time < 3.0f)
        {
            float direction = (Mathf.FloorToInt(time / 1.0f) & 1) > 0 ? 1 : -1;
            beeTarget.transform.position += Time.deltaTime * direction * 3 * Vector3.right;

            time += Time.deltaTime;
            yield return null;
        }

        beeTree.StopFollow();

        beeTarget.transform.position = original;
    }
}
