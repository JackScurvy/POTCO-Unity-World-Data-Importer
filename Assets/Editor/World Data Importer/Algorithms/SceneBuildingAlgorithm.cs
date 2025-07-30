using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using WorldDataImporter.Utilities;
using WorldDataImporter.Processors;
using WorldDataImporter.Data;
using POTCO;
using POTCO.Editor;

namespace WorldDataImporter.Algorithms
{
    public static class SceneBuildingAlgorithm
    {
        public static ImportStatistics BuildSceneFromPython(string path, bool useEgg, ImportSettings settings = null)
        {
            var startTime = System.DateTime.Now;
            var stats = new ImportStatistics();
            
            DebugLogger.LogWorldImporter($"📥 Reading file: {path}");
            string[] lines = File.ReadAllLines(path);

            Dictionary<string, GameObject> createdObjects = new();
            Dictionary<string, ObjectData> objectDataMap = new();
            Stack<(GameObject go, ObjectData data, int indent)> parentStack = new();
            GameObject root = null;
            ObjectData rootData = null;
            HashSet<GameObject> holidayObjectsToDelete = new HashSet<GameObject>();
            HashSet<GameObject> nodeObjectsToDelete = new HashSet<GameObject>();
            HashSet<GameObject> collisionObjectsToDelete = new HashSet<GameObject>();

            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                string line = lines[lineIndex];
                if (string.IsNullOrWhiteSpace(line)) continue;

                int indent = line.TakeWhile(char.IsWhiteSpace).Count();

                while (parentStack.Count > 0 && indent <= parentStack.Peek().indent)
                {
                    parentStack.Pop();
                }

                var current = parentStack.Count > 0 ? parentStack.Peek() : (null, null, 0);
                GameObject currentGO = current.go;
                ObjectData currentData = current.data;

                if (ParsingUtilities.IsObjectId(line, out string currentId))
                {
                    var newGO = new GameObject(currentId);
                    var newData = new ObjectData 
                    { 
                        id = currentId, 
                        gameObject = newGO, 
                        indent = indent 
                    };
                    
                    // Add POTCOTypeInfo component to store metadata only if ImportObjectListData is enabled
                    if (settings != null && settings.importObjectListData)
                    {
                        var typeInfo = Undo.AddComponent<POTCOTypeInfo>(newGO);
                        typeInfo.objectId = currentId;
                    }
                    
                    createdObjects[currentId] = newGO;
                    objectDataMap[currentId] = newData;
                    stats.totalObjects++;

                    if (currentGO != null) 
                    {
                        newGO.transform.SetParent(currentGO.transform, false);
                    }
                    else 
                    {
                        root = newGO;
                        rootData = newData;
                    }

                    parentStack.Push((newGO, newData, indent));
                    continue;
                }

                if (ParsingUtilities.IsProperty(line, out string key, out string val) && currentGO != null)
                {
                    // Mark holiday objects for deletion after parsing (don't destroy during parsing)
                    if (settings != null && !settings.importHolidayObjects && 
                        key == "Holiday" && !string.IsNullOrEmpty(val))
                    {
                        string holiday = ParsingUtilities.ExtractStringValue(val);
                        if (!string.IsNullOrEmpty(holiday))
                        {
                            // Mark this object for deletion after parsing is complete
                            if (currentGO != root)
                            {
                                DebugLogger.LogWorldImporter($"🎄 Marking holiday object for deletion: {currentGO.name} (Holiday: {holiday})");
                                holidayObjectsToDelete.Add(currentGO);
                            }
                        }
                    }
                    
                    // Mark node objects for deletion if nodes are disabled
                    if (settings != null && !settings.importNodes && 
                        key == "Type" && !string.IsNullOrEmpty(val))
                    {
                        string objectType = ParsingUtilities.ExtractStringValue(val);
                        if (objectType.Contains("Node") || objectType == "Townsperson")
                        {
                            // Mark this node object for deletion after parsing is complete
                            if (currentGO != root)
                            {
                                DebugLogger.LogWorldImporter($"🎯 Marking node object for deletion: {currentGO.name} (Type: {objectType})");
                                nodeObjectsToDelete.Add(currentGO);
                            }
                        }
                    }
                    
                    // Mark collision objects for deletion if collisions are disabled
                    if (settings != null && !settings.importCollisions && 
                        key == "Type" && !string.IsNullOrEmpty(val))
                    {
                        string objectType = ParsingUtilities.ExtractStringValue(val);
                        if (objectType.Contains("Collision Barrier"))
                        {
                            // Mark this collision object for deletion after parsing is complete
                            if (currentGO != root)
                            {
                                DebugLogger.LogWorldImporter($"🚧 Marking collision object for deletion: {currentGO.name} (Type: {objectType})");
                                collisionObjectsToDelete.Add(currentGO);
                            }
                        }
                    }

                    PropertyProcessor.ProcessProperty(key, val, currentGO, root, useEgg, currentData, stats, settings);
                    continue;
                }
            }

