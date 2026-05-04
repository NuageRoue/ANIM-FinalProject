using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements.Experimental;

public class CallEventDemoScene2 : CallEventDemo
{
    [SerializeField]
    float totalTime = 10;
    float currentTime = 0;

    [SerializeField]
    TextMeshProUGUI text;

    public override void StartEvent(EventContextDemo context)
    {
        currentTime = 0;

        text.SetText(string.Format("Current amount {0}", context.value));
    }

    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= totalTime)
        {
            EndEvent(new EventResultDemo { add = 0, sub = 1 });
        }
    }
}
