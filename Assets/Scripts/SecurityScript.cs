using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SecurityScript : MonoBehaviour
{
    public Transform[] patrolPoints; // Points the enemy will patrol between
    public Transform adminBuilding; // Reference to the admin building
    public float patrolSpeed = 2f; // Speed of patrol
    public float chaseSpeed = 4f; // Speed of chase
    public float sightRange = 5f; // Distance within which the enemy can see the player
    public float catchZoneRange = 2f; // Catch zone range
    public float sightAngle = 90f; // Angle (in degrees) within which the enemy can detect the player
    public float playerSpeedThreshold = 2f; // Player speed threshold for triggering chase
    public float chaseDuration = 5f; // How long the enemy chases the player
    public TextMeshProUGUI chaseTimerText; // Reference to the TextMeshProUGUI component for displaying the timer
    public GameObject SecurityUI;
    public RectTransform slidingTimer; // Reference to the SlidingTimer image
    public GameObject Retry;
    public GameObject Next;

    private int currentPatrolIndex = 0;
    private Transform player;
    private Vector3 originalPosition;
    private float chaseTimer = 0f;
    private bool isChasing = false;
    private bool isDraggingPlayer = false;
    public CoinManager coinManager;

    public GameObject caughtCanvas;
    public GameObject InGame;

    private void Start()
    {
        if (patrolPoints.Length > 0)
        {
            transform.position = patrolPoints[0].position;
        }
        originalPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (chaseTimerText != null)
        {
            chaseTimerText.enabled = false; // Initially deactivate the timer display
        }

        if (SecurityUI != null)
        {
            SecurityUI.SetActive(false);
        }

        if (slidingTimer != null)
        {
            slidingTimer.gameObject.SetActive(false); // Initially deactivate the sliding timer
        }

        Retry.SetActive(false);
        Next.SetActive(true);
    }

    private void Update()
    {
        if (isDraggingPlayer)
        {
            DragPlayerToAdminBuilding();
        }
        else if (isChasing)
        {
            ChasePlayer();
            DetectCatchZone();
        }
        else
        {
            Patrol();
            DetectPlayer();
        }
    }

    private void Patrol()
    {
        caughtCanvas.SetActive(false);
        InGame.SetActive(true);
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    private void DetectPlayer()
    {
        if (player == null) return;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float angleToPlayer = Vector3.Angle(transform.right, directionToPlayer);

        if (distanceToPlayer <= sightRange && angleToPlayer <= sightAngle / 2)
        {
            float playerSpeed = player.GetComponent<Rigidbody2D>().velocity.magnitude;

            if (playerSpeed > playerSpeedThreshold)
            {
                isChasing = true;
                chaseTimer = chaseDuration;

                if (chaseTimerText != null)
                {
                    chaseTimerText.enabled = true; // Activate the timer display
                    chaseTimerText.text = chaseTimer.ToString("F1"); // Display the initial timer
                }

                if (SecurityUI != null)
                {
                    SecurityUI.SetActive(true);
                }

                if (slidingTimer != null)
                {
                    slidingTimer.gameObject.SetActive(true); // Activate the sliding timer
                    slidingTimer.localScale = new Vector3(1f, 1f, 1f); // Reset the size
                }
            }
        }
    }

    private void ChasePlayer()
    {
        if (player == null) return;

        chaseTimer -= Time.deltaTime;

        if (chaseTimerText != null)
        {
            chaseTimerText.text = chaseTimer > 0 ? chaseTimer.ToString("F1") : ""; // Update the timer
        }

        if (slidingTimer != null)
        {
            float scaleFactor = Mathf.Clamp01(chaseTimer / chaseDuration); // Ensure scale doesn't go below 0
            slidingTimer.localScale = new Vector3(scaleFactor, 1f, 1f); // Update the width only
        }

        if (chaseTimer > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
            DetectCatchZone(); // Check if player is in the catch zone
        }
        else
        {
            StopChase(); // Timer has expired; stop chasing
        }
    }

    private void DetectCatchZone()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= catchZoneRange && isChasing)
        {
            ActivateCatchZone(); // Only activate catch zone if the player is within range
        }
    }

    private void StopChase()
    {
        isChasing = false;

        if (chaseTimerText != null)
        {
            chaseTimerText.enabled = false; // Deactivate the timer
        }

        if (SecurityUI != null)
        {
            SecurityUI.SetActive(false);
        }

        if (slidingTimer != null)
        {
            slidingTimer.gameObject.SetActive(false); // Deactivate the sliding timer
        }
    }

    private void ActivateCatchZone()
    {
        isChasing = false;
        isDraggingPlayer = true;

        coinManager.coinCount -= 10;

        if (player.TryGetComponent(out PlayerMovement playerMovement))
        {
            playerMovement.enabled = false; // Disable player movement
        }

        if (chaseTimerText != null)
        {
            chaseTimerText.enabled = false; // Deactivate the timer
        }

        if (SecurityUI != null)
        {
            SecurityUI.SetActive(false);
        }

        if (slidingTimer != null)
        {
            slidingTimer.gameObject.SetActive(false); // Deactivate the sliding timer
        }
    }

    private void DragPlayerToAdminBuilding()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (adminBuilding == null || player == null)
        { 
            return; 
        }

        if (distanceToPlayer <= catchZoneRange)
        {
            // Move both the player and security guard to the admin building
            transform.position = Vector3.MoveTowards(transform.position, adminBuilding.position, patrolSpeed * Time.deltaTime);
            player.position = transform.position;

            if (Vector3.Distance(transform.position, adminBuilding.position) < 0.1f)
            {
                CompleteDragToAdminBuilding();
            }
        }

        
    }

    private void CompleteDragToAdminBuilding()
    {
        caughtCanvas.SetActive(true);
        InGame.SetActive(false);

        
        if (coinManager.coinCount < 0)
        {
            Retry.SetActive(true);
            Next.SetActive(false);
        }

    }

    private void OnDrawGizmosSelected()
    {
        // Draw sight range in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        // Draw catch zone in the editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, catchZoneRange);

        // Draw sight angle in the editor
        Vector3 leftBoundary = Quaternion.Euler(0, 0, -sightAngle / 2) * transform.right * sightRange;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, sightAngle / 2) * transform.right * sightRange;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }

    public void nextButton()
    {
        caughtCanvas.SetActive(false);
        InGame.SetActive(true);
        if (coinManager.coinCount < 0)
        {
            SceneManager.LoadSceneAsync("TravelMap");
        }

            isDraggingPlayer = false;

        if (player.TryGetComponent(out PlayerMovement playerMovement))
        {
            playerMovement.enabled = true; // Re-enable player movement
        }

        // Reset security guard to patrol state
        if (patrolPoints.Length > 0)
        {
            currentPatrolIndex = 0;
            transform.position = patrolPoints[currentPatrolIndex].position;
        }
    }
}
