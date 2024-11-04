using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomControl : MonoBehaviour
{
    public float SmoothChange;
    public float[] ZoomLevels = new float[] { 5f, 10f, 30f }; // Array to store different zoom levels (e.g. 5, 10, 20)

    private Camera cam;
    public int currentZoomLevel; // Track the current zoom level index

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

        // Optional: Clamp the camera size if needed (but generally handled by zoom levels)
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, ZoomLevels[0], ZoomLevels[ZoomLevels.Length - 1]);
    }
}
