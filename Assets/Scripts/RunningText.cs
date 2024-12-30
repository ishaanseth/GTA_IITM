using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SimultaneousScrollingTextWithClick : MonoBehaviour
{
    [Header("Text Settings")]
    [TextArea(5, 10)]
    public string[] lines; // Array of lines to display
    public float delayBetweenLetters = 0.05f; // Delay between each letter

    [Header("UI References")]
    public TextMeshProUGUI textDisplay; // TextMeshProUGUI component inside the ScrollRect
    public ScrollRect scrollRect; // ScrollRect component for scrolling

    private int currentLineIndex = 0; // Tracks the current line
    private bool isAnimating = false; // Prevent multiple inputs during animation

    private void Start()
    {
        // Ensure required components are assigned
        if (textDisplay == null || scrollRect == null)
        {
            Debug.LogError("TextDisplay or ScrollRect not assigned!");
            return;
        }

        // Start displaying the first line
        DisplayNextLine();
    }

    private void Update()
    {
        // Check for Enter key press or mouse click (except on the scrollbar)
        if (!isAnimating && (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)))
        {
            if (!IsPointerOverScrollbar())
            {
                DisplayNextLine();
            }
        }
    }

    private void DisplayNextLine()
    {
        // Check if there are more lines to display
        if (currentLineIndex < lines.Length)
        {
            StartCoroutine(DisplayTextLetterByLetter(lines[currentLineIndex]));
            currentLineIndex++;
        }
        else
        {
            // Clear text if no more lines
            textDisplay.text = "";
            UpdateContentSizeAndScroll(); // Ensure scrolling resets properly
        }
    }

    private IEnumerator DisplayTextLetterByLetter(string line)
    {
        isAnimating = true;

        // Clear the current text
        textDisplay.text = "";

        // Loop through each character and display it
        foreach (char letter in line)
        {
            textDisplay.text += letter; // Add the letter to the text
            UpdateContentSizeAndScroll(); // Adjust content size and scroll dynamically
            yield return new WaitForSeconds(delayBetweenLetters); // Wait before showing the next letter
        }

        isAnimating = false; // Animation complete
    }

    private void UpdateContentSizeAndScroll()
    {
        // Update the size of the Content based on its text
        textDisplay.ForceMeshUpdate();
        float textHeight = textDisplay.preferredHeight;

        RectTransform contentRect = textDisplay.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, textHeight);

        // Ensure the ScrollRect scrolls dynamically to show the latest part
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f; // Set to bottom to simulate real-time scrolling
    }

    private bool IsPointerOverScrollbar()
    {
        // Check if the mouse is over a UI element
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            // Check if the pointer is over the scrollbar
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);

            foreach (var result in results)
            {
                if (result.gameObject.GetComponent<Scrollbar>() != null)
                {
                    return true; // Pointer is over a scrollbar
                }
            }
        }
        return false; // Pointer is not over a scrollbar
    }
}