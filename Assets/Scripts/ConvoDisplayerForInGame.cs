using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConvoDisplayerForInGame : MonoBehaviour
{
    public GameObject canvasToActivate; // Assign the Canvas in the Inspector
    public GameObject canvasToDeActivate;
    private bool isPlayerInTrigger = false;
    private Vector3 playerLastPosition;
    public GameObject player; // Reference to the player GameObject
    private Rigidbody2D playerRigidbody; // Optional: Reference to player's Rigidbody2D
    public GameObject dialogueBox;

    public GameObject MissionComplete;
    public GameObject MissionFailed;

    public InstiHospital InstiHospital;



    void Start()
    {
        if (canvasToActivate != null)
        {
            canvasToActivate.SetActive(false); // Ensure the canvas is initially disabled
            MissionComplete.SetActive(false);
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
                dialogueBox.SetActive(false);
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

    public void Return_Mission_Complete()
    {
        MissionComplete.SetActive(false);
        canvasToDeActivate.SetActive(true);

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

    public void Return_Mission_Failed()
    {
        MissionFailed.SetActive(false);
        canvasToDeActivate.SetActive(true);

        if (player != null)
        {
            if (playerRigidbody != null)
            {
                playerRigidbody.velocity = Vector2.zero; // Reset velocity if Rigidbody2D exists
            }

            player.transform.position = InstiHospital.playerLastPosition; // Restore the player's position
            Debug.Log("Player position restored: " + playerLastPosition);
        }
    }
}
