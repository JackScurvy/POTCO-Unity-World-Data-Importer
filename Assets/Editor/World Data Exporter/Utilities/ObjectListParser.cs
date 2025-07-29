using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using WorldDataExporter.Data;

namespace WorldDataExporter.Utilities
{
    public static class ObjectListParser
    {
        private static Dictionary<string, POTCOObjectDefinition> _objectDefinitions;
        private static Dictionary<string, string> _modelToTypeMap;
        private static HashSet<string> _objectTypesWithNames;
        private static HashSet<string> _objectTypesWithInstanced;
        private static bool _initialized = false;
        
        public static Dictionary<string, POTCOObjectDefinition> GetObjectDefinitions()
        {
            if (!_initialized)
            {
                LoadObjectDefinitions();
            }
            return _objectDefinitions ?? new Dictionary<string, POTCOObjectDefinition>();
        }
        
        public static POTCOObjectDefinition GetObjectDefinition(string objectType)
        {
            var definitions = GetObjectDefinitions();
            
            // Handle Cave_Pieces -> MODULAR_OBJ reverse mapping for data lookup
            if (objectType == "Cave_Pieces" && definitions.ContainsKey("MODULAR_OBJ"))
            {
                return definitions["MODULAR_OBJ"];
            }
            
            return definitions.ContainsKey(objectType) ? definitions[objectType] : null;
        }
        
        public static List<string> GetAllObjectTypes()
        {
            return new List<string>(GetObjectDefinitions().Keys);
        }
        
        public static bool ObjectTypeHasName(string objectType)
        {
            if (!_initialized)
            {
                LoadObjectDefinitions();
            }
            return _objectTypesWithNames != null && _objectTypesWithNames.Contains(objectType);
        }
        
        public static bool ObjectTypeHasInstanced(string objectType)
        {
            if (!_initialized)
            {
                LoadObjectDefinitions();
            }
            return _objectTypesWithInstanced != null && _objectTypesWithInstanced.Contains(objectType);
        }
        
        /// <summary>
        /// Simple lookup: model name -> object type
        /// </summary>
        public static string GetObjectTypeByModelName(string modelName)
        {
            if (!_initialized)
            {
                LoadObjectDefinitions();
            }
            
            if (string.IsNullOrEmpty(modelName)) return "MISC_OBJ";
            
            Debug.Log($"🔍 GetObjectTypeByModelName called with: '{modelName}'");
            
            // Special debugging for interior_tavern
            bool isInteriorTavern = modelName.ToLower().Contains("interior_tavern");
            if (isInteriorTavern)
            {
                Debug.Log($"🏰 DEBUGGING interior_tavern lookup...");
                Debug.Log($"📊 Total entries in _modelToTypeMap: {_modelToTypeMap?.Count ?? 0}");
                
                // Log all keys that contain "tavern" or "interior"
                if (_modelToTypeMap != null)
                {
                    var tavernKeys = _modelToTypeMap.Keys.Where(k => k.Contains("tavern") || k.Contains("interior")).ToList();
                    Debug.Log($"🔍 Found {tavernKeys.Count} keys containing 'tavern' or 'interior':");
                    foreach (var key in tavernKeys)
                    {
                        Debug.Log($"  📝 '{key}' -> '{_modelToTypeMap[key]}'");
                    }
                    
                    // Log first 10 keys to see the pattern
                    var firstKeys = _modelToTypeMap.Keys.Take(10).ToList();
                    Debug.Log($"🔍 First 10 keys in lookup table:");
                    foreach (var key in firstKeys)
                    {
                        Debug.Log($"  📝 '{key}' -> '{_modelToTypeMap[key]}'");
                    }
                }
            }
            
            // Try multiple variations for matching
            string[] variations = {
                modelName.Trim().ToLower(),                           // exact lowercase
                Path.GetFileNameWithoutExtension(modelName).ToLower(), // filename only
                $"models/vegetation/{modelName}".ToLower(),           // common paths
                $"models/props/{modelName}".ToLower(),
                $"models/buildings/{modelName}".ToLower()
            };
            
            if (isInteriorTavern)
            {
                Debug.Log($"🔍 Trying {variations.Length} variations for interior_tavern:");
                for (int i = 0; i < variations.Length; i++)
                {
                    Debug.Log($"  {i + 1}. '{variations[i]}'");
                }
            }
            
            foreach (var variation in variations)
            {
                if (_modelToTypeMap != null && _modelToTypeMap.ContainsKey(variation))
                {
                    string objectType = _modelToTypeMap[variation];
                    
                    // Apply MODULAR_OBJ -> Cave_Pieces mapping for user-friendly display
                    if (objectType == "MODULAR_OBJ")
                    {
                        Debug.Log($"✅ Found match: '{modelName}' -> 'MODULAR_OBJ' -> 'Cave_Pieces' (matched as '{variation}')");
                        return "Cave_Pieces";
                    }
                    
                    Debug.Log($"✅ Found match: '{modelName}' -> '{objectType}' (matched as '{variation}')");
                    return objectType;
                }
                else if (isInteriorTavern)
                {
                    Debug.Log($"❌ No match for variation: '{variation}'");
                }
            }
            
            Debug.Log($"⚠️ No match found for '{modelName}' - returning 'MISC_OBJ'");
            return "MISC_OBJ";
        }
        
