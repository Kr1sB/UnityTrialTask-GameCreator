using UnityEngine;
using UnityEngine.InputSystem;


[DefaultExecutionOrder(-1)]
public class InputController : MonoBehaviour
{
    public static InputController instance { get; private set; }

    public delegate void TouchEvent(Vector2 position);

    public event TouchEvent onPrimaryTouchStart;
    public event TouchEvent onPrimaryTouchEnd;


    private TouchControls touchControls;

    private void Awake()
    {
        if (instance != null)
            throw new System.Exception("Multiple instances of " + GetType());

        instance = this;
        touchControls = new TouchControls();
    }

    private void OnEnable()
    {
        touchControls.Enable();
    }

    private void OnDisable()
    {
        touchControls.Disable();
    }

    private void Start()
    {
        touchControls.Touch.PrimaryTouchPress.started += ctx => TouchStarted(ctx);
        touchControls.Touch.PrimaryTouchPress.canceled += ctx => TouchEnded(ctx);

        touchControls.Touch.SecondaryTouchPress.started += ctx => TouchStarted(ctx);
        touchControls.Touch.SecondaryTouchPosition.canceled += ctx => TouchEnded(ctx);
    }

    private void TouchStarted(InputAction.CallbackContext context)
    {
        Vector2 p = touchControls.Touch.PrimaryTouchPosition.ReadValue<Vector2>();
        onPrimaryTouchStart?.Invoke(p);
    }

    private void TouchEnded(InputAction.CallbackContext context)
    {
        Vector2 p = touchControls.Touch.PrimaryTouchPosition.ReadValue<Vector2>();
        onPrimaryTouchEnd?.Invoke(p);
    }
}
