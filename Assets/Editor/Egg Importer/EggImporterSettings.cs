using UnityEngine;

[System.Serializable]
public class EggImporterSettings : ScriptableObject
{
    [Header("LOD Import Settings")]
    public LODImportMode lodImportMode = LODImportMode.HighestOnly;
    
    [Header("Footprint Settings")]
    public bool skipFootprints = true;
    
    [Header("Animation Settings")]
    public bool skipAnimations = false;
    public bool skipSkeletalModels = false;
    
    [Header("Collision Import Settings")]
    public bool skipCollisions = true;
    public bool importCollisions = false; // For compatibility with reference implementation
    
    [Header("Debug Settings")]
    public bool enableDebugLogging = true;
    
    [Header("Pivot Settings")]
    public PivotMode pivotMode = PivotMode.BottomCenter;
    
    public enum LODImportMode
    {
        HighestOnly,    // Import only the highest quality LOD (default)
        AllLODs,        // Import all LOD levels
        Custom          // Allow custom LOD selection (future feature)
    }
    
    public enum PivotMode
    {
        Original,       // Keep vertices as they are (pivot at origin)
        Center,         // Center of bounds
        BottomCenter,   // Bottom center of bounds (default for most models)
        TopCenter,      // Top center of bounds
        Custom          // Future: allow custom pivot offset
    }
    
    private static EggImporterSettings _instance;
    
    public static EggImporterSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<EggImporterSettings>("EggImporterSettings");
                if (_instance == null)
                {
                    _instance = CreateInstance<EggImporterSettings>();
                    // Create the settings file in Resources folder if it doesn't exist
                    #if UNITY_EDITOR
                    if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Resources"))
                    {
                        UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");
                    }
                    UnityEditor.AssetDatabase.CreateAsset(_instance, "Assets/Resources/EggImporterSettings.asset");
                    UnityEditor.AssetDatabase.SaveAssets();
                    #endif
                }
            }
            return _instance;
        }
    }
}