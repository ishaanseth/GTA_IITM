using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeToEating : MonoBehaviour
{
    public GameObject glowEffect; // Assign a glowing green effect to this

    private bool playerInRange = false;
    public int eating = 0;

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
        eating = PlayerPrefs.GetInt("Eating");
        Debug.Log(playerInRange);
        if (playerInRange && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return)))
        {
            Debug.Log("Scene Name");
            SceneManager.LoadScene("Eating", LoadSceneMode.Additive);
        }

        if (eating != 0)
        {
            this.gameObject.SetActive(false);
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
}
