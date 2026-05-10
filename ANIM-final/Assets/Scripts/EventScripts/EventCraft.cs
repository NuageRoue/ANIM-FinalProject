using System.Collections;
using UnityEngine;

public class EventCraft : EventBase
{
    [SerializeField]
    Inventory inventory = new();

    [SerializeField]
    UICraftingSystem uics;

    void Awake()
    {
        StartEvent();
    }

    public override void StartEvent()
    {
        base.StartEvent();

        uics.SetInventory(inventory);

        StartCoroutine(OnEventStart());
    }

    public override void EndEvent()
    {
        base.EndEvent();
    }

    private IEnumerator OnEventStart()
    {
        uics.Hide();

        const float endTime = 1.0f;
        float time = 0;

        while (time < endTime)
        {
            time += Time.deltaTime;
            yield return null;
        }

        uics.Reveal();

        uics.OnFinish(OnCraftingEnd);
        StartCoroutine(uics.StartCrafting());
    }

    private void OnCraftingEnd()
    {
        uics.Hide();
    }
}
