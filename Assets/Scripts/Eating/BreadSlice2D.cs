using UnityEngine;

public class BreadSlice2D : MonoBehaviour
{
    public bool isSpread = false; // Track if butter/jam is applied
    public float butterPercentage = 0f; // Percentage of butter applied
    public float jamPercentage = 0f; // Percentage of jam applied
    public Texture2D breadTexture; // Texture acting as the canvas
    public Color butterColor = Color.yellow; // Butter color
    public Color jamColor = Color.red; // Jam color
    public int brushSize = 5; // Brush size for painting
    public GameObject jamSpread;
    public GameObject butterSpread;

    private SpriteRenderer spriteRenderer;
    private bool butter = false;
    private bool jam = false;

    private Vector3 lastMousePosition; // To track mouse movement

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Initialize the texture for painting
        breadTexture = new Texture2D(512, 512);
        breadTexture.filterMode = FilterMode.Point;
        for (int x = 0; x < breadTexture.width; x++)
        {
            for (int y = 0; y < breadTexture.height; y++)
            {
                breadTexture.SetPixel(x, y, Color.clear); // Transparent canvas
            }
        }
        breadTexture.Apply();

        // Assign the texture to the bread slice material
        spriteRenderer.material.mainTexture = breadTexture;
        butterSpread.transform.localScale = Vector3.zero;
        jamSpread.transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.B))
        {
            butter = true;
            jam = false;
        }
        else if (Input.GetKey(KeyCode.J))
        {
            jam = true;
            butter = false;
        }
    }

    private void OnMouseDrag()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Set z position to 0 since it's a 2D game

        // Check for mouse movement
        if (lastMousePosition != Vector3.zero && lastMousePosition == mousePosition)
        {
            // Mouse is idle, don't paint
            return;
        }

        if (GetComponent<Collider2D>().OverlapPoint(mousePosition))
        {
            Vector2 localPoint = transform.InverseTransformPoint(mousePosition);
            int pixelX = Mathf.FloorToInt((localPoint.x + 0.5f) * breadTexture.width);
            int pixelY = Mathf.FloorToInt((localPoint.y + 0.5f) * breadTexture.height);

            // Paint with butter or jam
            if (butter)
            {
                Paint(pixelX, pixelY, butterColor);
                IncreaseButter(5f);
            }
            else if (jam)
            {
                Paint(pixelX, pixelY, jamColor);
                IncreaseJam(5f);
            }
        }

        lastMousePosition = mousePosition; // Update last mouse position
    }

    private void OnMouseUp()
    {
        lastMousePosition = Vector3.zero; // Reset mouse position when drag ends
    }

    private void Paint(int x, int y, Color color)
    {
        for (int i = -brushSize; i <= brushSize; i++)
        {
            for (int j = -brushSize; j <= brushSize; j++)
            {
                if (Vector2.Distance(Vector2.zero, new Vector2(i, j)) <= brushSize)
                {
                    int pixelX = Mathf.Clamp(x + i, 0, breadTexture.width - 1);
                    int pixelY = Mathf.Clamp(y + j, 0, breadTexture.height - 1);
                    breadTexture.SetPixel(pixelX, pixelY, color);
                }
            }
        }
        breadTexture.Apply();
    }

    public void IncreaseButter(float amount)
    {
        butterPercentage += amount;
        if (butterPercentage > 100) butterPercentage = 100; // Cap at 100%

        // Calculate the scaling factor (from 0 to 1)
        float scaleFactor = (float)butterPercentage / 100;

        // Interpolate between zero scale and max scale
        butterSpread.transform.localScale = new Vector3(scaleFactor * 0.81615f, 0.81615f, 0.81615f);

        UpdatePercentageText();
        CheckCompletion();
    }

    public void IncreaseJam(float amount)
    {
        jamPercentage += amount;
        if (jamPercentage > 100) jamPercentage = 100; // Cap at 100%

        // Calculate the scaling factor (from 0 to 1)
        float scaleFactor = (float)jamPercentage / 100;

        // Interpolate between zero scale and max scale
        jamSpread.transform.localScale = new Vector3(scaleFactor, 1, 1);

        UpdatePercentageText();
        CheckCompletion();
    }

    private void UpdatePercentageText()
    {
        BreadSpreadManager.Instance.ButterPercentage.text =
            "Butter = " + butterPercentage.ToString("F1") + "%";
        BreadSpreadManager.Instance.JamPercentage.text =
            "Jam = " + jamPercentage.ToString("F1") + "%";
    }

    private void CheckCompletion()
    {
        if (butterPercentage >= 75 && jamPercentage >= 75)
        {
            isSpread = true; // Mark this slice as spread
            ResetPercentages();
            BreadSpreadManager.Instance.IncrementSliceCounter(); // Notify GameManager to increment counter
            Destroy(gameObject); // Remove this slice from the scene
        }
    }

    private void ResetPercentages()
    {
        butterPercentage = 0f;
        jamPercentage = 0f;
    }
}
