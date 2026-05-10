using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventFood : EventBase
{
    [SerializeField]
    MoovingTree moovingTree;

    [SerializeField]
    UIDialog dialog;

    [SerializeField]
    WheelUIManager wheelManager;

    [SerializeField]
    List<SegmentAttribute> foodSegments;

    SegmentAttribute result;

    void Start()
    {
        //StartEvent();
        dialog.Hide();
        wheelManager.Hide();
    }

    public override void StartEvent()
    {
        base.StartEvent();
        StartCoroutine(OnEventStarted());
    }

    public override void EndEvent()
    {
        dialog.Hide();
        wheelManager.Hide();

        base.EndEvent();

        Debug.Log(result.name);
        EventManager.Instance.UnloadEventScene();
    }

    private IEnumerator OnEventStarted()
    {
        dialog.Hide();
        wheelManager.Hide();

        wheelManager.Build(foodSegments);

        float time = 0;

        while (time < 1.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        dialog.Reveal();
        dialog.NextText(OnTreeAnimation, "You found food !");
    }

    private void OnTreeAnimation()
    {
        dialog.Hide();

        moovingTree.SetOnAnimationFinish(() =>
        {
            StartCoroutine(OnWheelStartSpinning());
        });
        moovingTree.StartAnimation();
    }

    private IEnumerator OnWheelStartSpinning()
    {
        wheelManager.Reveal();

        float time = 0;

        while (time < 1.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        wheelManager.OnSpinFinish(() =>
        {
            StartCoroutine(OnWheelEndSpinning());
        });
        result = wheelManager.Speen();
    }

    private IEnumerator OnWheelEndSpinning()
    {
        float time = 0;

        while (time < 1.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        wheelManager.Hide();
        dialog.Reveal();
        dialog.NextText(EndEvent, "You won: " + result.name + " food !");
    }
}
