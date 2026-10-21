using UnityEngine;

public class MovePlayer : MonoBehaviour
{   

    [SerializeField] private float _speed;
    [SerializeField] private float _minView;
    [SerializeField] private float _maxView;
    [SerializeField] private float _gravity;

    private CharacterController _player;    
    private float _verticalVelosity;
    private float _gorizontalVelosity;
    
    private Transform _camera;
    private float _cameraRotationX;

    private void Start()
    {
        _player = GetComponent<CharacterController>();
        _camera = GetComponentInChildren<Camera>().transform;

        Cursor.lockState = CursorLockMode.Locked;
    }
    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Move();
        Rotation();
    }

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = transform.right * horizontal + transform.forward * vertical;

        direction = Vector3.ClampMagnitude(direction, 1f);

        if (_player.isGrounded && _verticalVelosity < 0f)
            _verticalVelosity = -2f;

        _verticalVelosity += _gravity * Time.deltaTime;

        Vector3 movement = direction * _speed;

        movement.y = _verticalVelosity;

        _player.Move(movement * Time.deltaTime);
    }

    private void Rotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        transform.Rotate(Vector3.up * mouseX);

        _cameraRotationX -= mouseY;

        _cameraRotationX = Mathf.Clamp(_cameraRotationX, _minView, _maxView);

        _camera.localRotation = Quaternion.Euler(_cameraRotationX, 0f, 0f);
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}
