using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class SimCam : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float zoomSpeed = 120f;
    [SerializeField] private float minFieldOfView = 20f;
    [SerializeField] private float maxFieldOfView = 90f;

    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        Move();
        Zoom();
    }

    private void Move()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        float xInput = 0f;
        float yInput = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            xInput -= 1f;
        }

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            xInput += 1f;
        }

        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
        {
            yInput -= 1f;
        }

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
        {
            yInput += 1f;
        }

        var movement = new Vector3(xInput, yInput, 0f).normalized;
        transform.position += movement * (moveSpeed * Time.deltaTime);
    }

    private void Zoom()
    {
        if (Mouse.current == null)
        {
            return;
        }

        float scrollInput = Mouse.current.scroll.ReadValue().y;
        if (Mathf.Approximately(scrollInput, 0f))
        {
            return;
        }

        _camera.fieldOfView -= scrollInput * zoomSpeed * 0.01f;
        _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView, minFieldOfView, maxFieldOfView);
    }
}
