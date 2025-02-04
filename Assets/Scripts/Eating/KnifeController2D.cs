using UnityEngine;

public class KnifeController2D : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButton(0)) // Check if left mouse button is held down
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // Set z position to 0 since it's a 2D game
            transform.position = mousePosition; // Move knife to mouse position
        }
    }
}
