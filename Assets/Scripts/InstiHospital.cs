using UnityEngine;
using TMPro;

public class InstiHospital : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject
    public GameObject canvasToActivate;
    public GameObject canvasToDeActivate;
    public GameObject dialogueBox;
    public Transform instiHospital;
    public GameObject returnButton; // Reference to the "Return" button
    public RunningTextForMission runningText; // Reference to the RunningTextForMission script
    public TextMeshProUGUI timerText; // TextMeshProUGUI to display the timer
    public GameObject health; // The health bar GameObject whose size decreases with time
    public GameObject missionCompleteCanvas; // Reference to the "MissionComplete" canvas
    public GameObject missionFailedCanvas; // Reference to the "MissionFailed" canvas
    public GameObject dropOff;

    private bool isInside = false;
    public CoinManager cm;
    private bool isAttached = false;
    private bool isPlayerInTrigger = false;
    public Vector3 playerLastPosition;
    private Vector3 originalPosition; // Store the original position of the object
    private Quaternion originalRotation; // Store the original rotation of the object
    private Rigidbody2D playerRigidbody;

    public float timer = 15f; // Timer countdown
    private Vector3 initialHealthScale; // Initial scale of the health bar

    void Start()
    {
        if (canvasToActivate != null)
        {
            canvasToActivate.SetActive(false);
            canvasToDeActivate.SetActive(true);
        }

        if (returnButton != null)
        {
            returnButton.SetActive(false); // Ensure "Return" button is hidden initially
        }

        if (dialogueBox != null)
        {
            dialogueBox.SetActive(true); // Ensure dialogueBox is shown initially
        }

        if (missionCompleteCanvas != null)
        {
            missionCompleteCanvas.SetActive(false); // Ensure MissionComplete canvas is hidden initially
        }

        if (missionFailedCanvas != null)
        {
            missionFailedCanvas.SetActive(false); // Ensure MissionFailed canvas is hidden initially
        }

        if (runningText != null)
        {
            runningText.OnAllLinesDisplayed += HandleAllLinesDisplayed;
        }

        if (player != null)
        {
            playerRigidbody = player.GetComponent<Rigidbody2D>();
        }

        if (dropOff != null)
        {
            dropOff.SetActive(false);
        }

        // Store the original position and rotation of the object
        originalPosition = this.transform.position;
        originalRotation = this.transform.rotation;

        // Show InstiHospital initially
        this.gameObject.SetActive(true);

        // Initialize health scale
        if (health != null)
        {
            initialHealthScale = health.transform.localScale;
        }

        // Initialize timer display
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false); // Ensure timerText is hidden initially
            timerText.text = $"Timer: {timer:F1}s";
        }
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogError("Player is not assigned.");
            return;
        }

        // Attach this object to the player when Enter key is pressed
        if (Input.GetKeyDown(KeyCode.Return) && !isAttached && isInside && !canvasToActivate.activeSelf)
        {
            AttachToPlayer();
        }

        // Update the timer and health size if attached
        if (isAttached)
        {
            UpdateTimerAndHealth();
        }

        // Check if player reaches InstiHospital
        if (isAttached && Vector3.Distance(player.transform.position, instiHospital.position) < 0.5f)
        {
            ShowMissionComplete();
        }

        // Handle canvas activation
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            if (canvasToActivate != null)
            {
                canvasToActivate.SetActive(true);
                canvasToDeActivate.SetActive(false);
                if (dialogueBox != null)
                {
                    dialogueBox.SetActive(false);
                }
            }
        }
    }

    private void AttachToPlayer()
    {
        dropOff.SetActive(true);
        // Make this GameObject visible and attach it to the player
        this.gameObject.SetActive(true);
        dialogueBox.SetActive(false);
        this.transform.SetParent(player.transform);
        this.transform.localPosition = new Vector3(0, -1, 0); // Adjust position relative to the player
        this.transform.localRotation = Quaternion.identity; // Reset rotation relative to the player
        isAttached = true;

        // Show the timerText
        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
        }

        Debug.Log($"{this.gameObject.name} successfully attached to the player.");
    }

    private void DetachFromPlayer()
    {
        // Detach this GameObject from the player
        this.transform.SetParent(null);
        isAttached = false;

        // Move the GameObject back to its original position and rotation
        this.transform.position = originalPosition;
        this.transform.rotation = originalRotation;

        // Hide the timerText
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }

        // Activate the dialogueBox
        if (dialogueBox != null)
        {
            dialogueBox.SetActive(true);
        }

        // Reset the timer
        ResetTimer();

        Debug.Log($"{this.gameObject.name} detached from the player, reset to original state, and timer reset.");
    }

    private void UpdateTimerAndHealth()
    {
        // Reduce the timer
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            timer = Mathf.Max(timer, 0f); // Ensure timer doesn't go below 0

            // Update the timer display
            if (timerText != null)
            {
                timerText.text = $"Timer: {timer:F1}s";
            }

            // Reduce the health bar size proportionally
            if (health != null)
            {
                float healthScaleFactor = timer / 15f; // Proportion based on remaining time
                health.transform.localScale = new Vector3(
                    initialHealthScale.x * healthScaleFactor,
                    initialHealthScale.y,
                    initialHealthScale.z
                );
            }
        }
        else
        {
            // Timer has finished, show MissionFailed canvas
            ShowMissionFailed();
        }
    }

    private void ResetTimer()
    {
        // Reset the timer to 15 seconds
        timer = 15f;

        // Reset the health bar size to its original scale
        if (health != null)
        {
            health.transform.localScale = initialHealthScale;
        }

        // Update the timer display
        if (timerText != null)
        {
            timerText.text = $"{timer:F1}";
        }

        Debug.Log("Timer reset to 15 seconds and health restored.");
    }

    private void ShowMissionComplete()
    {
        Destroy(this);
        canvasToActivate.SetActive(false);
        dialogueBox.SetActive(false );
        dropOff.SetActive(false);
        // Activate the MissionComplete canvas
        if (missionCompleteCanvas != null)
        {
            missionCompleteCanvas.SetActive(true);
            canvasToDeActivate.SetActive(false);
        }

        cm.coinCount += 10;

        // Detach from player and end the mission
        DetachFromPlayer();

        Debug.Log("Mission completed! MissionComplete canvas activated.");
    }

    private void ShowMissionFailed()
    {
        dropOff.SetActive(false);
        // Activate the MissionFailed canvas
        if (missionFailedCanvas != null)
        {
            missionFailedCanvas.SetActive(true);
            canvasToDeActivate.SetActive(false);
        }

        // Detach from player and reset the mission
        DetachFromPlayer();

        Debug.Log("Mission failed! MissionFailed canvas activated.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            playerLastPosition = other.transform.position;
            Debug.Log("Player position saved 465456454: " + playerLastPosition);
            isInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            Debug.Log("Player exited the trigger area.");
            isInside = false;
        }
    }

    public void Return_()
    {
        if (canvasToActivate != null && canvasToDeActivate != null)
        {
            canvasToActivate.SetActive(false);
            canvasToDeActivate.SetActive(true);
        }

        if (player != null)
        {
            if (playerRigidbody != null)
            {
                playerRigidbody.velocity = Vector2.zero;
            }

            player.transform.position = playerLastPosition;
            Debug.Log("Player position restored: " + playerLastPosition);

            if (!isAttached)
            {
                AttachToPlayer();
            }
        }
    }

    private void HandleAllLinesDisplayed()
    {
        if (returnButton != null)
        {
            returnButton.SetActive(true); // Make the "Return" button visible
        }
        Debug.Log("All lines displayed. Return button is now visible.");
    }
}
