using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private float _rotationSpeed = 1000f;

    private float[] _speedMultipliers = { 1f, 2f, 4f }; // Multipliers for speed (1x, 2x, 4x)
    private int _currentSpeedIndex = 0; // To track which speed we're on


    private Rigidbody2D _rigidbody;
    private Vector2 _movementInput;
    private Vector2 _smoothedMovementInput;
    private Vector2 _movementInputSmoothVelocity;

    public float playerVelocity;

    public CoinManager cm;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Toggle speed multipliers on pressing "Space"
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _currentSpeedIndex = (_currentSpeedIndex + 1) % _speedMultipliers.Length;
        }
    }

    private void FixedUpdate()
    {
        SetPlayerVelocity();
        RotateInDirectionOfInput();
    }

    private void SetPlayerVelocity()
    {
        _smoothedMovementInput = Vector2.SmoothDamp(
                    _smoothedMovementInput,
                    _movementInput,
                    ref _movementInputSmoothVelocity,
                    0.1f);
        
        // Set the player velocity to base speed multiplied by the current speed multiplier
        playerVelocity = _speed * _speedMultipliers[_currentSpeedIndex];

        _rigidbody.velocity = _smoothedMovementInput * playerVelocity;
    }

    private void RotateInDirectionOfInput()
    {
        if (_movementInput != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward, _smoothedMovementInput);
            Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

            _rigidbody.MoveRotation(rotation);
        }
    }

    private void OnMove(InputValue inputValue)
    {
        _movementInput = inputValue.Get<Vector2>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            cm.coinCount++;
        }
    }
}
