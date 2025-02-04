using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeToBrushing : MonoBehaviour
{
    public GameObject glowEffect; // Assign a glowing green effect to this
    public GameObject dialogueBox; // Assign the dialogue box GameObject here
    public GameObject brushingGame; // Assign the brushing game GameObject here
    public GameObject Eating;
    public GameObject Himalyas;

    private bool playerInRange = false;
    private bool brushingCompleted = false;

    public int brushingDone = 0;

    void Start()
    {
        // Make sure the glow effect is active initially
        glowEffect.SetActive(true);

        // Ensure glowEffect has a 2D Collider
        if (!glowEffect.TryGetComponent<Collider2D>(out var glowCollider))
        {
            Debug.LogError("GlowEffect does not have a 2D Collider component!");
        }
        else
        {
            glowCollider.isTrigger = true;
        }
    }

    void Update()
    {
        brushingDone = PlayerPrefs.GetInt("BrushingDone");
        Debug.Log(playerInRange);
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!brushingCompleted)
            {
                // Deactivate dialogue box and activate brushing game
                dialogueBox.SetActive(false);
                brushingGame.SetActive(true);
            }
            else
            {
                // Load the next scene additively if brushing is completed
                SceneManager.LoadScene("Brushing Scene", LoadSceneMode.Additive);
            }
        }

        if (brushingDone != 0)
        {
            Eating.SetActive(true);
            dialogueBox.SetActive(false );
            glowEffect.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Entered trigger with: {other.name}");
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    // This function can be called by another script when the brushing game is completed
    public void BrushingGameCompleted()
    {
        brushingGame.SetActive(false);
        brushingCompleted = true;
    }

    public void EatingSet()
    {
        Eating.SetActive(false);
        Himalyas.SetActive(true);
    }
}
