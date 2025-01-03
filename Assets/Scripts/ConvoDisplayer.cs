using UnityEngine;
using UnityEngine.SceneManagement;

public class ConvoDisplayer : MonoBehaviour
{
    public GameObject canvasToActivate; // Assign the Canvas in the Inspector
    public GameObject canvasToDeActivate;
    private bool isPlayerInTrigger = false;
    private Vector3 playerLastPosition;
    public GameObject player; // Reference to the player GameObject
    private Rigidbody2D playerRigidbody; // Optional: Reference to player's Rigidbody2D

    void Start()
    {
        if (canvasToActivate != null)
        {
            canvasToActivate.SetActive(false); // Ensure the canvas is initially disabled
            canvasToDeActivate.SetActive(true);
        }

        if (player != null)
        {
            playerRigidbody = player.GetComponent<Rigidbody2D>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            playerLastPosition = other.transform.position; // Save the player's position
            Debug.Log("Player position saved: " + playerLastPosition);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
        }
    }

    void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E)) // Enter key
        {
            if (canvasToActivate != null)
            {
                canvasToActivate.SetActive(true); // Activate the canvas
                canvasToDeActivate.SetActive(false);
            }
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
                playerRigidbody.velocity = Vector2.zero; // Reset velocity if Rigidbody2D exists
            }

            player.transform.position = playerLastPosition; // Restore the player's position
            Debug.Log("Player position restored: " + playerLastPosition);
        }
    }

    public void nextScene()
    {
        if (SceneManager.GetActiveScene().name != "TravelMap") // Prevent reloading the same scene
        {
            SceneManager.LoadSceneAsync("TravelMap");
        }
    }
}
