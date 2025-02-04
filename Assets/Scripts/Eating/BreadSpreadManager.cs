using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Required for UI elements

public class BreadSpreadManager : MonoBehaviour
{
    public static BreadSpreadManager Instance; // Singleton instance for easy access
    public GameObject breadSlicePrefab; // Reference to the bread slice prefab
    public int totalSlices = 5; // Total number of slices
    private int currentSliceIndex = 0; // Current slice index
    public Text timerText; // UI Text element for displaying the timer
    public Text sliceCounterText; // UI Text element for displaying the number of slices done
    public Text JamPercentage; // UI Text element for displaying butter and jam percentages
    private float timeRemaining = 60f; // Set the timer duration (in seconds)
    public Text ButterPercentage;
    private int slicesDoneCounter = 0; // Counter for completed slices
    public int maxToComplete = 2;

    public GameObject missionPassed;
    public GameObject missionFailed;

    void Awake()
    {
        Instance = this; // Initialize singleton instance
    }

    void Start()
    {
        SpawnNextSlice();
        UpdateTimerText();
        UpdateSliceCounterText();
    }

    void Update()
    {
        // Update the timer
        if (currentSliceIndex < totalSlices)
        {
            timeRemaining -= Time.deltaTime; // Decrease remaining time
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                EndGame(); // Call end game function if time runs out
            }
            UpdateTimerText(); // Update the displayed timer
        }

        HandleInput(); // Handle key presses for butter and jam
    }

    private void HandleInput()
    {
        if (currentSliceIndex < totalSlices)
        {
            BreadSlice2D currentSlice = FindObjectOfType<BreadSlice2D>();
            if (currentSlice != null)
            {
                if (Input.GetKeyDown(KeyCode.B)) // Press B to increase butter percentage
                {
                    currentSlice.IncreaseButter(5); // Increase butter by 25%
                }

                if (Input.GetKeyDown(KeyCode.J)) // Press J to increase jam percentage
                {
                    currentSlice.IncreaseJam(5); // Increase jam by 25%
                }
            }
        }
    }

    public void IncrementSliceCounter()
    {
        slicesDoneCounter++;
        UpdateSliceCounterText(); // Update UI text for slices done
        if (slicesDoneCounter >= totalSlices)
        {
            EndGame(); // End game when all slices are done
        }
    }

    private void SpawnNextSlice()
    {
        if (currentSliceIndex < totalSlices)
        {
            Instantiate(breadSlicePrefab, new Vector3(0, currentSliceIndex * -1.5f, 0), Quaternion.identity);
            currentSliceIndex++;
        }
        else
        {
            Debug.Log("All slices have been spread!");
            EndGame(); // Call end game function when all slices are spread
        }
    }

    private void UpdateTimerText()
    {
        timerText.text = "Time Left: " + Mathf.Ceil(timeRemaining).ToString() + "s"; // Display remaining time
    }

    private void UpdateSliceCounterText()
    {
        sliceCounterText.text = "Slices Done: " + slicesDoneCounter.ToString(); // Display completed slices count
    }

    private void EndGame()
    {
        Debug.Log("Game Over!"); // You can also show a UI panel here.
        //Time.timeScale = 0; // Pause the game (optional)
        // Optionally reset game or show end screen here
        if (slicesDoneCounter >= maxToComplete)
        {
            missionPassed.SetActive(true);
            missionFailed.SetActive(false);
        }
        else
        {
            missionPassed.SetActive(false);
            missionFailed.SetActive(true);
        }
    }

    public void redo()
    {
        SceneManager.UnloadSceneAsync("Eating").completed += (AsyncOperation op) =>
        {
            // Reload the additive scene after unloading
            SceneManager.LoadScene("Eating", LoadSceneMode.Additive);
        };
    }

    public void next()
    {
        PlayerPrefs.SetInt("Eating", 1);
        SceneManager.UnloadSceneAsync("Eating");
    }
}
