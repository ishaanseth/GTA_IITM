using UnityEngine;
using TMPro; // Use this if you are using TextMeshPro; otherwise, use UnityEngine.UI for standard Text.

public class GameManager : MonoBehaviour
{
    public PlayerMovement playerMovement; // Reference to the PlayerMovement script
    public ZoomControl zoomControl; // Reference to the ZoomControl script

    public TextMeshProUGUI speedText; // UI element to display player speed (TextMeshPro)
    public TextMeshProUGUI zoomText; // UI element to display zoom level (TextMeshPro)

    // If you're using standard UI Text, use this instead:
    // public Text speedText;
    // public Text zoomText;

    private void Update()
    {
        // Update speed and zoom level in the UI
        UpdateSpeedUI();
        UpdateZoomUI();
    }

    private void UpdateSpeedUI()
    {
        // Get the player's current speed and update the UI text
        speedText.text = "Speed: " + playerMovement.playerVelocity.ToString("F1") + "x";
    }

    private void UpdateZoomUI()
    {
        // Get the current zoom level from the ZoomControl script and update the UI
        float currentZoom = zoomControl.ZoomLevels[zoomControl.currentZoomLevel]; // Access current zoom level
        zoomText.text = "Zoom Level: " + currentZoom.ToString("F1");
    }
}
