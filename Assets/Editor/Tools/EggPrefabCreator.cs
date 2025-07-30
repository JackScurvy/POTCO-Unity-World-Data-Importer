using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using POTCO.Editor;

public class EggPrefabCreator : EditorWindow
{
    [MenuItem("POTCO/EGG Prefab Creator")]
    public static void ShowWindow()
    {
        GetWindow<EggPrefabCreator>("EGG Prefab Creator");
    }

    void OnGUI()
    {
        GUILayout.Label("EGG to Prefab Batch Converter", EditorStyles.boldLabel);
        
        GUILayout.Space(10);
        
        GUILayout.Label("This tool creates Unity prefabs from all .egg files in the Resources folder.", EditorStyles.wordWrappedLabel);
        GUILayout.Label("Prefabs will be created in the same location as the .egg files.", EditorStyles.wordWrappedLabel);
        
        GUILayout.Space(20);
        
        if (GUILayout.Button("Create Missing Prefabs Only", GUILayout.Height(30)))
        {
            CreatePrefabsFromEggFiles(false);
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Force Recreate All Prefabs", GUILayout.Height(30)))
        {
            bool confirm = EditorUtility.DisplayDialog("Confirm Force Recreate", 
                "This will overwrite ALL existing prefabs. Are you sure?", 
                "Yes, Overwrite All", "Cancel");
                
            if (confirm)
            {
                CreatePrefabsFromEggFiles(true);
            }
        }
    }

    private static void CreatePrefabsFromEggFiles(bool forceRecreate)
    {
        DebugLogger.LogEggImporter($"🥚 Starting batch prefab creation from .egg files... (Force recreate: {forceRecreate})");
        
        // Find all .egg files in Resources folder
        string resourcesPath = "Assets/Resources";
        string[] eggFiles = System.IO.Directory.GetFiles(resourcesPath, "*.egg", System.IO.SearchOption.AllDirectories);
        
        int successCount = 0;
        int failCount = 0;
        int skippedCount = 0;
        
        EditorUtility.DisplayProgressBar("Creating Prefabs", "Starting...", 0f);
        
        for (int i = 0; i < eggFiles.Length; i++)
        {
            string eggPath = eggFiles[i];
            float progress = (float)i / eggFiles.Length;
            
            EditorUtility.DisplayProgressBar("Creating Prefabs", 
                $"Processing: {Path.GetFileName(eggPath)}", progress);
            
            try
            {
                string prefabPath = eggPath.Replace(".egg", ".prefab");
                
                // Skip if prefab exists and not forcing recreate
                if (!forceRecreate && System.IO.File.Exists(prefabPath))
                {
                    DebugLogger.LogEggImporter($"⏭️ Skipping {Path.GetFileName(eggPath)} - prefab already exists");
                    skippedCount++;
                    continue;
                }
                
                // Load the imported egg asset
                string assetPath = eggPath.Replace("\\", "/");
                GameObject eggAsset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                
                if (eggAsset != null)
                {
                    // Create instance and save as prefab
                    GameObject instance = Instantiate(eggAsset);
                    GameObject prefab = PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
                    DestroyImmediate(instance);
                    
                    if (prefab != null)
                    {
                        DebugLogger.LogEggImporter($"✅ Created prefab: {Path.GetFileName(prefabPath)}");
                        successCount++;
                    }
                    else
                    {
                        DebugLogger.LogErrorEggImporter($"❌ Failed to save prefab: {prefabPath}");
                        failCount++;
                    }
                }
                else
                {
                    DebugLogger.LogWarningEggImporter($"⚠️ Could not load egg asset: {assetPath}");
                    failCount++;
                }
            }
            catch (System.Exception e)
            {
                DebugLogger.LogErrorEggImporter($"❌ Error processing {eggPath}: {e.Message}");
                failCount++;
            }
        }
        
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        
        DebugLogger.LogEggImporter($"🏁 Prefab creation complete! Success: {successCount}, Failed: {failCount}, Skipped: {skippedCount}, Total: {eggFiles.Length}");
        
        EditorUtility.DisplayDialog("Prefab Creation Complete", 
            $"Successfully created: {successCount} prefabs\nFailed: {failCount}\nSkipped: {skippedCount}\nTotal .egg files: {eggFiles.Length}", 
            "OK");
    }
}