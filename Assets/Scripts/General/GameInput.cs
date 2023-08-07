using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance;

    private PlayerInput playerInput;

    private void Awake()
    {
        Instance = this;

        playerInput = new PlayerInput();

        playerInput.Player.Enable();

        playerInput.Player.Interact.performed += Interact_performed;
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("click");
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInput.Player.Movement.ReadValue<Vector2>();

        inputVector = inputVector.normalized;
        return inputVector;
    }

    public float GetRotationFloat()
    {
        float rotionFloat = playerInput.Player.Rotate.ReadValue<float>();
        return rotionFloat;
    }

    public float GetZoom()
    {
        return playerInput.Player.Zoom.ReadValue<Vector2>().y;
    }
}
