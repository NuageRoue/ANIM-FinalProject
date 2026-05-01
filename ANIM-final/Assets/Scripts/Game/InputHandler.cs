using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputHandler : MonoBehaviour
{
    public event Action<Vector2> OnNavigate;
    public event Action OnConfirm;
    public event Action OnCancel;
    public event Action OnSpecialAction;

    private void OnNavigatePerformed(InputValue value)
    {
        OnNavigate?.Invoke(value.Get<Vector2>());
    }

    private void OnConfirmPerformed()
    {
        OnConfirm?.Invoke();
    }

    private void OnCancelPerformed()
    {
        OnCancel?.Invoke();
    }

    private void OnSpecialActionPerformed()
    {
        OnSpecialAction?.Invoke();
    }
}
