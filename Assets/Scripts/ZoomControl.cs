using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Required for TextMeshPro

public class ZoomControl : MonoBehaviour
{
    public float SmoothChange;
    public float[] ZoomLevels = new float[] { 5f, 10f, 30f }; // Array to store different zoom levels

    private Camera cam;
    public int currentZoomLevel; // Track the current zoom level index

    public UnityEngine.UI.Button button; // Reference to the button
    public float baseButtonSize = 100f; // Base size of the button at the default zoom level
    public float zoomRatioMultiplier = 1f; // Multiplier to adjust how the button scales with zoom

    public float baseFontSize = 14f; // Default font size for the button text

    private void Start()
    {
        cam = GetComponent<Camera>();
        currentZoomLevel = 0; // Start at the first zoom level (normal size)
    }

    private void Update()
    {
        // Toggle zoom levels on pressing "C"
        if (Input.GetKeyDown(KeyCode.C))
        {
            currentZoomLevel = (currentZoomLevel + 1) % ZoomLevels.Length; // Cycle through zoom levels
        }

        // Smoothly interpolate between current camera size and target zoom level
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, ZoomLevels[currentZoomLevel], SmoothChange * Time.deltaTime);

        // Adjust button size and text size based on the ratio of current zoom
        AdjustButtonSize(cam.orthographicSize);
    }

    private void AdjustButtonSize(float currentZoom)
{
    if (button == null) return;

    // Calculate the ratio of the current zoom level to the maximum zoom level
    float zoomRatio = currentZoom / ZoomLevels[ZoomLevels.Length - 1];

    // Calculate the button size based on the base size and the zoom ratio
    float buttonSize = baseButtonSize / (zoomRatio * zoomRatioMultiplier + 1);

    // Update the button's RectTransform size and maintain its bottom-left position
    RectTransform buttonRect = button.GetComponent<RectTransform>();

    // Store current bottom-left position in local coordinates
    Vector2 bottomLeftPosition = buttonRect.anchoredPosition;

    // Update sizeDelta for resizing the button
    buttonRect.sizeDelta = new Vector2(buttonSize, buttonSize);

    // Restore bottom-left position after resizing
    buttonRect.anchoredPosition = bottomLeftPosition;

    // Adjust the text size in the button
    TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
    if (buttonText != null)
    {
        float textSize = baseFontSize / (zoomRatio * zoomRatioMultiplier + 1); // Scale text size proportionally
        buttonText.fontSize = textSize;
    }
}

}
