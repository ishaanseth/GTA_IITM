using UnityEngine;
using UnityEngine.SceneManagement;

public class BrushMovement : MonoBehaviour
{
    public Vector3 positionA; // Point A, set in the Inspector
    public Vector3 positionB; // Point B, set in the Inspector
    public Vector3 positionC; // Point C, set in the Inspector
    public Vector3 positionD; // Point D, set in the Inspector
    public float acceleration = 5f; // Rate of acceleration when space is pressed
    public float deceleration = 5f; // Rate of deceleration when space is not pressed
    public float maxSpeed = 10f; // Maximum speed
    public float returnSpeed = 2f; // Speed of returning to positionA
    public float timer = 10f; // Timer duration in seconds

    public GameObject brushingTimer; // Reference to the GameObject whose size reduces
    private Vector3 initialScale; // Initial scale of the brushingTimer

    private float currentSpeed = 0f;
    private int currentPointIndex = 0; // Tracks the current point in the sequence
    private Vector3[] points; // Array to store the sequence of points
    private bool reverse = false; // Tracks if the object is reversing direction
    private bool isTimerActive = false; // Tracks if the timer is active
    public RectTransform rectTransform;


    void Start()
    {
        // Initialize the sequence of points in abcdcba order
        points = new Vector3[] { positionA, positionB, positionC, positionD, positionC, positionB, positionA };

        if (brushingTimer != null)
        {
            initialScale = brushingTimer.transform.localScale; // Store the initial scale of the brushingTimer

            if (rectTransform != null)
            {
                rectTransform.anchorMin = new Vector2(0, 0.5f); // Anchor to the left-center
                rectTransform.anchorMax = new Vector2(0, 0.5f); // Anchor to the left-center
                rectTransform.pivot = new Vector2(0, 0.5f); // Pivot point at the left-center
            }
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Accelerate();
            UpdateTimer();
        }
        else
        {
            Decelerate();
            isTimerActive = false; // Pause the timer when not accelerating
        }

        MoveObject();
        UpdateBrushingTimerScale();

        if (timer <= 0)
        {
            UnloadCurrentScene();
        }
    }

    void Accelerate()
    {
        // Increase speed up to maxSpeed
        currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);
        isTimerActive = true; // Activate the timer
    }

    void Decelerate()
    {
        // Decrease speed to 0
        currentSpeed = Mathf.Max(currentSpeed - deceleration * Time.deltaTime, 0f);
    }

    void MoveObject()
    {
        if (currentSpeed > 0)
        {
            // Calculate direction towards the next point
            Vector3 targetPoint = points[currentPointIndex];
            Vector3 direction = (targetPoint - transform.position).normalized;

            // Move the object based on current speed
            transform.position += direction * currentSpeed * Time.deltaTime;

            // Check if it has reached the target point
            if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
            {
                transform.position = targetPoint; // Snap to the target point

                // Update the next point index based on the sequence
                if (!reverse)
                {
                    currentPointIndex++;
                    if (currentPointIndex >= points.Length - 1)
                    {
                        reverse = true; // Start reversing the sequence
                    }
                }
                else
                {
                    currentPointIndex--;
                    if (currentPointIndex <= 0)
                    {
                        reverse = false; // Start moving forward in the sequence again
                    }
                }
            }
        }
    }

    void UpdateTimer()
    {
        if (isTimerActive && timer > 0)
        {
            timer -= Time.deltaTime;
            timer = Mathf.Max(timer, 0f); // Ensure timer does not go below zero
        }
    }

    void UpdateBrushingTimerScale()
    {
        if (brushingTimer != null && timer > 0)
        {
            // Calculate the new scale based on the timer value
            float scaleRatio = timer / 10f; // Assuming the timer starts at 10 seconds
            Vector3 newScale = new Vector3(initialScale.x * scaleRatio, initialScale.y, initialScale.z);

            // Calculate the offset to keep the left side fixed
            float scaleDifference = newScale.x - brushingTimer.transform.localScale.x;
            brushingTimer.transform.position += new Vector3(scaleDifference / 2f, 0f, 0f);

            // Apply the new scale
            brushingTimer.transform.localScale = newScale;
        }
    }

    void UnloadCurrentScene()
    {
        PlayerPrefs.SetInt("BrushingDone", 1);
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.UnloadSceneAsync("Brushing Scene");
    }
}
