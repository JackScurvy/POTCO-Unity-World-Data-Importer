using UnityEngine;
using UnityEditor;

namespace POTCO.Editor
{
    /// <summary>
    /// Centralized debug settings for all POTCO tools - completely independent from any specific tool
    /// </summary>
    public static class DebugSettings
    {
        // Keys for EditorPrefs storage
        private const string DEBUG_WORLD_IMPORTER_KEY = "POTCO_Debug_WorldImporter";
        private const string DEBUG_AUTO_POTCO_KEY = "POTCO_Debug_AutoPOTCO";
        private const string DEBUG_EGG_IMPORTER_KEY = "POTCO_Debug_EggImporter";
        private const string DEBUG_WORLD_EXPORTER_KEY = "POTCO_Debug_WorldExporter";
        private const string DEBUG_PROCEDURAL_GEN_KEY = "POTCO_Debug_ProceduralGen";

        // Debug settings properties that persist between Unity sessions
        public static bool debugWorldSceneImporter
        {
            get => EditorPrefs.GetBool(DEBUG_WORLD_IMPORTER_KEY, false);
            set => EditorPrefs.SetBool(DEBUG_WORLD_IMPORTER_KEY, value);
        }

        public static bool debugAutoObjectListDetection
        {
            get => EditorPrefs.GetBool(DEBUG_AUTO_POTCO_KEY, false);
            set 
            { 
                EditorPrefs.SetBool(DEBUG_AUTO_POTCO_KEY, value);
                // Apply immediately to AutoObjectListDetection
                AutoObjectListDetection.SetDebugLogging(value);
            }
        }

        public static bool debugEggImporter
        {
            get => EditorPrefs.GetBool(DEBUG_EGG_IMPORTER_KEY, false);
            set => EditorPrefs.SetBool(DEBUG_EGG_IMPORTER_KEY, value);
        }

        public static bool debugWorldDataExporter
        {
            get => EditorPrefs.GetBool(DEBUG_WORLD_EXPORTER_KEY, false);
            set => EditorPrefs.SetBool(DEBUG_WORLD_EXPORTER_KEY, value);
        }

        public static bool debugProceduralGeneration
        {
            get => EditorPrefs.GetBool(DEBUG_PROCEDURAL_GEN_KEY, false);
            set => EditorPrefs.SetBool(DEBUG_PROCEDURAL_GEN_KEY, value);
        }

        /// <summary>
        /// Enable all debug logging
        /// </summary>
        public static void EnableAllDebug()
        {
            debugWorldSceneImporter = true;
            debugAutoObjectListDetection = true;
            debugEggImporter = true;
            debugWorldDataExporter = true;
            debugProceduralGeneration = true;
            
            DebugLogger.LogAlways("🔍 All POTCO debug logging enabled");
        }

        /// <summary>
        /// Disable all debug logging for maximum performance
        /// </summary>
        public static void DisableAllDebug()
        {
            debugWorldSceneImporter = false;
            debugAutoObjectListDetection = false;
            debugEggImporter = false;
            debugWorldDataExporter = false;
            debugProceduralGeneration = false;
            
            DebugLogger.LogAlways("🔇 All POTCO debug logging disabled");
        }

        /// <summary>
        /// Reset all debug settings to default (disabled)
        /// </summary>
        public static void ResetToDefaults()
        {
            EditorPrefs.DeleteKey(DEBUG_WORLD_IMPORTER_KEY);
            EditorPrefs.DeleteKey(DEBUG_AUTO_POTCO_KEY);
            EditorPrefs.DeleteKey(DEBUG_EGG_IMPORTER_KEY);
            EditorPrefs.DeleteKey(DEBUG_WORLD_EXPORTER_KEY);
            EditorPrefs.DeleteKey(DEBUG_PROCEDURAL_GEN_KEY);
            
            // Apply AutoObjectListDetection change
            AutoObjectListDetection.SetDebugLogging(false);
            
            DebugLogger.LogAlways("🔄 POTCO debug settings reset to defaults");
        }
    }
}