using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _baseSpeed = 5f; // Minimum speed

    [SerializeField]
    private float _maxSpeed = 25f; // Maximum speed

    [SerializeField]
    private float _speedChangeRate = 2f; // Rate of speed change when pressing or holding shift/space

    private Rigidbody2D _rigidbody;
    private Vector2 _movementInput;
    private Vector2 _smoothedMovementInput;
    private Vector2 _movementInputSmoothVelocity;

    public float playerVelocity;

    public CoinManager cm;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        playerVelocity = _baseSpeed; // Initialize player speed to base speed
    }

    private void Start()
    {
        PlayerPrefs.DeleteAll();
    }
    private void Update()
    {
        HandleSpeedChange();
    }

    private void FixedUpdate()
    {
        SetPlayerVelocity();
        RotateInDirectionOfInput();
    }

    private void HandleSpeedChange()
    {
        // Increase speed when Shift is pressed or held
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            playerVelocity += _speedChangeRate * Time.deltaTime;
            playerVelocity = Mathf.Clamp(playerVelocity, _baseSpeed, _maxSpeed);
        }

        // Decrease speed when Space is pressed or held
        if (Input.GetKey(KeyCode.Space))
        {
            playerVelocity -= _speedChangeRate * Time.deltaTime;
            playerVelocity = Mathf.Clamp(playerVelocity, _baseSpeed, _maxSpeed);
        }
    }

    private void SetPlayerVelocity()
    {
        _smoothedMovementInput = Vector2.SmoothDamp(
                    _smoothedMovementInput,
                    _movementInput,
                    ref _movementInputSmoothVelocity,
                    0.1f);

        _rigidbody.velocity = _smoothedMovementInput * playerVelocity;
    }

    private void RotateInDirectionOfInput()
    {
        if (_movementInput != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward, _smoothedMovementInput);
            Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 1000f * Time.deltaTime);

            _rigidbody.MoveRotation(rotation);
        }
    }

    private void OnMove(InputValue inputValue)
    {
        _movementInput = inputValue.Get<Vector2>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            cm.coinCount++;
        }
    }
}