            // Clean up holiday objects after parsing is complete
            if (holidayObjectsToDelete.Count > 0)
            {
                DebugLogger.LogWorldImporter($"🎄 Cleaning up {holidayObjectsToDelete.Count} holiday objects...");
                foreach (var holidayObj in holidayObjectsToDelete)
                {
                    if (holidayObj != null)
                    {
                        Object.DestroyImmediate(holidayObj);
                    }
                }
            }
            
            // Clean up node objects after parsing is complete
            if (nodeObjectsToDelete.Count > 0)
            {
                DebugLogger.LogWorldImporter($"🎯 Cleaning up {nodeObjectsToDelete.Count} node objects...");
                foreach (var nodeObj in nodeObjectsToDelete)
                {
                    if (nodeObj != null)
                    {
                        Object.DestroyImmediate(nodeObj);
                    }
                }
            }
            
            // Clean up collision objects after parsing is complete
            if (collisionObjectsToDelete.Count > 0)
            {
                DebugLogger.LogWorldImporter($"🚧 Cleaning up {collisionObjectsToDelete.Count} collision objects...");
                foreach (var collisionObj in collisionObjectsToDelete)
                {
                    if (collisionObj != null)
                    {
                        Object.DestroyImmediate(collisionObj);
                    }
                }
            }

            stats.importTime = (float)(System.DateTime.Now - startTime).TotalSeconds;
            LogImportStatistics(stats, path);
            DebugLogger.LogWorldImporter($"✅ Scene built successfully in {stats.importTime:F2} seconds.");
            
            return stats;
        }

        /// <summary>
        /// Coroutine version of BuildSceneFromPython that adds delays between object creation
        /// </summary>
        public static IEnumerator BuildSceneFromPythonCoroutine(string path, bool useEgg, ImportSettings settings, System.Action<ImportStatistics> onComplete)
        {
            var startTime = System.DateTime.Now;
            var stats = new ImportStatistics();
            
            DebugLogger.LogWorldImporter($"📥 Reading file: {path}");
            string[] lines = File.ReadAllLines(path);

            Dictionary<string, GameObject> createdObjects = new();
            Dictionary<string, ObjectData> objectDataMap = new();
            Stack<(GameObject go, ObjectData data, int indent)> parentStack = new();
            GameObject root = null;
            ObjectData rootData = null;
            HashSet<GameObject> holidayObjectsToDelete = new HashSet<GameObject>();
            HashSet<GameObject> nodeObjectsToDelete = new HashSet<GameObject>();
            HashSet<GameObject> collisionObjectsToDelete = new HashSet<GameObject>();

            int objectsCreated = 0;
            
            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                string line = lines[lineIndex];
                if (string.IsNullOrWhiteSpace(line)) continue;

                int indent = line.TakeWhile(char.IsWhiteSpace).Count();

                while (parentStack.Count > 0 && indent <= parentStack.Peek().indent)
                {
                    parentStack.Pop();
                }

                var current = parentStack.Count > 0 ? parentStack.Peek() : (null, null, 0);
                GameObject currentGO = current.go;
                ObjectData currentData = current.data;

                if (ParsingUtilities.IsObjectId(line, out string currentId))
                {
                    var newGO = new GameObject(currentId);
                    var newData = new ObjectData 
                    { 
                        id = currentId, 
                        gameObject = newGO, 
                        indent = indent 
                    };
                    
                    // Add POTCOTypeInfo component to store metadata only if ImportObjectListData is enabled
                    if (settings != null && settings.importObjectListData)
                    {
                        var typeInfo = Undo.AddComponent<POTCOTypeInfo>(newGO);
                        typeInfo.objectId = currentId;
                    }
                    
                    createdObjects[currentId] = newGO;
                    objectDataMap[currentId] = newData;
                    stats.totalObjects++;

                    if (currentGO != null) 
                    {
                        newGO.transform.SetParent(currentGO.transform, false);
                    }
                    else 
                    {
                        root = newGO;
                        rootData = newData;
                    }

                    parentStack.Push((newGO, newData, indent));
                    objectsCreated++;
                    
                    // Add delay after creating objects (but not after every line parse)
                    if (settings != null && settings.useGenerationDelay && objectsCreated % 5 == 0) // Every 5 objects
                    {
                        yield return new WaitForSeconds(settings.delayBetweenObjects);
                    }
                    
                    continue;
                }

                if (ParsingUtilities.IsProperty(line, out string key, out string val) && currentGO != null)
                {
                    // Mark holiday objects for deletion after parsing (don't destroy during parsing)
                    if (settings != null && !settings.importHolidayObjects && 
                        key == "Holiday" && !string.IsNullOrEmpty(val))
                    {
                        string holiday = ParsingUtilities.ExtractStringValue(val);
                        if (!string.IsNullOrEmpty(holiday))
                        {
                            // Mark this object for deletion after parsing is complete
                            if (currentGO != root)
                            {
                                DebugLogger.LogWorldImporter($"🎄 Marking holiday object for deletion: {currentGO.name} (Holiday: {holiday})");
                                holidayObjectsToDelete.Add(currentGO);
                            }
                        }
                    }
                    
                    // Mark node objects for deletion if nodes are disabled
                    if (settings != null && !settings.importNodes && 
                        key == "Type" && !string.IsNullOrEmpty(val))
                    {
                        string objectType = ParsingUtilities.ExtractStringValue(val);
                        if (objectType.Contains("Node") || objectType == "Townsperson")
                        {
                            // Mark this node object for deletion after parsing is complete
                            if (currentGO != root)
                            {
                                DebugLogger.LogWorldImporter($"🎯 Marking node object for deletion: {currentGO.name} (Type: {objectType})");
                                nodeObjectsToDelete.Add(currentGO);
                            }
                        }
                    }
                    
                    // Mark collision objects for deletion if collisions are disabled
                    if (settings != null && !settings.importCollisions && 
                        key == "Type" && !string.IsNullOrEmpty(val))
                    {
                        string objectType = ParsingUtilities.ExtractStringValue(val);
                        if (objectType.Contains("Collision"))
                        {
                            if (currentGO != root)
                            {
                                DebugLogger.LogWorldImporter($"🚧 Marking collision object for deletion: {currentGO.name}");
                                collisionObjectsToDelete.Add(currentGO);
                            }
                        }
                    }
                    
                    PropertyProcessor.ProcessProperty(key, val, currentGO, root, useEgg, currentData, stats, settings);
                }
            }

