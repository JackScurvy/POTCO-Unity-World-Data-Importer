using UnityEngine;
using UnityEditor;
using System.IO;
using POTCO.Editor;

[InitializeOnLoad]
public class EggImportStartupPrompt
{
    private static bool hasPrompted = false;
    
    static EggImportStartupPrompt()
    {
        EditorApplication.delayCall += ShowStartupPrompt;
    }
    
    private static void ShowStartupPrompt()
    {
        // Only show once per session and only if we haven't already prompted
        if (hasPrompted) return;
        hasPrompted = true;
        
        // Check if auto-import is already enabled
        bool autoImportEnabled = EditorPrefs.GetBool("EggImporter_AutoImportEnabled", false);
        if (autoImportEnabled) return; // Skip prompt if auto-import is enabled
        
        // Check if user has chosen to skip this prompt
        bool skipStartupPrompt = EditorPrefs.GetBool("EggImporter_SkipStartupPrompt", false);
        if (skipStartupPrompt) return;
        
        // Check if there are any EGG files in the project
        string[] eggFiles = Directory.GetFiles(Application.dataPath, "*.egg", SearchOption.AllDirectories);
        if (eggFiles.Length == 0) return; // No EGG files found, skip prompt
        
        // Show the blocking modal dialog
        ShowImportPromptDialog(eggFiles.Length);
    }
    
    private static void ShowImportPromptDialog(int eggFileCount)
    {
        string title = "🥚 EGG File Import Required";
        string message = $"Found {eggFileCount} EGG files in the project.\n\n" +
                        "Auto-import is currently DISABLED. Would you like to import all EGG files now?\n\n" +
                        "• Click 'Import Now' to process all EGG files immediately\n" +
                        "• Click 'Skip' to import manually later using POTCO > EGG Importer Manager\n" +
                        "• Click 'Don't Ask Again' to disable this startup prompt";
        
        int choice = EditorUtility.DisplayDialogComplex(
            title,
            message,
            "Import Now", // 0
            "Skip",       // 1
            "Don't Ask Again" // 2
        );
        
        switch (choice)
        {
            case 0: // Import Now
                DebugLogger.LogEggImporter("User chose to import EGG files at startup.");
                ImportAllEggFilesWithProgress(eggFileCount);
                break;
                
            case 1: // Skip
                DebugLogger.LogEggImporter("User chose to skip EGG import at startup.");
                // Do nothing, user can import manually later
                break;
                
            case 2: // Don't Ask Again
                EditorPrefs.SetBool("EggImporter_SkipStartupPrompt", true);
                DebugLogger.LogEggImporter("User chose to disable startup EGG import prompt.");
                EditorUtility.DisplayDialog("Startup Prompt Disabled", 
                    "The startup prompt has been disabled.\n\nYou can re-enable it in:\nPOTCO > EGG Importer Manager > Settings Tab", "OK");
                break;
        }
    }
    
    private static void ImportAllEggFilesWithProgress(int totalFiles)
    {
        string[] eggFiles = Directory.GetFiles(Application.dataPath, "*.egg", SearchOption.AllDirectories);
        int importedCount = 0;
        
        // Temporarily enable auto-import for this batch operation
        bool originalSetting = EditorPrefs.GetBool("EggImporter_AutoImportEnabled", false);
        EditorPrefs.SetBool("EggImporter_AutoImportEnabled", true);
        
        try
        {
            foreach (string fullPath in eggFiles)
            {
                string relativePath = "Assets" + fullPath.Substring(Application.dataPath.Length);
                relativePath = relativePath.Replace('\\', '/');
                
                // Show progress
                string fileName = Path.GetFileName(relativePath);
                bool cancelled = EditorUtility.DisplayCancelableProgressBar(
                    "Importing EGG Files", 
                    $"Processing {fileName}... ({importedCount + 1}/{totalFiles})", 
                    (float)importedCount / totalFiles);
                
                if (cancelled)
                {
                    DebugLogger.LogEggImporter($"EGG import cancelled by user after {importedCount} files.");
                    break;
                }
                
                // Force import the asset
                AssetDatabase.ImportAsset(relativePath, ImportAssetOptions.ForceUpdate);
                importedCount++;
                
                // Small delay to prevent Unity from freezing
                if (importedCount % 5 == 0)
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
        }
        finally
        {
            // Restore original auto-import setting
            EditorPrefs.SetBool("EggImporter_AutoImportEnabled", originalSetting);
            EditorUtility.ClearProgressBar();
        }
        
        // Show completion dialog
        string completionMessage = importedCount == totalFiles 
            ? $"✅ Successfully imported all {importedCount} EGG files!"
            : $"⚠️ Imported {importedCount} of {totalFiles} EGG files.";
            
        EditorUtility.DisplayDialog("Import Complete", completionMessage, "OK");
        DebugLogger.LogEggImporter($"Startup EGG import completed: {importedCount}/{totalFiles} files processed.");
    }
    
}