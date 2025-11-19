using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Automatically scales UI text based on screen size and platform
/// Ensures text is readable on mobile devices
/// </summary>
public class ResponsiveTextScaler : MonoBehaviour
{
    [Header("Scale Factors")]
    [Tooltip("Base scale multiplier for all text")]
    [Range(0.5f, 3f)]
    public float globalTextScale = 1f;

    [Tooltip("Additional scale for mobile devices")]
    [Range(1f, 2.5f)]
    public float mobileScaleMultiplier = 1.5f;

    [Tooltip("Additional scale for tablets")]
    [Range(1f, 2f)]
    public float tabletScaleMultiplier = 1.3f;

    [Header("Screen Size Thresholds")]
    [Tooltip("DPI threshold for mobile devices")]
    public float mobileDPIThreshold = 200f;

    [Tooltip("Minimum screen width for tablet (inches)")]
    public float tabletMinInches = 7f;

    [Header("References")]
    public UIDocument[] uiDocuments;

    private float calculatedScale = 1f;
    private bool isInitialized = false;

    void Start()
    {
        // Auto-find all UIDocuments if not assigned
        if (uiDocuments == null || uiDocuments.Length == 0)
        {
            uiDocuments = FindObjectsOfType<UIDocument>();
        }

        CalculateScale();
        ApplyScaling();
        isInitialized = true;
    }

    void CalculateScale()
    {
        calculatedScale = globalTextScale;

        // Detect device type and apply appropriate scaling
        DeviceType deviceType = DetectDeviceType();

        switch (deviceType)
        {
            case DeviceType.Mobile:
                calculatedScale *= mobileScaleMultiplier;
                Debug.Log($"📱 Mobile device detected - Text scale: {calculatedScale}x");
                break;

            case DeviceType.Tablet:
                calculatedScale *= tabletScaleMultiplier;
                Debug.Log($"📱 Tablet device detected - Text scale: {calculatedScale}x");
                break;

            case DeviceType.Desktop:
                Debug.Log($"🖥️ Desktop device detected - Text scale: {calculatedScale}x");
                break;
        }

        // Additional scaling based on screen DPI
        float dpiScale = CalculateDPIScale();
        calculatedScale *= dpiScale;

        Debug.Log($"✅ Final text scale: {calculatedScale}x (DPI factor: {dpiScale}x)");
    }

    DeviceType DetectDeviceType()
    {
#if UNITY_ANDROID || UNITY_IOS
        // Calculate screen size in inches
        float screenWidth = Screen.width / Screen.dpi;
        float screenHeight = Screen.height / Screen.dpi;
        float diagonalInches = Mathf.Sqrt(screenWidth * screenWidth + screenHeight * screenHeight);

        if (diagonalInches >= tabletMinInches)
        {
            return DeviceType.Tablet;
        }
        else
        {
            return DeviceType.Mobile;
        }
#else
        return DeviceType.Desktop;
#endif
    }

    float CalculateDPIScale()
    {
        float dpi = Screen.dpi;
        
        // Fallback for devices that don't report DPI
        if (dpi <= 0)
        {
            dpi = 160f; // Standard Android DPI
        }

        // Scale text up for high DPI screens
        if (dpi > mobileDPIThreshold)
        {
            float dpiRatio = dpi / mobileDPIThreshold;
            // Cap the DPI scaling to prevent text from becoming too large
            return Mathf.Clamp(dpiRatio, 1f, 1.5f);
        }

        return 1f;
    }

    void ApplyScaling()
    {
        foreach (var uiDoc in uiDocuments)
        {
            if (uiDoc == null) continue;

            var root = uiDoc.rootVisualElement;
            if (root == null) continue;

            // Apply scaling to all text elements
            ScaleTextElements(root);
        }
    }

    void ScaleTextElements(VisualElement element)
    {
        // Scale Labels
        if (element is Label label)
        {
            ScaleLabel(label);
        }
        // Scale Buttons (which contain TextElements)
        else if (element is Button button)
        {
            ScaleButton(button);
        }
        // Scale TextFields
        else if (element is TextField textField)
        {
            ScaleTextField(textField);
        }

        // Recursively scale children
        foreach (var child in element.Children())
        {
            ScaleTextElements(child);
        }
    }

    void ScaleLabel(Label label)
    {
        var currentSize = label.resolvedStyle.fontSize;
        if (!float.IsNaN(currentSize) && currentSize > 0)
        {
            float newSize = currentSize * calculatedScale;
            label.style.fontSize = new StyleLength(newSize);
        }
    }

    void ScaleButton(Button button)
    {
        var currentSize = button.resolvedStyle.fontSize;
        if (!float.IsNaN(currentSize) && currentSize > 0)
        {
            float newSize = currentSize * calculatedScale;
            button.style.fontSize = new StyleLength(newSize);
        }
    }

    void ScaleTextField(TextField textField)
    {
        var currentSize = textField.resolvedStyle.fontSize;
        if (!float.IsNaN(currentSize) && currentSize > 0)
        {
            float newSize = currentSize * calculatedScale;
            textField.style.fontSize = new StyleLength(newSize);
        }
    }

    // Public method to manually refresh scaling
    public void RefreshScaling()
    {
        if (!isInitialized) return;
        
        CalculateScale();
        ApplyScaling();
    }

    // Public method to set custom scale
    public void SetCustomScale(float scale)
    {
        globalTextScale = scale;
        RefreshScaling();
    }

#if UNITY_EDITOR
    // Editor-only: Allow testing different scales
    [ContextMenu("Apply 1.5x Scale")]
    void TestScale150() { SetCustomScale(1.5f); }

    [ContextMenu("Apply 2x Scale")]
    void TestScale200() { SetCustomScale(2f); }

    [ContextMenu("Reset Scale")]
    void TestScaleReset() { SetCustomScale(1f); }
#endif
}

enum DeviceType
{
    Mobile,
    Tablet,
    Desktop
}