        public static POTCOObjectDefinition FindBestMatchingType(string unityObjectName, UnityEngine.GameObject unityObj)
        {
            if (!_initialized)
            {
                LoadObjectDefinitions();
            }
            
            // Simply use the model name lookup
            string objectType = GetObjectTypeByModelName(unityObjectName);
            
            if (objectType != null && objectType != "MISC_OBJ")
            {
                return _objectDefinitions.ContainsKey(objectType) ? _objectDefinitions[objectType] : null;
            }
            
            return null;
        }
        
        
        private static void LoadObjectDefinitions()
        {
            try
            {
                string objectListPath = Path.Combine(Application.dataPath, "Editor", "World Data Exporter", "ObjectList.py");
                
                if (!File.Exists(objectListPath))
                {
                    Debug.LogError($"❌ ObjectList.py not found at: {objectListPath}");
                    _objectDefinitions = new Dictionary<string, POTCOObjectDefinition>();
                    _initialized = true;
                    return;
                }
                
                Debug.Log($"📖 Loading POTCO object definitions from: {objectListPath}");
                
                string content = File.ReadAllText(objectListPath);
                _objectDefinitions = ParseObjectDefinitionsSimple(content);
                _modelToTypeMap = BuildSimpleModelToTypeMap(_objectDefinitions);
                _objectTypesWithNames = ParseObjectTypesWithNames(content);
                _objectTypesWithInstanced = ParseObjectTypesWithInstanced(content);
                
                Debug.Log($"✅ Loaded {_objectDefinitions.Count} object types with Visual blocks");
                Debug.Log($"✅ Built lookup table with {_modelToTypeMap.Count} model mappings");
                Debug.Log($"✅ Found {_objectTypesWithNames.Count} object types with Name property");
                Debug.Log($"✅ Found {_objectTypesWithInstanced.Count} object types with Instanced property");
                _initialized = true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Failed to load ObjectList.py: {ex.Message}");
                _objectDefinitions = new Dictionary<string, POTCOObjectDefinition>();
                _modelToTypeMap = new Dictionary<string, string>();
                _objectTypesWithNames = new HashSet<string>();
                _objectTypesWithInstanced = new HashSet<string>();
                _initialized = true;
            }
        }
        
