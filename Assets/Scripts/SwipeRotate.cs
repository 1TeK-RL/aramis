using UnityEngine;
using UnityEngine.InputSystem;

public class SwipeRotateY : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 0.2f;

    private SwipeInputActions input;
    private InputAction rotateAction;

    private void Awake()
    {
        input = new SwipeInputActions();
        rotateAction = input.Gameplay.Rotate;
    }

    private void OnEnable()
    {
        input.Gameplay.Enable();
    }

    private void OnDisable()
    {
        input.Gameplay.Disable();
    }

    private void Update()
    {
        Vector2 delta = rotateAction.ReadValue<Vector2>();

        if (Mathf.Abs(delta.x) > 0.01f)
        {
            transform.Rotate(0f, -delta.x * rotationSpeed, 0f, Space.World);
        }
    }
}