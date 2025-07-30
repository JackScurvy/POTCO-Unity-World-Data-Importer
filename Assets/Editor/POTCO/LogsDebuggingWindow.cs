using UnityEditor;
using UnityEngine;

namespace POTCO.Editor
{
    public class LogsDebuggingWindow : EditorWindow
    {
        private Vector2 scrollPosition;

        [MenuItem("Logs Debugging/Debug Controls")]
        public static void ShowWindow()
        {
            GetWindow<LogsDebuggingWindow>("Debug Controls");
        }

        void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            GUILayout.Label("🐛 POTCO Toolkit Debug Controls", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            EditorGUILayout.LabelField("Control debug logging for all POTCO tools from this central location.", EditorStyles.wordWrappedLabel);
            GUILayout.Space(10);

            DrawDebugControls();

            EditorGUILayout.EndScrollView();
        }

        private void DrawDebugControls()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Debug Logging Controls", EditorStyles.boldLabel);
            GUILayout.Space(5);
            
            GUILayout.Label("Enable debug logging for specific tools:", EditorStyles.miniLabel);
            GUILayout.Space(5);
            
            // World Scene Importer
            EditorGUILayout.BeginHorizontal();
            DebugSettings.debugWorldSceneImporter = EditorGUILayout.Toggle("World Scene Importer", DebugSettings.debugWorldSceneImporter);
            if (DebugSettings.debugWorldSceneImporter) EditorGUILayout.LabelField("🔍", GUILayout.Width(20));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("   Logs object placement, model loading, and import progress", EditorStyles.miniLabel);
            
            GUILayout.Space(3);
            
            // Auto POTCO Detection
            EditorGUILayout.BeginHorizontal();
            DebugSettings.debugAutoPOTCODetection = EditorGUILayout.Toggle("Auto POTCO Detection", DebugSettings.debugAutoPOTCODetection);
            if (DebugSettings.debugAutoPOTCODetection) EditorGUILayout.LabelField("🔍", GUILayout.Width(20));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("   Logs automatic POTCOTypeInfo component assignment", EditorStyles.miniLabel);
            
            GUILayout.Space(3);
            
            // EGG Importer
            EditorGUILayout.BeginHorizontal();
            DebugSettings.debugEggImporter = EditorGUILayout.Toggle("EGG File Importer", DebugSettings.debugEggImporter);
            if (DebugSettings.debugEggImporter) EditorGUILayout.LabelField("🔍", GUILayout.Width(20));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("   Logs .egg file parsing, geometry processing, and animation import", EditorStyles.miniLabel);
            
            GUILayout.Space(3);
            
            // World Data Exporter
            EditorGUILayout.BeginHorizontal();
            DebugSettings.debugWorldDataExporter = EditorGUILayout.Toggle("World Data Exporter", DebugSettings.debugWorldDataExporter);
            if (DebugSettings.debugWorldDataExporter) EditorGUILayout.LabelField("🔍", GUILayout.Width(20));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("   Logs scene export, coordinate conversion, and Python file generation", EditorStyles.miniLabel);
            
            GUILayout.Space(3);
            
            // Procedural Generation (includes Cave Generator)
            EditorGUILayout.BeginHorizontal();
            DebugSettings.debugProceduralGeneration = EditorGUILayout.Toggle("Procedural Generation", DebugSettings.debugProceduralGeneration);
            if (DebugSettings.debugProceduralGeneration) EditorGUILayout.LabelField("🔍", GUILayout.Width(20));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("   Logs cave generation, connector validation, placement algorithms, and procedural processes", EditorStyles.miniLabel);
            
            GUILayout.Space(10);
            
            // Control buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("🔍 Enable All Debug", GUILayout.Height(30)))
            {
                DebugSettings.EnableAllDebug();
            }
            
            if (GUILayout.Button("🔇 Disable All Debug", GUILayout.Height(30)))
            {
                DebugSettings.DisableAllDebug();
            }
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            
            // Reset button
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("🔄 Reset to Defaults", GUILayout.Height(25)))
            {
                if (EditorUtility.DisplayDialog("Reset Debug Settings", 
                    "This will reset all debug settings to their defaults (disabled). Continue?", 
                    "Reset", "Cancel"))
                {
                    DebugSettings.ResetToDefaults();
                }
            }
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            
            // Performance warning
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("⚠️ Performance Impact", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Debug logging can significantly slow down operations, especially during:", EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField("• World imports with hundreds of objects", EditorStyles.miniLabel);
            EditorGUILayout.LabelField("• Large .egg file processing", EditorStyles.miniLabel);
            EditorGUILayout.LabelField("• Procedural cave generation", EditorStyles.miniLabel);
            EditorGUILayout.LabelField("• Bulk POTCOTypeInfo detection", EditorStyles.miniLabel);
            EditorGUILayout.LabelField("Disable debug logging for maximum performance.", EditorStyles.miniLabel);
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndVertical();
        }
    }
}