            // Process all the data (same as original method)
            yield return new WaitForSeconds(0.01f); // Small delay before processing
            
            // Clean up holiday objects after parsing is complete
            if (holidayObjectsToDelete.Count > 0)
            {
                DebugLogger.LogWorldImporter($"🎄 Cleaning up {holidayObjectsToDelete.Count} holiday objects...");
                foreach (var holidayObj in holidayObjectsToDelete)
                {
                    if (holidayObj != null)
                    {
                        Object.DestroyImmediate(holidayObj);
                    }
                }
            }
            
            // Clean up node objects after parsing is complete
            if (nodeObjectsToDelete.Count > 0)
            {
                DebugLogger.LogWorldImporter($"🎯 Cleaning up {nodeObjectsToDelete.Count} node objects...");
                foreach (var nodeObj in nodeObjectsToDelete)
                {
                    if (nodeObj != null)
                    {
                        Object.DestroyImmediate(nodeObj);
                    }
                }
            }
            
            // Clean up collision objects after parsing is complete
            if (collisionObjectsToDelete.Count > 0)
            {
                DebugLogger.LogWorldImporter($"🚧 Cleaning up {collisionObjectsToDelete.Count} collision objects...");
                foreach (var collisionObj in collisionObjectsToDelete)
                {
                    if (collisionObj != null)
                    {
                        Object.DestroyImmediate(collisionObj);
                    }
                }
            }
            
            stats.importTime = (float)(System.DateTime.Now - startTime).TotalSeconds;
            LogImportStatistics(stats, path);
            DebugLogger.LogWorldImporter($"✅ Scene built successfully in {stats.importTime:F2} seconds with delays.");
            
            onComplete?.Invoke(stats);
        }

        private static void LogImportStatistics(ImportStatistics stats, string filePath)
        {
            DebugLogger.LogWorldImporter($"📊 Import Statistics for {System.IO.Path.GetFileName(filePath)}:");
            DebugLogger.LogWorldImporter($"   • Total Objects: {stats.totalObjects}");
            DebugLogger.LogWorldImporter($"   • Successful Imports: {stats.successfulImports}");
            DebugLogger.LogWorldImporter($"   • Missing Models: {stats.missingModels}");
            DebugLogger.LogWorldImporter($"   • Color Overrides Applied: {stats.colorOverrides}");
            DebugLogger.LogWorldImporter($"   • Collision Disabled: {stats.collisionDisabled}");
            DebugLogger.LogWorldImporter($"   • Import Time: {stats.importTime:F2}s");
            
            if (stats.objectTypeCount.Count > 0)
            {
                DebugLogger.LogWorldImporter("   📋 Object Types:");
                foreach (var kvp in stats.objectTypeCount)
                {
                    DebugLogger.LogWorldImporter($"      - {kvp.Key}: {kvp.Value}");
                }
            }
        }
    }
}