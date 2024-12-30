using UnityEngine;

public class ConvoDisplayer : MonoBehaviour
{
    public GameObject canvasToActivate; // Assign the Canvas in the Inspector
    public GameObject canvasToDeActivate;
    private bool isPlayerInTrigger = false;

    void Start()
    {
        if (canvasToActivate != null)
        {
            canvasToActivate.SetActive(false); // Ensure the canvas is initially disabled
            canvasToDeActivate.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
            Debug.Log("Entered");
            if (other.CompareTag("Player"))
            {
                isPlayerInTrigger = true;
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

    public void Return()
    {
        canvasToActivate.SetActive(false);
        canvasToDeActivate.SetActive(true);
    }
}
