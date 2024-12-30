using System.Collections;
using UnityEngine;
using TMPro; // Required for TextMeshPro support

public class DotsAnimation : MonoBehaviour
{
    public TextMeshProUGUI displayText; // Reference to the TextMeshProUGUI component
    public float interval = 0.5f; // Time interval between dot updates

    public string baseText = "Loading"; // Base text to display
    private int dotCount = 0; // Keeps track of the number of dots
    public char character = '.';

    private void Start()
    {
        if (displayText == null)
        {
            Debug.LogError("Display Text is not assigned!");
            return;
        }
        StartCoroutine(AnimateDots());
    }

    private IEnumerator AnimateDots()
    {
        while (true)
        {
            dotCount = (dotCount + 1) % 4; // Cycle through 0, 1, 2, 3
            displayText.text = baseText + new string(character, dotCount); // Append the dots to the base text
            yield return new WaitForSeconds(interval);
        }
    }
}
