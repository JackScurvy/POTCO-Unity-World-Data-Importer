using UnityEngine;
using UnityEditor;

public class EggImporterSettingsWindow : EditorWindow
{
    private EggImporterSettings settings;
    private SerializedObject serializedSettings;
    
    [MenuItem("POTCO/Egg Importer Settings")]
    public static void ShowWindow()
    {
        var window = GetWindow<EggImporterSettingsWindow>("Egg Importer Settings");
        window.minSize = new Vector2(400, 300);
        window.Show();
    }
    
    private void OnEnable()
    {
        settings = EggImporterSettings.Instance;
        serializedSettings = new SerializedObject(settings);
    }
    
    private void OnGUI()
    {
        if (settings == null || serializedSettings == null)
        {
            OnEnable();
            return;
        }
        
        serializedSettings.Update();
        
        GUILayout.Label("POTCO Egg Importer Settings", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        // LOD Import Settings
        GUILayout.Label("LOD Import Settings", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        EditorGUI.BeginChangeCheck();
        
        var lodModeProperty = serializedSettings.FindProperty("lodImportMode");
        EditorGUILayout.PropertyField(lodModeProperty, new GUIContent("LOD Import Mode"));
        
        // Add help text based on selected mode
        switch ((EggImporterSettings.LODImportMode)lodModeProperty.enumValueIndex)
        {
            case EggImporterSettings.LODImportMode.HighestOnly:
                EditorGUILayout.HelpBox("Only imports the highest quality LOD (Distance ending with 0). This is the recommended setting for most use cases.", MessageType.Info);
                break;
            case EggImporterSettings.LODImportMode.AllLODs:
                EditorGUILayout.HelpBox("Imports all LOD levels as separate GameObjects. Useful for analyzing LOD differences or manual LOD setup.", MessageType.Info);
                break;
            case EggImporterSettings.LODImportMode.Custom:
                EditorGUILayout.HelpBox("Custom LOD selection (Not yet implemented).", MessageType.Warning);
                break;
        }
        
        GUILayout.Space(15);
        
        // Collision Import Settings
        GUILayout.Label("Collision Import Settings", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        var collisionProperty = serializedSettings.FindProperty("importCollisions");
        EditorGUILayout.PropertyField(collisionProperty, new GUIContent("Import Collisions"));
        EditorGUILayout.HelpBox("Enable to import collision geometry. Disabled by default as collision meshes are usually not needed for visual purposes.", MessageType.Info);
        
        GUILayout.Space(15);
        
        // Debug Settings
        GUILayout.Label("Debug Settings", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        var debugProperty = serializedSettings.FindProperty("enableDebugLogging");
        EditorGUILayout.PropertyField(debugProperty, new GUIContent("Enable Debug Logging"));
        EditorGUILayout.HelpBox("Enable detailed logging during EGG import process.", MessageType.None);
        
        if (EditorGUI.EndChangeCheck())
        {
            serializedSettings.ApplyModifiedProperties();
            EditorUtility.SetDirty(settings);
        }
        
        GUILayout.Space(20);
        
        // Information section
        GUILayout.Label("Information", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("LOD Detection: Groups with <SwitchCondition> and <Distance> tags are considered LODs. The LOD with Distance ending in 0 is considered the highest quality.", MessageType.None);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Reset to Defaults"))
        {
            settings.lodImportMode = EggImporterSettings.LODImportMode.HighestOnly;
            settings.importCollisions = false;
            settings.enableDebugLogging = true;
            EditorUtility.SetDirty(settings);
        }
    }
}