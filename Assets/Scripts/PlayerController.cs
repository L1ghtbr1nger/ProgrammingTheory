using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerEntity))]
public class PlayerController : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    [SerializeField] private PlayerEntity entity;
    [SerializeField] private PlayerMotor motor;

    private PlayerInput playerInput;
    private InputSystem_Actions input;

    private void Awake()
    {
        if (entity == null) entity = GetComponent<PlayerEntity>();
        if (motor == null) motor = GetComponent<PlayerMotor>();
        playerInput = GetComponent<PlayerInput>();

        input = new InputSystem_Actions();
        if (playerInput != null)
            playerInput.actions = input.asset; // keep pairing / control schemes

        Debug.Log($"PlayerController init. Entity={(entity?._GetTypeName())}, Motor={(motor?._GetTypeName())}");
    }

    private void OnEnable()
    {
        input.Player.SetCallbacks(this);
        input.Player.Enable();
        Debug.Log("Input Player map enabled.");
    }

    private void OnDisable()
    {
        input.Player.RemoveCallbacks(this);
        input.Player.Disable();
    }

    // Movement/look
    public void OnMove(InputAction.CallbackContext ctx) { motor?.SetMoveInput(ctx.ReadValue<Vector2>()); }
    public void OnLook(InputAction.CallbackContext ctx) { motor?.SetLookInput(ctx.ReadValue<Vector2>()); }
    public void OnSprint(InputAction.CallbackContext ctx)
    {
        bool pressed = ctx.ReadValueAsButton();
        if (ctx.canceled) pressed = false;
        motor?.SetSprint(pressed);
    }
    public void OnJump(InputAction.CallbackContext ctx) { if (ctx.performed) motor?.Jump(); }

    // Gameplay commands
    public void OnWieldLeft(InputAction.CallbackContext ctx)   { if (ctx.performed) entity?.WieldLeftCommand(); }
    public void OnWieldRight(InputAction.CallbackContext ctx)  { if (ctx.performed) entity?.WieldRightCommand(); }
    public void OnAbilityLeft(InputAction.CallbackContext ctx) { if (ctx.performed) entity?.AbilityLeftCommand(); }
    public void OnAbilityRight(InputAction.CallbackContext ctx){ if (ctx.performed) entity?.AbilityRightCommand(); }
    public void OnSwitchMode(InputAction.CallbackContext ctx)  { if (ctx.performed) entity?.SwitchModeCommand(); }
    public void OnInteract(InputAction.CallbackContext ctx)
    {
        Debug.Log($"OnInteract phase={ctx.phase}, performed={ctx.performed}, started={ctx.started}, canceled={ctx.canceled}");
        if (ctx.performed)
        {
            Debug.Log("OnInteract performed");
            entity?.InteractCommand();
        }
    }
    public void OnCrouch(InputAction.CallbackContext ctx) { }
    public void OnSwitchHands(InputAction.CallbackContext ctx) { if (ctx.performed) entity?.SwitchHandsCommand(); }
    public void OnPrevious(InputAction.CallbackContext ctx) { /* No-op or implement as needed */ }
    public void OnNext(InputAction.CallbackContext ctx) { /* No-op or implement as needed */ }
}

static class DebugExt
{
    public static string _GetTypeName(this Object o) => o == null ? "null" : o.GetType().Name;
}
