using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Manages sorting order for UI Toolkit panels to prevent overlap issues
/// Higher sort order = rendered on top
/// </summary>
public class UIPanelSortingManager : MonoBehaviour
{
    public static UIPanelSortingManager Instance;

    [Header("Sorting Order Configuration")]
    [Tooltip("Base HUD elements (always at bottom)")]
    public int hudSortOrder = 0;
    
    [Tooltip("Game UI elements (joystick, pulse button)")]
    public int gameUISortOrder = 10;
    
    [Tooltip("Tutorial panel")]
    public int tutorialSortOrder = 100;
    
    [Tooltip("Pause menu")]
    public int pauseSortOrder = 200;
    
    [Tooltip("Win/Loss panels")]
    public int modalSortOrder = 300;
    
    [Tooltip("Transition effects (highest)")]
    public int transitionSortOrder = 1000;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Set the sorting order for a UIDocument component
    /// </summary>
    public void SetPanelSortOrder(UIDocument uiDocument, PanelType panelType)
    {
        if (uiDocument == null)
        {
            Debug.LogWarning("UIPanelSortingManager: UIDocument is null!");
            return;
        }

        int sortOrder = GetSortOrderForType(panelType);
        
        var panelSettings = uiDocument.panelSettings;
        if (panelSettings != null)
        {
            panelSettings.sortingOrder = sortOrder;
            Debug.Log($"Set {panelType} panel sorting order to {sortOrder}");
        }
        else
        {
            Debug.LogWarning($"UIPanelSortingManager: Panel settings not found for {panelType}");
        }
    }

    /// <summary>
    /// Bring a panel to the front temporarily
    /// </summary>
    public void BringToFront(UIDocument uiDocument)
    {
        if (uiDocument == null) return;
        
        var panelSettings = uiDocument.panelSettings;
        if (panelSettings != null)
        {
            panelSettings.sortingOrder = transitionSortOrder + 100;
        }
    }

    /// <summary>
    /// Reset panel to its default sorting order
    /// </summary>
    public void ResetToDefaultOrder(UIDocument uiDocument, PanelType panelType)
    {
        SetPanelSortOrder(uiDocument, panelType);
    }

    private int GetSortOrderForType(PanelType type)
    {
        return type switch
        {
            PanelType.HUD => hudSortOrder,
            PanelType.GameUI => gameUISortOrder,
            PanelType.Tutorial => tutorialSortOrder,
            PanelType.Pause => pauseSortOrder,
            PanelType.Modal => modalSortOrder,
            PanelType.Transition => transitionSortOrder,
            _ => 0
        };
    }
}

/// <summary>
/// Panel type enumeration for sorting
/// </summary>
public enum PanelType
{
    HUD,        // Base HUD (level, score, energy)
    GameUI,     // Interactive game UI (joystick, pulse button)
    Tutorial,   // Tutorial overlay
    Pause,      // Pause menu
    Modal,      // Win/Loss panels
    Transition  // Level transitions
}