        /// <summary>
        /// Build a simple lookup table: model name -> object type
        /// </summary>
        private static Dictionary<string, string> BuildSimpleModelToTypeMap(Dictionary<string, POTCOObjectDefinition> definitions)
        {
            var modelToTypeMap = new Dictionary<string, string>();
            int tavernModelsAdded = 0;
            
            foreach (var kvp in definitions)
            {
                string objectType = kvp.Key;
                var definition = kvp.Value;
                
                // Map each full model path to its object type
                foreach (string modelPath in definition.visual.models)
                {
                    // Normalize the path (lowercase, trim)
                    string normalizedPath = modelPath.Trim().ToLower();
                    
                    // Also extract just the filename for flexible matching
                    string modelName = Path.GetFileNameWithoutExtension(modelPath);
                    string normalizedName = modelName.Trim().ToLower();
                    
                    // Debug logging for tavern/interior models
                    bool isTavernOrInterior = normalizedPath.Contains("tavern") || normalizedPath.Contains("interior") ||
                                            normalizedName.Contains("tavern") || normalizedName.Contains("interior");
                    
                    if (isTavernOrInterior)
                    {
                        Debug.Log($"🏰 Adding tavern/interior model to lookup: '{normalizedPath}' -> '{objectType}'");
                        Debug.Log($"🏰 Also adding filename: '{normalizedName}' -> '{objectType}'");
                        tavernModelsAdded++;
                    }
                    
                    // Add full path mapping
                    if (!modelToTypeMap.ContainsKey(normalizedPath))
                    {
                        modelToTypeMap[normalizedPath] = objectType;
                    }
                    
                    // Add filename mapping
                    if (!string.IsNullOrEmpty(normalizedName) && !modelToTypeMap.ContainsKey(normalizedName))
                    {
                        modelToTypeMap[normalizedName] = objectType;
                    }
                }
            }
            
            Debug.Log($"📊 Built lookup table with {modelToTypeMap.Count} model mappings");
            Debug.Log($"🏰 Added {tavernModelsAdded} tavern/interior models to lookup table");
            return modelToTypeMap;
        }
        
        
        private static Dictionary<string, POTCOObjectDefinition> ParseObjectDefinitionsSimple(string content)
        {
            var definitions = new Dictionary<string, POTCOObjectDefinition>();
            
            try
            {
                Debug.Log("🔍 Starting simple Visual-based parsing with regex approach...");
                Debug.Log($"📄 Processing {content.Length} characters of ObjectList.py content");
                
                // First, parse all constants for replacement
                var constants = ParseConstants(content);
                Debug.Log($"📋 Found {constants.Count} constants for replacement");
                
                // First, let's find the AVAIL_OBJ_LIST section - it goes to the end of the file
                var availObjMatch = Regex.Match(content, @"AVAIL_OBJ_LIST\s*=\s*\{(.*)", RegexOptions.Singleline);
                if (!availObjMatch.Success)
                {
                    Debug.LogError("❌ Could not find AVAIL_OBJ_LIST section");
                    return definitions;
                }
                
                string objListContent = availObjMatch.Groups[1].Value;
                Debug.Log($"📦 Found AVAIL_OBJ_LIST with {objListContent.Length} characters");
                
                // Look for object definitions that contain Visual blocks
                // Pattern: 'ObjectType': { ... } OR CONSTANT: { ... }
                // Require constants to be at least 2 characters to avoid matching single letters
                var objectPattern = @"(?:'([^']+)'|([A-Z_][A-Z0-9_]{1,})):?\s*\{([^{}]*(?:\{[^{}]*(?:\{[^{}]*\}[^{}]*)*\}[^{}]*)*)\}";
                var objectMatches = Regex.Matches(objListContent, objectPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                
                Debug.Log($"🎯 Found {objectMatches.Count} potential object definitions");
                
                foreach (Match match in objectMatches)
                {
                    // Extract object type name - either from string literal or constant
                    string objectType = null;
                    if (!string.IsNullOrEmpty(match.Groups[1].Value))
                    {
                        // String literal: 'ObjectType'
                        objectType = match.Groups[1].Value;
                    }
                    else if (!string.IsNullOrEmpty(match.Groups[2].Value))
                    {
                        // Constant: CONSTANT_NAME (resolve to string value)
                        string constantName = match.Groups[2].Value;
                        if (constants.ContainsKey(constantName))
                        {
                            objectType = constants[constantName];
                            Debug.Log($"🔄 Resolved constant '{constantName}' -> '{objectType}'");
                        }
                        else
                        {
                            Debug.LogWarning($"⚠️ Unknown constant '{constantName}', using as-is");
                            objectType = constantName;
                        }
                    }
                    
                    string objectContent = match.Groups[3].Value;
                    
                    if (!string.IsNullOrEmpty(objectType))
                    {
                        Debug.Log($"🔍 Processing potential object: '{objectType}'");
                        
                        // Check if this object has a Visual block
                        if (objectContent.Contains("'Visual':"))
                        {
                            Debug.Log($"📸 '{objectType}' has Visual block, parsing...");
                            ProcessObjectContentRegex(objectType, objectContent, definitions, content);
                        }
                        else
                        {
                            Debug.Log($"⏭️ Skipping '{objectType}' - no Visual block");
                        }
                    }
                }
                
                Debug.Log($"✅ Regex parsing completed. Found {definitions.Count} valid object types with Visual blocks.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Error in simple parsing: {ex.Message}");
                Debug.LogError($"Stack trace: {ex.StackTrace}");
            }
            
            return definitions;
        }
        
        private static void ProcessObjectContent(string objectType, string content, Dictionary<string, POTCOObjectDefinition> definitions)
        {
            Debug.Log($"🔍 Processing object '{objectType}' with {content.Length} characters of content");
            
            // Only process if it has a Visual block
            if (!content.Contains("'Visual':"))
            {
                Debug.Log($"⏭️ Skipping '{objectType}' - no Visual block found");
                return;
            }
            
            Debug.Log($"📸 '{objectType}' has Visual block, processing...");
            
            // Extract Visual block content
            var visualMatch = Regex.Match(content, @"'Visual'\s*:\s*\{([^{}]+(?:\{[^{}]+\}[^{}]+)*)\}", RegexOptions.Singleline);
            if (!visualMatch.Success) return;
            
            string visualContent = visualMatch.Groups[1].Value;
            var definition = new POTCOObjectDefinition(objectType);
            bool foundModels = false;
            
            // Extract Model (single)
            var modelMatch = Regex.Match(visualContent, @"'Model'\s*:\s*'([^']+)'");
            if (modelMatch.Success)
            {
                string modelPath = modelMatch.Groups[1].Value.Trim();
                if (modelPath.StartsWith("models/" ))
                {
                    definition.visual.models.Add(modelPath);
                    foundModels = true;
                }
            }
            
            // Extract Models (array) - only simple string arrays
            var modelsMatch = Regex.Match(visualContent, @"'Models'\s*:\s*\[([^\]]+)\]", RegexOptions.Singleline);
            if (modelsMatch.Success)
            {
                string modelsContent = modelsMatch.Groups[1].Value;
                
                // Skip if it contains UI elements or nested structures
                if (!modelsContent.Contains("PROP_UI_" ) && !modelsContent.Contains("["))
                {
                    // Extract all model paths
                    var modelPaths = Regex.Matches(modelsContent, @"'(models/[^']+)'");
                    foreach (Match pathMatch in modelPaths)
                    {
                        string modelPath = pathMatch.Groups[1].Value.Trim();
                        definition.visual.models.Add(modelPath);
                        foundModels = true;
                    }
                }
            }
            
            // Only add definition if we found at least one model
            if (foundModels)
            {
                definitions[objectType] = definition;
                Debug.Log($"📋 Added '{objectType}' with {definition.visual.models.Count} models");
            }
        }
        
        private static void ProcessObjectContentRegex(string objectType, string content, Dictionary<string, POTCOObjectDefinition> definitions, string fullContent)
        {
            Debug.Log($"🔍 Processing object '{objectType}' with regex approach");
            
            // Extract Visual block content using regex
            var visualMatch = Regex.Match(content, @"'Visual'\s*:\s*\{([^{}]*(?:\{[^{}]*\}[^{}]*)*)\}", RegexOptions.Singleline);
            if (!visualMatch.Success)
            {
                Debug.Log($"⏭️ No Visual block found in '{objectType}'");
                return;
            }
            
            string visualContent = visualMatch.Groups[1].Value;
            var definition = new POTCOObjectDefinition(objectType);
            bool foundModels = false;
            
            Debug.Log($"📸 Found Visual block in '{objectType}': {visualContent.Substring(0, Math.Min(100, visualContent.Length))}...");
            
            // Extract Model (single)
            var modelMatch = Regex.Match(visualContent, @"'Model'\s*:\s*'([^']+)'");
            if (modelMatch.Success)
            {
                string modelPath = modelMatch.Groups[1].Value.Trim();
                if (modelPath.StartsWith("models/"))
                {
                    definition.visual.models.Add(modelPath);
                    foundModels = true;
                    Debug.Log($"✅ Found single Model in '{objectType}': {modelPath}");
                }
            }
            
            // Extract Models (array or variable reference)
            var modelsMatch = Regex.Match(visualContent, @"'Models'\s*:\s*((\[([^\]]+)\])|([A-Z_][A-Z0-9_]*)|'([^']+)')", RegexOptions.Singleline);
            if (modelsMatch.Success)
            {
                if (!string.IsNullOrEmpty(modelsMatch.Groups[3].Value))
                {
                    // Direct array: ['model1', 'model2']
                    string modelsContent = modelsMatch.Groups[3].Value;
                    
                    // Skip if it contains UI elements or nested structures
                    if (!modelsContent.Contains("PROP_UI_") && !modelsContent.Contains("["))
                    {
                        // Extract all model paths
                        var modelPaths = Regex.Matches(modelsContent, @"'(models/[^']+)'");
                        foreach (Match pathMatch in modelPaths)
                        {
                            string modelPath = pathMatch.Groups[1].Value.Trim();
                            definition.visual.models.Add(modelPath);
                            foundModels = true;
                            Debug.Log($"✅ Found Model in '{objectType}' Models array: {modelPath}");
                        }
                    }
                    else
                    {
                        Debug.Log($"⏭️ Skipping Models array in '{objectType}' - contains UI elements or nested structures");
                    }
                }
                else if (!string.IsNullOrEmpty(modelsMatch.Groups[4].Value))
                {
                    // Variable reference: BUILDING_INTERIOR_LIST
                    string variableName = modelsMatch.Groups[4].Value;
                    Debug.Log($"🔗 Found Models variable reference in '{objectType}': {variableName}");
                    
                    // Resolve variable to model list
                    var resolvedModels = ResolveVariableToModelList(variableName, fullContent);
                    foreach (string modelPath in resolvedModels)
                    {
                        definition.visual.models.Add(modelPath);
                        foundModels = true;
                        Debug.Log($"✅ Resolved Model from {variableName} in '{objectType}': {modelPath}");
                    }
                }
                else if (!string.IsNullOrEmpty(modelsMatch.Groups[5].Value))
                {
                    // Single model string: 'models/buildings/something'
                    string modelPath = modelsMatch.Groups[5].Value.Trim();
                    if (modelPath.StartsWith("models/"))
                    {
                        definition.visual.models.Add(modelPath);
                        foundModels = true;
                        Debug.Log($"✅ Found single Model string in '{objectType}': {modelPath}");
                    }
                }
            }
            
            // Only add definition if we found at least one model
            if (foundModels)
            {
                definitions[objectType] = definition;
                Debug.Log($"📋 Added '{objectType}' with {definition.visual.models.Count} models");
            }
            else
            {
                Debug.Log($"⏭️ No valid models found in '{objectType}'");
            }
        }
        
        private static HashSet<string> ParseObjectTypesWithNames(string content)
        {
            var typesWithNames = new HashSet<string>();
            
            try
            {
                // Parse constant definitions first to get the actual type names
                // AREA_TYPE_BUILDING_INTERIOR = 'Building Interior'
                var buildingInteriorConstant = Regex.Match(content, @"AREA_TYPE_BUILDING_INTERIOR\s*=\s*'([^']+)'");
                if (buildingInteriorConstant.Success)
                {
                    string buildingInteriorType = buildingInteriorConstant.Groups[1].Value;
                    typesWithNames.Add(buildingInteriorType);
                    Debug.Log($"📋 Found building interior type with Name: {buildingInteriorType}");
                }
                
                
                // AREA_TYPE_ISLAND = 'Island'
                var islandConstant = Regex.Match(content, @"AREA_TYPE_ISLAND\s*=\s*'([^']+)'");
                if (islandConstant.Success)
                {
                    string islandType = islandConstant.Groups[1].Value;
                    typesWithNames.Add(islandType);
                    Debug.Log($"📋 Found island type with Name: {islandType}");
                }
                
                // AREA_TYPE_ISLAND_REGION = 'Island Game Area'
                var islandRegionConstant = Regex.Match(content, @"AREA_TYPE_ISLAND_REGION\s*=\s*'([^']+)'");
                if (islandRegionConstant.Success)
                {
                    string islandRegionType = islandRegionConstant.Groups[1].Value;
                    typesWithNames.Add(islandRegionType);
                    Debug.Log($"📋 Found island region type with Name: {islandRegionType}");
                }
                
                // AREA_TYPE_WORLD_REGION = 'Region'
                var worldRegionConstant = Regex.Match(content, @"AREA_TYPE_WORLD_REGION\s*=\s*'([^']+)'");
                if (worldRegionConstant.Success)
                {
                    string worldRegionType = worldRegionConstant.Groups[1].Value;
                    typesWithNames.Add(worldRegionType);
                    Debug.Log($"📋 Found world region type with Name: {worldRegionType}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Error parsing object types with names: {ex.Message}");
            }
            
            return typesWithNames;
        }
        
        private static HashSet<string> ParseObjectTypesWithInstanced(string content)
        {
            var typesWithInstanced = new HashSet<string>();
            
            try
            {
                // Only building interiors have Instanced property
                // AREA_TYPE_BUILDING_INTERIOR = 'Building Interior'
                var buildingInteriorConstant = Regex.Match(content, @"AREA_TYPE_BUILDING_INTERIOR\s*=\s*'([^']+)'");
                if (buildingInteriorConstant.Success)
                {
                    string buildingInteriorType = buildingInteriorConstant.Groups[1].Value;
                    typesWithInstanced.Add(buildingInteriorType);
                    Debug.Log($"📋 Found building interior type with Instanced: {buildingInteriorType}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Error parsing object types with instanced: {ex.Message}");
            }
            
            return typesWithInstanced;
        }
        
        /// <summary>
        /// Resolve a variable name like BUILDING_INTERIOR_LIST to its list of model paths
        /// </summary>
        private static List<string> ResolveVariableToModelList(string variableName, string fullContent)
        {
            var models = new List<string>();
            
            try
            {
                // Skip single-character variables (likely parsing errors)
                if (string.IsNullOrEmpty(variableName) || variableName.Length < 2)
                {
                    // Silently skip - these are false matches from regex, not actual errors
                    return models;
                }
                
                Debug.Log($"🔍 Resolving variable '{variableName}' to model list...");
                
                // Look for variable definition: VARIABLE_NAME = [...]
                var pattern = $@"{Regex.Escape(variableName)}\s*=\s*\[\s*((?:'[^']*'\s*,?\s*)*)\s*\]";
                var match = Regex.Match(fullContent, pattern, RegexOptions.Singleline);
                
                if (match.Success)
                {
                    string listContent = match.Groups[1].Value;
                    Debug.Log($"🔍 Found {variableName} definition with content: {listContent.Substring(0, Math.Min(200, listContent.Length))}...");
                    
                    // Extract all model paths from the list
                    var modelMatches = Regex.Matches(listContent, @"'(models/[^']+)'");
                    foreach (Match modelMatch in modelMatches)
                    {
                        string modelPath = modelMatch.Groups[1].Value.Trim();
                        models.Add(modelPath);
                        Debug.Log($"✅ Resolved model from {variableName}: {modelPath}");
                    }
                    
                    Debug.Log($"✅ Resolved {models.Count} models from variable '{variableName}'");
                }
                else
                {
                    Debug.LogWarning($"⚠️ Could not find definition for variable '{variableName}'");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Error resolving variable '{variableName}': {ex.Message}");
            }
            
            return models;
        }
        
        /// <summary>
        /// Parse constant definitions from ObjectList.py
        /// </summary>
        private static Dictionary<string, string> ParseConstants(string content)
        {
            var constants = new Dictionary<string, string>();
            
            try
            {
                // Parse all constant definitions like: CONSTANT_NAME = 'Value'
                var constantPattern = @"([A-Z_][A-Z0-9_]*)\s*=\s*'([^']+)'";
                var matches = Regex.Matches(content, constantPattern);
                
                foreach (Match match in matches)
                {
                    string constantName = match.Groups[1].Value;
                    string constantValue = match.Groups[2].Value;
                    constants[constantName] = constantValue;
                    Debug.Log($"📝 Found constant: {constantName} = '{constantValue}'");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Error parsing constants: {ex.Message}");
            }
            
            return constants;
        }
    }
}