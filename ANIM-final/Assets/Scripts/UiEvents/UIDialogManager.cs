using UnityEngine;
using UnityEngine.Events;

public class UITextManagerDemo : MonoBehaviour
{
    [SerializeField]
    UIDialog uiDialog;

    void Start()
    {
        uiDialog.NextText(Call1, "Current: Start");
    }

    public void Call1()
    {
        uiDialog.NextText(Call2, "Current: Call1");
    }

    public void Call2()
    {
        uiDialog.NextText(Call3, "Current: Call2");
    }

    public void Call3()
    {
        uiDialog.NextText(Call1, "Current: Call3");
    }

    public void Log()
    {
        Debug.Log("ici");
    }
}
