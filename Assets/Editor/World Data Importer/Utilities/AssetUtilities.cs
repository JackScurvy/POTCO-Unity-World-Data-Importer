using UnityEngine;
using UnityEditor;
using System.IO;
using WorldDataImporter.Data;
using POTCO.Editor;

namespace WorldDataImporter.Utilities
{
    public static class AssetUtilities
    {
        public static GameObject InstantiatePrefab(string modelPath, GameObject parentGO, bool useEgg, ImportStatistics stats = null)
        {
            string[] phaseFolders = Directory.GetDirectories("Assets/Resources", "phase_*", SearchOption.AllDirectories);
            GameObject assetToInstantiate = null;
            string extension = useEgg ? ".egg" : ".prefab";

            foreach (string phase in phaseFolders)
            {
                string attemptPath = Path.Combine(phase, modelPath + extension).Replace("\\", "/");
                assetToInstantiate = AssetDatabase.LoadAssetAtPath<GameObject>(attemptPath);
                if (assetToInstantiate != null)
                {
                    GameObject instance;
                    if (useEgg)
                    {
                        // For .egg files, instantiate the imported GameObject
                        instance = Object.Instantiate(assetToInstantiate);
                    }
                    else
                    {
                        // For .prefab files, use PrefabUtility to maintain prefab connection
                        instance = (GameObject)PrefabUtility.InstantiatePrefab(assetToInstantiate);
                    }
                    
                    instance.name = assetToInstantiate.name;
                    instance.transform.SetParent(parentGO.transform, false);
                    
                    if (stats != null) stats.successfulImports++;
                    return instance;
                }
            }
            
            DebugLogger.LogWarningWorldImporter($"❌ {(useEgg ? "EGG" : "Prefab")} not found for model: '{modelPath}'.");
            if (stats != null)
            {
                stats.missingModels++;
                stats.missingModelPaths.Add(modelPath);
            }
            return null;
        }

        public static void ApplyColorOverride(GameObject obj, Color color, ImportStatistics stats = null)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                // Use sharedMaterials to avoid creating instances in edit mode
                var sharedMaterials = renderer.sharedMaterials;
                var newMaterials = new Material[sharedMaterials.Length];
                
                for (int i = 0; i < sharedMaterials.Length; i++)
                {
                    if (sharedMaterials[i] != null)
                    {
                        newMaterials[i] = new Material(sharedMaterials[i]);
                        newMaterials[i].color = color;
                    }
                }
                renderer.sharedMaterials = newMaterials;
            }
            
            // Only increment stats if we actually applied color overrides
            if (stats != null && renderers.Length > 0) stats.colorOverrides++;
        }

        public static void SetCollisionEnabled(GameObject obj, bool enabled, ImportStatistics stats = null)
        {
            var colliders = obj.GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                collider.enabled = enabled;
            }
            
            // Only increment stats if we actually disabled collision
            if (stats != null && colliders.Length > 0 && !enabled) stats.collisionDisabled++;
        }
        
        public static void RemoveCollisions(GameObject obj, ImportStatistics stats = null)
        {
            var colliders = obj.GetComponentsInChildren<Collider>();
            int removedCount = colliders.Length;
            
            foreach (var collider in colliders)
            {
                Object.DestroyImmediate(collider);
            }
            
            // Track removed collisions in stats
            if (stats != null && removedCount > 0) stats.collisionRemoved += removedCount;
        }


        public static void CreateLight(GameObject obj, ObjectData lightData, ImportStatistics stats = null)
        {
            if (lightData?.lightType == null) return;

            // Create Light component
            Light unityLight = obj.GetComponent<Light>();
            if (unityLight == null)
            {
                unityLight = obj.AddComponent<Light>();
            }

            // Map POTCO light type to Unity light type
            switch (lightData.lightType.ToUpper())
            {
                case "POINT":
                    unityLight.type = LightType.Point;
                    break;
                case "SPOT":
                    unityLight.type = LightType.Spot;
                    if (lightData.coneAngle.HasValue)
                    {
                        unityLight.spotAngle = lightData.coneAngle.Value;
                    }
                    break;
                case "AMBIENT":
                    // Unity doesn't have ambient light components, use point light with large range
                    unityLight.type = LightType.Point;
                    unityLight.range = 100f; // Large range for ambient-like effect
                    break;
                default:
                    unityLight.type = LightType.Point;
                    break;
            }

            // Set light properties
            if (lightData.intensity.HasValue)
            {
                // POTCO intensity seems to be 0-1 range, Unity typically uses 0-8
                unityLight.intensity = lightData.intensity.Value * 2f;
            }

            if (lightData.visualColor.HasValue)
            {
                unityLight.color = new Color(lightData.visualColor.Value.r, lightData.visualColor.Value.g, lightData.visualColor.Value.b);
            }

            if (lightData.attenuation.HasValue && lightData.attenuation.Value > 0)
            {
                // Convert POTCO attenuation to Unity range
                // POTCO attenuation of 0.005 should give reasonable range
                unityLight.range = 1f / lightData.attenuation.Value * 10f;
                unityLight.range = Mathf.Clamp(unityLight.range, 1f, 100f);
            }
            else
            {
                unityLight.range = 10f; // Default range
            }

            // Set shadow settings
            unityLight.shadows = LightShadows.Soft;

            // Handle flickering (basic implementation)
            if (lightData.flickering.HasValue && lightData.flickering.Value && lightData.flickRate.HasValue)
            {
                try
                {
                    // Add a simple flickering script component
                    var flickerComponent = obj.AddComponent<LightFlicker>();
                    if (flickerComponent != null)
                    {
                        flickerComponent.flickRate = lightData.flickRate.Value;
                        flickerComponent.originalIntensity = unityLight.intensity;
                        DebugLogger.LogWorldImporter($"💡 Added flickering to light: {obj.name} (Rate: {lightData.flickRate.Value})");
                    }
                }
                catch (System.Exception ex)
                {
                    DebugLogger.LogWarningWorldImporter($"⚠️ Could not add LightFlicker component to {obj.name}: {ex.Message}");
                }
            }

            if (stats != null) stats.lightsCreated++;
            
            DebugLogger.LogWorldImporter($"💡 Created {lightData.lightType} light: {obj.name} (Intensity: {unityLight.intensity}, Range: {unityLight.range})");
        }
    }
}