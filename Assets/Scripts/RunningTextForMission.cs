using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class RunningTextForMission : MonoBehaviour
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

    // Event to notify when all lines are displayed
    public System.Action OnAllLinesDisplayed;

    private void Start()
    {
        if (textDisplay == null || scrollRect == null)
        {
            Debug.LogError("TextDisplay or ScrollRect not assigned!");
            return;
        }

        DisplayNextLine();
    }

    private void Update()
    {
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
        if (currentLineIndex < lines.Length)
        {
            StartCoroutine(DisplayTextLetterByLetter(lines[currentLineIndex]));
            currentLineIndex++;
        }
        else
        {
            // Notify when all lines are displayed
            OnAllLinesDisplayed?.Invoke();
        }
    }

    private IEnumerator DisplayTextLetterByLetter(string line)
    {
        isAnimating = true;

        textDisplay.text = "";

        foreach (char letter in line)
        {
            textDisplay.text += letter;
            UpdateContentSizeAndScroll();
            yield return new WaitForSeconds(delayBetweenLetters);
        }

        isAnimating = false;
    }

    private void UpdateContentSizeAndScroll()
    {
        textDisplay.ForceMeshUpdate();
        float textHeight = textDisplay.preferredHeight;

        RectTransform contentRect = textDisplay.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, textHeight);

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    private bool IsPointerOverScrollbar()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);

            foreach (var result in results)
            {
                if (result.gameObject.GetComponent<Scrollbar>() != null)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
