using UnityEngine;

namespace POTCO.Editor
{
    /// <summary>
    /// Centralized debug logging system that respects DebugSettings flags
    /// </summary>
    public static class DebugLogger
    {
        /// <summary>
        /// Log message for World Scene Importer
        /// </summary>
        public static void LogWorldImporter(string message)
        {
            if (DebugSettings.debugWorldSceneImporter)
            {
                Debug.Log(message);
            }
        }
        
        /// <summary>
        /// Log warning for World Scene Importer
        /// </summary>
        public static void LogWarningWorldImporter(string message)
        {
            if (DebugSettings.debugWorldSceneImporter)
            {
                Debug.LogWarning(message);
            }
        }
        
        /// <summary>
        /// Log error for World Scene Importer
        /// </summary>
        public static void LogErrorWorldImporter(string message)
        {
            if (DebugSettings.debugWorldSceneImporter)
            {
                Debug.LogError(message);
            }
        }

        /// <summary>
        /// Log message for Auto POTCO Detection
        /// </summary>
        public static void LogAutoPOTCO(string message)
        {
            if (DebugSettings.debugAutoPOTCODetection)
            {
                Debug.Log(message);
            }
        }
        
        /// <summary>
        /// Log warning for Auto POTCO Detection
        /// </summary>
        public static void LogWarningAutoPOTCO(string message)
        {
            if (DebugSettings.debugAutoPOTCODetection)
            {
                Debug.LogWarning(message);
            }
        }
        
        /// <summary>
        /// Log error for Auto POTCO Detection
        /// </summary>
        public static void LogErrorAutoPOTCO(string message)
        {
            if (DebugSettings.debugAutoPOTCODetection)
            {
                Debug.LogError(message);
            }
        }

        /// <summary>
        /// Log message for EGG File Importer
        /// </summary>
        public static void LogEggImporter(string message)
        {
            if (DebugSettings.debugEggImporter)
            {
                Debug.Log(message);
            }
        }
        
        /// <summary>
        /// Log warning for EGG File Importer
        /// </summary>
        public static void LogWarningEggImporter(string message)
        {
            if (DebugSettings.debugEggImporter)
            {
                Debug.LogWarning(message);
            }
        }
        
        /// <summary>
        /// Log error for EGG File Importer
        /// </summary>
        public static void LogErrorEggImporter(string message)
        {
            if (DebugSettings.debugEggImporter)
            {
                Debug.LogError(message);
            }
        }

        /// <summary>
        /// Log message for World Data Exporter
        /// </summary>
        public static void LogWorldExporter(string message)
        {
            if (DebugSettings.debugWorldDataExporter)
            {
                Debug.Log(message);
            }
        }
        
        /// <summary>
        /// Log warning for World Data Exporter
        /// </summary>
        public static void LogWarningWorldExporter(string message)
        {
            if (DebugSettings.debugWorldDataExporter)
            {
                Debug.LogWarning(message);
            }
        }
        
        /// <summary>
        /// Log error for World Data Exporter
        /// </summary>
        public static void LogErrorWorldExporter(string message)
        {
            if (DebugSettings.debugWorldDataExporter)
            {
                Debug.LogError(message);
            }
        }

        /// <summary>
        /// Log message for Procedural Generation (Cave Generator)
        /// </summary>
        public static void LogProceduralGeneration(string message)
        {
            if (DebugSettings.debugProceduralGeneration)
            {
                Debug.Log(message);
            }
        }
        
        /// <summary>
        /// Log warning for Procedural Generation (Cave Generator)
        /// </summary>
        public static void LogWarningProceduralGeneration(string message)
        {
            if (DebugSettings.debugProceduralGeneration)
            {
                Debug.LogWarning(message);
            }
        }
        
        /// <summary>
        /// Log error for Procedural Generation (Cave Generator)
        /// </summary>
        public static void LogErrorProceduralGeneration(string message)
        {
            if (DebugSettings.debugProceduralGeneration)
            {
                Debug.LogError(message);
            }
        }

        /// <summary>
        /// Always log critical messages regardless of debug settings
        /// </summary>
        public static void LogAlways(string message)
        {
            Debug.Log(message);
        }
        
        /// <summary>
        /// Always log critical warnings regardless of debug settings
        /// </summary>
        public static void LogWarningAlways(string message)
        {
            Debug.LogWarning(message);
        }
        
        /// <summary>
        /// Always log critical errors regardless of debug settings
        /// </summary>
        public static void LogErrorAlways(string message)
        {
            Debug.LogError(message);
        }
    }
}