using System.Collections;
using UnityEngine;

public class EventCraft : EventBase
{
    [SerializeField]
    UICraftingSystem uics;

    protected override void InternalStartEvent()
    {
        uics.SetInventory(inventory);

        StartCoroutine(OnEventStart());
    }

    protected override void InernalEndEvent() { }

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
