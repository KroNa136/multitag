using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _cameraRoot;

    [Space]

    [SerializeField] private bool _invertYAxis = false;
    [SerializeField][Range(0.1f, 10f)] private float _sensitivity = 1f;

    private float _pitch;
    private float Pitch
    {
        get => _pitch;
        set => _pitch = Mathf.Clamp(value, -90f, 90f);
    }

    private float _yaw;
    private float Yaw
    {
        get => _yaw;
        set => _yaw = Mathf.Repeat(value, 360f);
    }

    public Vector2 Look => new(Pitch, Yaw);

    private InputManager _inputManager;

    public bool CanBeControlledByPlayer = true;

    private void Start()
    {
        _ = TryGetComponent(out _inputManager);

        Yaw = transform.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;

        GameManager.OnClientGameOver.AddListener(OnGameOver);
    }

    private void OnGameOver(int _)
    {
        CanBeControlledByPlayer = false;
    }

    private void Update()
    {
        if (!CanBeControlledByPlayer || _inputManager == null)
            return;

        float lookX = _inputManager.LookX * _sensitivity;
        float lookY = _inputManager.LookY * _sensitivity;

        Pitch += lookY * (_invertYAxis ? 1f : -1f);
        Yaw += lookX;

        Rotate();
    }

    public void SetLook(Vector2 newLook)
    {
        Pitch = newLook.x;
        Yaw = newLook.y;

        Rotate();
    }

    private void Rotate()
    {
        _cameraRoot.localRotation = Quaternion.Euler(Pitch, 0f, 0f);
        transform.rotation = Quaternion.Euler(0, Yaw, 0f);
    }
}
