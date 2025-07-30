using UnityEngine;
using UnityEditor;
using Unity.EditorCoroutines.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using POTCO.Editor;

public class ProceduralCaveGenerator : EditorWindow
{
    [MenuItem("POTCO/Procedural Cave Generator")]
    public static void ShowWindow() => GetWindow<ProceduralCaveGenerator>("Cave Generator");

    // --- Generation Settings ---
    [System.Serializable]
    public class GenerationSettings
    {
        public int caveLength = 10;
        public float generationDelay = 0.1f;
        public bool capOpenEnds = true;
        public bool useEggFiles = true;
        public bool preventOverlaps = true;
        public float overlapCheckRadius = 5f;
        public int maxBranches = 3;
        public float branchProbability = 0.3f;
        public bool enableBranching = true;
        public int maxDepth = 8;
        public bool allowLoops = false;
        public bool forceCaveLength = false;
        public int seed = -1; // -1 for random
        public bool visualizeConnectors = false;
        public bool realtimePreview = true;
    }
    
    GenerationSettings settings = new GenerationSettings();

    // --- Enhanced Internal State ---
    GameObject root;
    List<GameObject> allFoundPrefabs = new List<GameObject>();
    List<GameObject> validPrefabs;
    Dictionary<int, List<GameObject>> categorizedPrefabs = new();
    List<GameObject> deadEnds;
    
    // Enhanced tracking
    List<Transform> openConnectors = new();
    HashSet<Vector3> occupiedPositions = new();
    Dictionary<Transform, ConnectorInfo> connectorData = new();
    List<CavePieceNode> generatedPieces = new();
    
    // UI State
    int currentIndex = 1;
    Vector2 prefabScroll;
    Vector2 statsScroll;
    bool showAdvanced = false;
    int selectedTab = 0;
    string[] tabNames = { "Generation", "Pieces", "Rules", "Statistics", "Debug" };

    // Piece selection
    Dictionary<GameObject, bool> prefabToggles = new();
    Dictionary<GameObject, int> prefabLikelihoods = new();
    Dictionary<GameObject, string> prefabTags = new();
    
    // Generation history
    Stack<string> generationHistory = new();
    string lastGenerationSeed = "";
    
    // General Presets
    string[] generalPresetNames = new string[0];
    string[] generalPresetPaths = new string[0];
    int selectedGeneralPresetIndex = 0;
    
    // Cave Piece Presets
    string[] cavePresetNames = new string[0];
    string[] cavePresetPaths = new string[0];
    int selectedCavePresetIndex = 0;
    
    // Connector visualization
    Color connectorOpenColor = Color.green;
    Color connectorUsedColor = Color.red;
    
    // Debug data
    [System.Serializable]
    class DebugSnapshot
    {
        public string timestamp;
        public GenerationSettings settings;
        public List<DebugConnectionData> connections = new();
        public List<DebugPieceData> pieces = new();
        public string notes = "";
    }
    
    [System.Serializable]
    class DebugConnectionData
    {
        public string fromPiece;
        public string toPiece;
        public string fromConnector;
        public string toConnector;
        public Vector3 fromPosition;
        public Vector3 toPosition;
        public Vector3 fromRotation;
        public Vector3 toRotation;
        public Vector3 fromDirection;
        public Vector3 toDirection;
        public float connectionDistance;
        public float angleDifference;
        public bool isCorrectlyAligned;
    }
    
    [System.Serializable]
    class DebugPieceData
    {
        public string pieceName;
        public Vector3 position;
        public Vector3 rotation;
        public List<string> connectorNames = new();
        public List<Vector3> connectorPositions = new();
        public List<Vector3> connectorDirections = new();
    }
    
    DebugSnapshot currentSnapshot;
    DebugSnapshot previousSnapshot;
    DebugSnapshot originalGenerationSnapshot;
    Vector2 debugScroll;
    bool autoRecordOnGeneration = true;
    
    [System.Serializable]
    class ConnectorInfo
    {
        public Transform connector;
        public string type = "default";
        public bool isUsed = false;
        public Transform connectedTo;
        public Vector3 direction;
    }
    
    [System.Serializable]
    class CavePieceNode
    {
        public GameObject piece;
        public Vector3 position;
        public Quaternion rotation;
        public List<Transform> connectors;
        public int depth;
        public bool isDeadEnd;
    }

    void OnEnable()
    {
        LoadAllPrefabs();
        LoadPresetList();
    }
    
    void OnValidate()
    {
        // Reload when switching between egg and prefab mode
        if (allFoundPrefabs != null)
        {
            LoadAllPrefabs();
        }
    }
    
    void LoadPresetList()
    {
        LoadGeneralPresetList();
        LoadCavePresetList();
    }
    
    void LoadGeneralPresetList()
    {
        string presetsPath = "Assets/Editor/Cave Generator/General_Presets";
        if (Directory.Exists(presetsPath))
        {
            string[] jsonFiles = Directory.GetFiles(presetsPath, "*.json", SearchOption.AllDirectories);
            generalPresetPaths = jsonFiles;
            generalPresetNames = new string[jsonFiles.Length + 1];
            generalPresetNames[0] = "None";
            for (int i = 0; i < jsonFiles.Length; i++)
            {
                generalPresetNames[i + 1] = Path.GetFileNameWithoutExtension(jsonFiles[i]);
            }
        }
        else
        {
            generalPresetNames = new string[] { "None" };
            generalPresetPaths = new string[0];
        }
    }
    
    void LoadCavePresetList()
    {
        string presetsPath = "Assets/Editor/Cave Generator/Cave_Presets";
        if (Directory.Exists(presetsPath))
        {
            string[] jsonFiles = Directory.GetFiles(presetsPath, "*.json", SearchOption.AllDirectories);
            cavePresetPaths = jsonFiles;
            cavePresetNames = new string[jsonFiles.Length + 1];
            cavePresetNames[0] = "None";
            for (int i = 0; i < jsonFiles.Length; i++)
            {
                cavePresetNames[i + 1] = Path.GetFileNameWithoutExtension(jsonFiles[i]);
            }
        }
        else
        {
            cavePresetNames = new string[] { "None" };
            cavePresetPaths = new string[0];
        }
    }
    
    void LoadPreset(string path)
    {
        LoadPresetFromPath(path);
    }
    
    void SaveGeneralPreset()
    {
        var presetData = new CavePresetData();
        presetData.settings = settings;
        
        // Save all piece selections
        foreach (var kvp in prefabToggles)
        {
            string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(kvp.Key));
            int likelihood = prefabLikelihoods.ContainsKey(kvp.Key) ? prefabLikelihoods[kvp.Key] : 100;
            presetData.selections.Add(new SelectionEntry { guid = guid, isEnabled = kvp.Value, likelihood = likelihood });
        }
        
        string json = JsonUtility.ToJson(presetData, true);
        string path = EditorUtility.SaveFilePanel("Save General Cave Preset", "Assets/Editor/Cave Generator/General_Presets/", "general_cave_preset.json", "json");
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, json);
            LoadGeneralPresetList(); // Refresh the general preset list
        }
    }
    
    void LoadGeneralPreset()
    {
        string path = EditorUtility.OpenFilePanel("Load General Cave Preset", "Assets/Editor/Cave Generator/General_Presets/", "json");
        if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;
        
        LoadPresetFromPath(path);
    }

    void OnGUI()
    {
        GUILayout.Label("Advanced Procedural Cave Generator", EditorStyles.boldLabel);
        
        // Tab selection
        selectedTab = GUILayout.Toolbar(selectedTab, tabNames);
        GUILayout.Space(10);
        
        switch (selectedTab)
        {
            case 0: DrawGenerationTab(); break;
            case 1: DrawPiecesTab(); break;
            case 2: DrawRulesTab(); break;
            case 3: DrawStatisticsTab(); break;
            case 4: DrawDebugTab(); break;
        }
    }
    
    void DrawGenerationTab()
    {
        // General Preset Management
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("General Preset Management", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("💾 Save Preset"))
        {
            SaveGeneralPreset();
        }
        
        if (GUILayout.Button("📂 Load Preset"))
        {
            LoadGeneralPreset();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // Preset dropdown
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Preset:", GUILayout.Width(50));
        int newGeneralPresetIndex = EditorGUILayout.Popup(selectedGeneralPresetIndex, generalPresetNames);
        if (newGeneralPresetIndex != selectedGeneralPresetIndex)
        {
            selectedGeneralPresetIndex = newGeneralPresetIndex;
            if (selectedGeneralPresetIndex > 0 && selectedGeneralPresetIndex <= generalPresetPaths.Length)
            {
                LoadPreset(generalPresetPaths[selectedGeneralPresetIndex - 1]);
            }
        }
        
        if (GUILayout.Button("🔄", GUILayout.Width(30)))
        {
            LoadGeneralPresetList();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Basic Settings", EditorStyles.boldLabel);
        
        settings.caveLength = EditorGUILayout.IntField("Cave Length", settings.caveLength);
        settings.caveLength = Mathf.Max(1, settings.caveLength); // Ensure minimum of 1
        settings.generationDelay = EditorGUILayout.Slider("Generation Delay", settings.generationDelay, 0f, 1f);
        settings.capOpenEnds = EditorGUILayout.Toggle("Cap Open Ends", settings.capOpenEnds);
        
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Model Source:", GUILayout.Width(100));
        bool previousUseEgg = settings.useEggFiles;
        settings.useEggFiles = EditorGUILayout.Toggle(settings.useEggFiles, GUILayout.Width(20));
        GUILayout.Label(settings.useEggFiles ? "Use .egg files" : "Use .prefab files");
        EditorGUILayout.EndHorizontal();
        
        if (previousUseEgg != settings.useEggFiles)
        {
            LoadAllPrefabs();
        }
        
        EditorGUILayout.EndVertical();
        
        // Advanced settings
        EditorGUILayout.Space();
        showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Advanced Settings");
        if (showAdvanced)
        {
            EditorGUILayout.BeginVertical("box");
            settings.preventOverlaps = EditorGUILayout.Toggle("Prevent Overlaps", settings.preventOverlaps);
            if (settings.preventOverlaps)
            {
                settings.overlapCheckRadius = EditorGUILayout.FloatField("Overlap Check Radius", settings.overlapCheckRadius);
                settings.overlapCheckRadius = Mathf.Max(0.1f, settings.overlapCheckRadius); // Ensure minimum of 0.1
            }
            
            settings.enableBranching = EditorGUILayout.Toggle("Enable Branching", settings.enableBranching);
            if (settings.enableBranching)
            {
                settings.maxBranches = EditorGUILayout.IntField("Max Branches", settings.maxBranches);
                settings.maxBranches = Mathf.Max(1, settings.maxBranches); // Ensure minimum of 1
                settings.branchProbability = EditorGUILayout.Slider("Branch Probability", settings.branchProbability, 0f, 1f);
                settings.maxDepth = EditorGUILayout.IntField("Max Depth", settings.maxDepth);
                settings.maxDepth = Mathf.Max(1, settings.maxDepth); // Ensure minimum of 1
            }
            settings.allowLoops = EditorGUILayout.Toggle("Allow Loops", settings.allowLoops);
            settings.forceCaveLength = EditorGUILayout.Toggle("Force Cave Length", settings.forceCaveLength);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Generation Control", EditorStyles.boldLabel);
            settings.seed = EditorGUILayout.IntField("Seed (-1 for random)", settings.seed);
            settings.realtimePreview = EditorGUILayout.Toggle("Realtime Preview", settings.realtimePreview);
            settings.visualizeConnectors = EditorGUILayout.Toggle("Visualize Connectors", settings.visualizeConnectors);
            
            EditorGUILayout.EndVertical();
        }
        
        EditorGUILayout.Space();
        
        // Generation buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🎲 Generate Cave System", GUILayout.Height(30)))
        {
            GenerateCaveWithEnhancements();
        }
        
        if (GUILayout.Button("🗑️ Clear Cave", GUILayout.Height(30)))
        {
            ClearCave();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("📦 Export as Prefab", GUILayout.Height(25)))
        {
            ExportCaveAsPrefab();
        }
        
        if (GUILayout.Button("🔄 Regenerate Last", GUILayout.Height(25)))
        {
            RegenerateLastCave();
        }
        EditorGUILayout.EndHorizontal();
        
        if (!string.IsNullOrEmpty(lastGenerationSeed))
        {
            EditorGUILayout.HelpBox($"Last generation seed: {lastGenerationSeed}", MessageType.Info);
        }
    }
    
    void DrawPiecesTab()
    {
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Cave Pieces Management", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🔄 Reload Pieces"))
        {
            LoadAllPrefabs();
        }
        
        if (GUILayout.Button("💾 Save Selection"))
        {
            SaveSelections();
        }
        
        if (GUILayout.Button("📂 Load Selection"))
        {
            LoadSelections();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // Preset dropdown
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Preset:", GUILayout.Width(50));
        int newPresetIndex = EditorGUILayout.Popup(selectedCavePresetIndex, cavePresetNames);
        if (newPresetIndex != selectedCavePresetIndex)
        {
            selectedCavePresetIndex = newPresetIndex;
            if (selectedCavePresetIndex > 0 && selectedCavePresetIndex <= cavePresetPaths.Length)
            {
                LoadPreset(cavePresetPaths[selectedCavePresetIndex - 1]);
            }
        }
        
        if (GUILayout.Button("🔄", GUILayout.Width(30)))
        {
            LoadCavePresetList();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        if (allFoundPrefabs == null || allFoundPrefabs.Count == 0)
        {
            EditorGUILayout.HelpBox("No cave pieces found. Make sure .egg or .prefab files exist in Assets/Resources/phase_4/models/caves/", MessageType.Warning);
            return;
        }
        
        GUILayout.Label($"Found {allFoundPrefabs.Count} cave pieces", EditorStyles.boldLabel);
        
        // Statistics
        if (categorizedPrefabs.Count > 0)
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Piece Categories", EditorStyles.boldLabel);
            foreach (var category in categorizedPrefabs.OrderBy(kvp => kvp.Key))
            {
                string label = category.Key == 1 ? "Dead Ends" : $"{category.Key} Connectors";
                EditorGUILayout.LabelField($"{label}: {category.Value.Count} pieces");
            }
            EditorGUILayout.EndVertical();
        }
        
        EditorGUILayout.Space();
        
        // Piece selection UI
        prefabScroll = EditorGUILayout.BeginScrollView(prefabScroll);
        foreach (var category in categorizedPrefabs.OrderBy(kvp => kvp.Key))
        {
            EditorGUILayout.BeginVertical("box");
            string categoryLabel = category.Key == 1 ? "🚫 Dead Ends" : $"🔗 {category.Key} Connectors";
            GUILayout.Label(categoryLabel, EditorStyles.boldLabel);
            
            foreach (var prefab in category.Value)
            {
                DrawPieceSelectionUI(prefab);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        EditorGUILayout.EndScrollView();
        
        EditorGUILayout.EndVertical();
    }
    
    void DrawPieceSelectionUI(GameObject prefab)
    {
        EditorGUILayout.BeginHorizontal();
        
        // Initialize if needed
        if (!prefabToggles.ContainsKey(prefab)) prefabToggles[prefab] = true;
        if (!prefabLikelihoods.ContainsKey(prefab)) prefabLikelihoods[prefab] = 100;
        if (!prefabTags.ContainsKey(prefab)) prefabTags[prefab] = "";
        
        // Toggle
        prefabToggles[prefab] = EditorGUILayout.Toggle(prefabToggles[prefab], GUILayout.Width(20));
        
        // Name
        EditorGUILayout.LabelField(prefab.name, GUILayout.MinWidth(150));
        
        GUILayout.FlexibleSpace();
        
        // Spawn weight
        EditorGUI.BeginDisabledGroup(!prefabToggles[prefab]);
        GUILayout.Label("Weight:", GUILayout.Width(50));
        prefabLikelihoods[prefab] = EditorGUILayout.IntField(prefabLikelihoods[prefab], GUILayout.Width(40));
        prefabLikelihoods[prefab] = Mathf.Clamp(prefabLikelihoods[prefab], 0, 1000);
        EditorGUI.EndDisabledGroup();
        
        // Actions
        if (GUILayout.Button("👁", GUILayout.Width(25)))
        {
            EditorGUIUtility.PingObject(prefab);
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
    void DrawRulesTab()
    {
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Generation Rules", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Advanced connection rules and constraints will be implemented here.", MessageType.Info);
        EditorGUILayout.EndVertical();
    }
    
    void DrawStatisticsTab()
    {
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Generation Statistics", EditorStyles.boldLabel);
        
        if (generatedPieces.Count > 0)
        {
            EditorGUILayout.LabelField($"Generated Pieces: {generatedPieces.Count}");
            EditorGUILayout.LabelField($"Open Connectors: {openConnectors.Count}");
            EditorGUILayout.LabelField($"Used Connectors: {connectorData.Values.Count(c => c.isUsed)}");
            
            if (generationHistory.Count > 0)
            {
                EditorGUILayout.Space();
                GUILayout.Label("Generation History", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Previous generations: {generationHistory.Count}");
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Generate a cave to see statistics.", MessageType.Info);
        }
        
        EditorGUILayout.EndVertical();
    }
    
    void GenerateCaveWithEnhancements()
    {
        if (root != null) DestroyImmediate(root);
        
        // Initialize
        root = new GameObject("GeneratedCaveSystem");
        openConnectors.Clear();
        occupiedPositions.Clear();
        connectorData.Clear();
        generatedPieces.Clear();
        currentIndex = 1;
        
        // Set seed
        if (settings.seed != -1)
        {
            Random.InitState(settings.seed);
            lastGenerationSeed = settings.seed.ToString();
        }
        else
        {
            int randomSeed = Random.Range(0, int.MaxValue);
            Random.InitState(randomSeed);
            lastGenerationSeed = randomSeed.ToString();
        }
        
        // Store generation parameters
        generationHistory.Push(JsonUtility.ToJson(settings));
        
        // Get valid pieces
        validPrefabs = allFoundPrefabs.Where(p => prefabToggles.ContainsKey(p) && prefabToggles[p]).ToList();
        deadEnds = validPrefabs.Where(p => categorizedPrefabs.ContainsKey(1) && categorizedPrefabs[1].Contains(p)).ToList();
        
        if (validPrefabs.Count == 0)
        {
            DebugLogger.LogErrorProceduralGeneration("No valid prefabs selected!");
            return;
        }
        
        // Start generation
        var startPrefabs = validPrefabs.Where(p => !deadEnds.Contains(p)).ToList();
        if (startPrefabs.Count == 0)
        {
            DebugLogger.LogErrorProceduralGeneration("No non-dead-end prefabs selected!");
            return;
        }
        
        var firstPrefab = GetWeightedRandomPrefab(startPrefabs);
        if (firstPrefab == null) return;
        
        // Set root name to starting plane
        root.name = "pir_m_are_cav_startingPlane";
        
        // Place first piece
        var firstNode = PlaceCavePiece(firstPrefab, Vector3.zero, Quaternion.identity, 0);
        if (firstNode != null)
        {
            DebugLogger.LogProceduralGeneration($"🔄 Loop Prevention: {(settings.allowLoops ? "DISABLED (loops allowed)" : "ENABLED (loops prevented)")}");
            DebugLogger.LogProceduralGeneration($"📏 Force Cave Length: {(settings.forceCaveLength ? "ENABLED (will relax validation to reach target length)" : "DISABLED (may end early)")}");
            EditorCoroutineUtility.StartCoroutineOwnerless(GenerateCaveCoroutine());
        }
    }
    
    CavePieceNode PlaceCavePiece(GameObject prefab, Vector3 position, Quaternion rotation, int depth)
    {
        // Check for overlaps if enabled
        if (settings.preventOverlaps && CheckForOverlap(position, settings.overlapCheckRadius))
        {
            DebugLogger.LogProceduralGeneration($"Prevented overlap at {position}");
            return null;
        }
        
        // Create wrapper and instance
        var wrapper = new GameObject($"{prefab.name}");
        wrapper.transform.SetParent(root.transform);
        wrapper.transform.position = position;
        wrapper.transform.rotation = rotation;
        
        // Add ObjectListInfo component for export compatibility
        var potcoInfo = wrapper.AddComponent<POTCO.ObjectListInfo>();
        
        var instance = InstantiateCavePiece(prefab, wrapper.transform);
        
        // Create node
        var node = new CavePieceNode
        {
            piece = wrapper,
            position = position,
            rotation = rotation,
            connectors = instance.GetComponentsInChildren<Transform>()
                .Where(t => t.name.StartsWith("cave_connector_"))
                .ToList(),
            depth = depth,
            isDeadEnd = deadEnds.Contains(prefab)
        };
        
        generatedPieces.Add(node);
        occupiedPositions.Add(position);
        
        // Add connectors to open list (only unused ones)
        foreach (var connector in node.connectors)
        {
            connectorData[connector] = new ConnectorInfo
            {
                connector = connector,
                type = "default",
                isUsed = false,
                direction = connector.forward
            };
            
            // Only add to open connectors if not already used/occupied
            if (!IsConnectorOccupied(connector))
            {
                openConnectors.Add(connector);
            }
        }
        
        currentIndex++;
        return node;
    }
    
    bool CheckForOverlap(Vector3 position, float radius)
    {
        return occupiedPositions.Any(pos => Vector3.Distance(pos, position) < radius);
    }
    
    bool CheckForOverlapExcluding(Vector3 position, float radius, Transform excludeConnector)
    {
        // Get the piece that contains the exclude connector
        GameObject excludePiece = GetCavePieceFromConnector(excludeConnector)?.gameObject;
        Vector3? excludePosition = excludePiece?.transform.position;
        
        foreach (var pos in occupiedPositions)
        {
            // Skip the position of the piece we're connecting to
            if (excludePosition.HasValue && Vector3.Distance(pos, excludePosition.Value) < 0.1f)
                continue;
                
            if (Vector3.Distance(pos, position) < radius)
            {
                DebugLogger.LogProceduralGeneration($"Overlap detected: New position {position} is {Vector3.Distance(pos, position):F2}m from existing position {pos} (threshold: {radius}m)");
                return true;
            }
        }
        return false;
    }
    
    bool IsConnectorOccupied(Transform connector)
    {
        // Check if this connector position is too close to any already used connectors
        float minDistance = 1f; // Minimum distance between connectors
        
        foreach (var existingConnector in connectorData.Keys)
        {
            if (existingConnector != connector && 
                connectorData[existingConnector].isUsed && 
                Vector3.Distance(connector.position, existingConnector.position) < minDistance)
            {
                return true;
            }
        }
        
        return false;
    }
    
    bool WouldCreateLoop(Transform fromConnector, Vector3 newPiecePosition)
    {
        // Get the piece that contains the fromConnector
        Transform fromPiece = GetCavePieceFromConnector(fromConnector);
        if (fromPiece == null) return false;
        
        // Check if the new piece position would be too close to any existing piece
        // that could potentially create a loop back to the source piece
        float loopDetectionRadius = settings.overlapCheckRadius * 1.5f;
        
        foreach (var existingNode in generatedPieces)
        {
            if (existingNode.piece.transform == fromPiece) continue; // Skip the piece we're connecting from
            
            float distance = Vector3.Distance(newPiecePosition, existingNode.position);
            
            // If we're placing a new piece close to an existing piece, check if it would create a potential loop
            if (distance < loopDetectionRadius)
            {
                // Check if the existing piece has any path back to the fromPiece
                if (HasPathBetweenPieces(fromPiece, existingNode.piece.transform))
                {
                    DebugLogger.LogProceduralGeneration($"🔄 Loop detected: New piece at {newPiecePosition} would be {distance:F2}m from {existingNode.piece.name}, which has path back to {fromPiece.name}");
                    return true;
                }
            }
        }
        
        return false;
    }
    
    bool HasPathBetweenPieces(Transform pieceA, Transform pieceB)
    {
        if (pieceA == pieceB) return true;
        
        // Use BFS to find if there's a connection path between two pieces
        var visited = new HashSet<Transform>();
        var queue = new Queue<Transform>();
        
        queue.Enqueue(pieceA);
        visited.Add(pieceA);
        
        while (queue.Count > 0)
        {
            var currentPiece = queue.Dequeue();
            
            // Get all connectors on this piece
            var pieceConnectors = connectorData.Keys
                .Where(c => GetCavePieceFromConnector(c) == currentPiece && connectorData[c].isUsed)
                .ToList();
            
            foreach (var connector in pieceConnectors)
            {
                var connectedTo = connectorData[connector].connectedTo;
                if (connectedTo != null)
                {
                    var connectedPiece = GetCavePieceFromConnector(connectedTo);
                    if (connectedPiece == pieceB) return true; // Found path!
                    
                    if (connectedPiece != null && !visited.Contains(connectedPiece))
                    {
                        visited.Add(connectedPiece);
                        queue.Enqueue(connectedPiece);
                    }
                }
            }
        }
        
        return false;
    }
    
    CavePieceNode ConnectCavePiece(GameObject prefab, Transform fromConnector, int depth, int remainingPieces = 0)
    {
        DebugLogger.LogProceduralGeneration($"🔗 ATTEMPTING CONNECTION: Prefab={prefab.name}, FromConnector={fromConnector.name}, Depth={depth}");
        DebugLogger.LogProceduralGeneration($"   FromConnector position: {fromConnector.position}, direction: {fromConnector.forward}");
        
        // Create temporary instance to get a connector from the new piece
        var tempInstance = InstantiateCavePiece(prefab, null);
        DebugLogger.LogProceduralGeneration($"   Created temp instance: {tempInstance.name}");
        
        var availableConnectors = tempInstance.GetComponentsInChildren<Transform>()
            .Where(t => t.name.StartsWith("cave_connector_"))
            .ToList();
            
        DebugLogger.LogProceduralGeneration($"   Found {availableConnectors.Count} connectors on temp instance");
        
        if (availableConnectors.Count == 0)
        {
            DestroyImmediate(tempInstance);
            DebugLogger.LogWarningProceduralGeneration($"❌ Prefab {prefab.name} has no connectors!");
            return null;
        }
        
        // Choose a random connector from the new piece
        var toConnector = availableConnectors[Random.Range(0, availableConnectors.Count)];
        
        // Calculate the position and rotation to align the connectors
        var wrapper = new GameObject($"{prefab.name}");
        wrapper.transform.SetParent(root.transform);
        
        // Add ObjectListInfo component for export compatibility
        var potcoInfo = wrapper.AddComponent<POTCO.ObjectListInfo>();
        
        // Move the temp instance to the wrapper
        tempInstance.transform.SetParent(wrapper.transform);
        
        // Align the pieces so connectors are at the same position and facing opposite directions
        AlignCavePieces(wrapper, fromConnector, toConnector);
        
        // Validate the connection quality after alignment
        float connectionDistance = Vector3.Distance(fromConnector.position, toConnector.position);
        float connectionAngle = Vector3.Angle(fromConnector.forward, -toConnector.forward);
        
        DebugLogger.LogProceduralGeneration($"🔍 Connection Quality Check: Distance={connectionDistance:F3}m, Angle={connectionAngle:F1}°");
        DebugLogger.LogProceduralGeneration($"   From connector: {fromConnector.name} at {fromConnector.position}, Dir: {fromConnector.forward}");
        DebugLogger.LogProceduralGeneration($"   To connector: {toConnector.name} at {toConnector.position}, Dir: {toConnector.forward}");
        
        // Determine if we should use strict validation or be more lenient
        bool needToForceConnection = settings.forceCaveLength && remainingPieces > 0;
        
        // Reject connections that are too far off (this prevents bad generations)
        // But be more lenient when forcing cave length
        float maxDistance = needToForceConnection ? 4.0f : 2.0f;
        float maxAngle = needToForceConnection ? 90f : 60f;
        
        if (connectionDistance > maxDistance || connectionAngle > maxAngle)
        {
            if (needToForceConnection)
            {
                DebugLogger.LogWarningProceduralGeneration($"⚠️ Poor connection accepted to force cave length: Distance={connectionDistance:F3}m, Angle={connectionAngle:F1}° (Remaining: {remainingPieces})");
            }
            else
            {
                DebugLogger.LogWarningProceduralGeneration($"❌ Rejected poor connection: Distance={connectionDistance:F3}m, Angle={connectionAngle:F1}° between {fromConnector.name} and {toConnector.name}");
                DebugLogger.LogWarningProceduralGeneration($"   Thresholds: Distance must be ≤{maxDistance}m, Angle must be ≤{maxAngle}°");
                DestroyImmediate(wrapper);
                return null;
            }
        }
        
        // Check for loops if loop prevention is enabled (but skip if forcing cave length)
        if (!settings.allowLoops && !needToForceConnection && WouldCreateLoop(fromConnector, wrapper.transform.position))
        {
            DebugLogger.LogProceduralGeneration($"🔄 Loop prevention: Rejected connection from {fromConnector.name} to prevent cave loop");
            DestroyImmediate(wrapper);
            return null;
        }
        else if (!settings.allowLoops && needToForceConnection && WouldCreateLoop(fromConnector, wrapper.transform.position))
        {
            DebugLogger.LogWarningProceduralGeneration($"🔄 Loop allowed to force cave length (Remaining: {remainingPieces})");
        }
        
        // Check for overlaps after alignment (but be more lenient when forcing cave length)
        if (settings.preventOverlaps && !needToForceConnection && CheckForOverlapExcluding(wrapper.transform.position, settings.overlapCheckRadius, fromConnector))
        {
            DebugLogger.LogProceduralGeneration($"Prevented overlap after alignment at {wrapper.transform.position}");
            DestroyImmediate(wrapper);
            return null;
        }
        else if (settings.preventOverlaps && needToForceConnection && CheckForOverlapExcluding(wrapper.transform.position, settings.overlapCheckRadius, fromConnector))
        {
            DebugLogger.LogWarningProceduralGeneration($"⚠️ Overlap allowed to force cave length (Remaining: {remainingPieces})");
        }
        
        // Create the node
        var node = new CavePieceNode
        {
            piece = wrapper,
            position = wrapper.transform.position,
            rotation = wrapper.transform.rotation,
            connectors = tempInstance.GetComponentsInChildren<Transform>()
                .Where(t => t.name.StartsWith("cave_connector_"))
                .ToList(),
            depth = depth,
            isDeadEnd = deadEnds.Contains(prefab)
        };
        
        generatedPieces.Add(node);
        occupiedPositions.Add(wrapper.transform.position);
        
        // CRITICAL FIX: Properly establish bidirectional connection between connectors
        // Mark fromConnector as used and connected to toConnector
        if (connectorData.ContainsKey(fromConnector))
        {
            connectorData[fromConnector].isUsed = true;
            connectorData[fromConnector].connectedTo = toConnector;
        }
        
        // Mark toConnector as used and connected to fromConnector
        connectorData[toConnector] = new ConnectorInfo
        {
            connector = toConnector,
            type = "default",
            isUsed = true, // This connector is now connected
            direction = toConnector.forward,
            connectedTo = fromConnector
        };
        
        // Add remaining connectors to open list (excluding the one we just used)
        foreach (var connector in node.connectors)
        {
            if (connector != toConnector) // Don't add the connector we just used
            {
                openConnectors.Add(connector);
                connectorData[connector] = new ConnectorInfo
                {
                    connector = connector,
                    type = "default",
                    isUsed = false, // These are available for future connections
                    direction = connector.forward
                };
            }
        }
        
        currentIndex++;
        DebugLogger.LogProceduralGeneration($"🔗 CONNECTED: {prefab.name} via {fromConnector.name}[{GetCavePieceFromConnector(fromConnector)?.name}] ↔ {toConnector.name}[{wrapper.name}]");
        
        return node;
    }
    
    void AlignCavePieces(GameObject wrapper, Transform fromConnector, Transform toConnector)
    {
        DebugLogger.LogProceduralGeneration($"🔧 ALIGNING: {wrapper.name} to connect {fromConnector.name}[{fromConnector.position}] ↔ {toConnector.name}[{toConnector.position}]");
        
        // Step 1: Detection - Convert connector directions to cardinal labels
        string fromDirection = GetCardinalDirection(fromConnector.forward);
        string toDirection = GetCardinalDirection(toConnector.forward);
        
        DebugLogger.LogProceduralGeneration($"   From connector facing: {fromDirection}, To connector facing: {toDirection}");
        
        // Step 2: Calculate required rotation using cardinal directions
        float fromAngle = GetAngleFromCardinal(fromDirection);
        float goalAngle = (fromAngle + 180f) % 360f; // Opposite direction
        float startAngle = GetAngleFromCardinal(toDirection);
        float requiredRotation = (goalAngle - startAngle + 360f) % 360f;
        
        // Normalize to shortest rotation (-180 to +180)
        if (requiredRotation > 180f)
            requiredRotation -= 360f;
            
        DebugLogger.LogProceduralGeneration($"   From angle: {fromAngle}°, Goal angle: {goalAngle}°, Start angle: {startAngle}°");
        DebugLogger.LogProceduralGeneration($"   Required Y rotation: {requiredRotation}°");
        
        // Step 3: Apply rotation to wrapper
        wrapper.transform.Rotate(0, requiredRotation, 0, Space.World);
        
        // Step 4: Position - Snap connectors together
        Vector3 positionOffset = fromConnector.position - toConnector.position;
        wrapper.transform.position += positionOffset;
        
        // Verify alignment
        float finalDistance = Vector3.Distance(fromConnector.position, toConnector.position);
        float finalAngle = Vector3.Angle(fromConnector.forward, -toConnector.forward);
        
        string qualityMsg = finalDistance < 0.1f && finalAngle < 5f ? "✅ PERFECT" : 
                           finalDistance < 1f && finalAngle < 30f ? "✅ GOOD" : "❌ POOR";
        
        DebugLogger.LogProceduralGeneration($"   {qualityMsg} Final Distance: {finalDistance:F3}m, Angle: {finalAngle:F1}°");
    }
    
    string GetCardinalDirection(Vector3 direction)
    {
        // Project to XZ plane and normalize
        Vector3 flatDir = new Vector3(direction.x, 0, direction.z).normalized;
        
        // Compare to cardinal directions and find the closest
        float dotNorth = Vector3.Dot(flatDir, Vector3.forward);   // (0, 0, 1)
        float dotEast = Vector3.Dot(flatDir, Vector3.right);      // (1, 0, 0) 
        float dotSouth = Vector3.Dot(flatDir, Vector3.back);      // (0, 0, -1)
        float dotWest = Vector3.Dot(flatDir, Vector3.left);       // (-1, 0, 0)
        
        float maxDot = Mathf.Max(dotNorth, dotEast, dotSouth, dotWest);
        
        if (maxDot == dotNorth) return "TOP";
        if (maxDot == dotEast) return "RIGHT"; 
        if (maxDot == dotSouth) return "BOTTOM";
        return "LEFT";
    }
    
    float GetAngleFromCardinal(string cardinal)
    {
        return cardinal switch
        {
            "TOP" => 0f,      // North
            "RIGHT" => 90f,   // East
            "BOTTOM" => 180f, // South
            "LEFT" => 270f,   // West
            _ => 0f
        };
    }
    
    IEnumerator GenerateCaveCoroutine()
    {
        if (settings.enableBranching)
        {
            yield return GenerateBranchingCave();
        }
        else
        {
            yield return GenerateLinearCave();
        }
    }
    
    IEnumerator GenerateBranchingCave()
    {
        var generationQueue = new Queue<(Transform connector, int depth)>();
        
        // Add all starting connectors to the queue for branching
        foreach (var connector in openConnectors.ToList())
        {
            generationQueue.Enqueue((connector, 1));
        }
        
        int piecesGenerated = 1; // First piece already placed
        
        while (piecesGenerated < settings.caveLength && generationQueue.Count > 0)
        {
            var (fromConnector, depth) = generationQueue.Dequeue();
            
            // Skip if this connector is already used or depth is too high
            if (connectorData.ContainsKey(fromConnector) && connectorData[fromConnector].isUsed)
            {
                DebugLogger.LogProceduralGeneration($"⏭️ Skipping connector {fromConnector.name} - already used");
                continue;
            }
            if (depth > settings.maxDepth && !settings.forceCaveLength)
            {
                DebugLogger.LogProceduralGeneration($"⏭️ Skipping connector {fromConnector.name} - depth {depth} exceeds max {settings.maxDepth}");
                continue;
            }
            else if (depth > settings.maxDepth && settings.forceCaveLength)
            {
                DebugLogger.LogProceduralGeneration($"📏 Continuing despite depth {depth} > {settings.maxDepth} (forcing cave length)");
            }
            
            DebugLogger.LogProceduralGeneration($"🔗 Attempting to connect piece {piecesGenerated + 1} to connector {fromConnector.name} at depth {depth}");
            
            // Choose piece type based on depth and branching settings
            var prefabsToChooseFrom = new List<GameObject>();
            
            if ((depth < settings.maxDepth || settings.forceCaveLength) && Random.value < settings.branchProbability)
            {
                // Use tunnel pieces for branching (ignore depth limit if forcing cave length)
                prefabsToChooseFrom = validPrefabs.Where(p => !deadEnds.Contains(p)).ToList();
                DebugLogger.LogProceduralGeneration(settings.forceCaveLength && depth >= settings.maxDepth ? 
                    "🌿 Attempting to create branch (ignoring depth limit)" : 
                    "🌿 Attempting to create branch");
            }
            else if ((piecesGenerated >= settings.caveLength - 2 && !settings.forceCaveLength) || 
                     (depth >= settings.maxDepth && !settings.forceCaveLength) ||
                     (piecesGenerated >= settings.caveLength - 1 && settings.forceCaveLength))
            {
                // Use end caps for finishing
                prefabsToChooseFrom = deadEnds;
                if (settings.forceCaveLength && piecesGenerated >= settings.caveLength - 1)
                {
                    DebugLogger.LogProceduralGeneration("🏁 Using end cap pieces (target length nearly reached)");
                }
                else
                {
                    DebugLogger.LogProceduralGeneration("🏁 Using end cap pieces");
                }
            }
            else
            {
                // Use any piece (but prefer non-end caps if forcing cave length and pieces remaining)
                if (settings.forceCaveLength && piecesGenerated < settings.caveLength - 1)
                {
                    prefabsToChooseFrom = validPrefabs.Where(p => !deadEnds.Contains(p)).ToList();
                    if (prefabsToChooseFrom.Count == 0)
                    {
                        prefabsToChooseFrom = validPrefabs; // Fallback to any piece
                    }
                    DebugLogger.LogProceduralGeneration($"🎯 Preferring non-end caps to reach target length (Remaining: {settings.caveLength - piecesGenerated})");
                }
                else
                {
                    prefabsToChooseFrom = validPrefabs;
                }
            }
            
            if (prefabsToChooseFrom.Count == 0) 
            {
                DebugLogger.LogWarningProceduralGeneration("❌ No valid prefabs available!");
                continue;
            }
            
            var chosenPrefab = GetWeightedRandomPrefab(prefabsToChooseFrom);
            if (chosenPrefab == null) 
            {
                DebugLogger.LogWarningProceduralGeneration("❌ GetWeightedRandomPrefab returned null!");
                continue;
            }
            
            DebugLogger.LogProceduralGeneration($"🎲 Chosen prefab: {chosenPrefab.name}");
            
            // Mark this connector as used BEFORE attempting connection
            if (connectorData.ContainsKey(fromConnector))
            {
                connectorData[fromConnector].isUsed = true;
            }
            
            // Create and align the new piece
            int remainingPieces = settings.caveLength - piecesGenerated;
            var newNode = ConnectCavePiece(chosenPrefab, fromConnector, depth, remainingPieces);
            
            if (newNode != null)
            {
                piecesGenerated++;
                DebugLogger.LogProceduralGeneration($"✅ Successfully connected piece {piecesGenerated} at depth {depth}");
                
                // Add new connectors to queue (except the one we just used)
                foreach (var connector in newNode.connectors)
                {
                    if (connectorData.ContainsKey(connector) && !connectorData[connector].isUsed)
                    {
                        generationQueue.Enqueue((connector, depth + 1));
                    }
                }
                
                if (settings.realtimePreview && settings.generationDelay > 0)
                {
                    yield return new EditorWaitForSeconds(settings.generationDelay);
                }
            }
            else
            {
                DebugLogger.LogErrorProceduralGeneration($"❌ DETAILED FAILURE: ConnectCavePiece returned null for prefab {chosenPrefab.name} at connector {fromConnector.name} (depth {depth})");
                DebugLogger.LogErrorProceduralGeneration($"   - Connector position: {fromConnector.position}");
                DebugLogger.LogErrorProceduralGeneration($"   - Connector direction: {fromConnector.forward}");
                DebugLogger.LogErrorProceduralGeneration($"   - Check previous logs for specific failure reason (validation, overlap, etc.)");
                
                // If connection failed, mark connector as unused again
                if (connectorData.ContainsKey(fromConnector))
                {
                    connectorData[fromConnector].isUsed = false;
                }
            }
        }
        
        FinalizeCaveGeneration(piecesGenerated);
    }
    
    IEnumerator GenerateLinearCave()
    {
        DebugLogger.LogProceduralGeneration("🚶 Generating LINEAR cave (no branching)");
        
        int piecesGenerated = 1; // First piece already placed
        Transform currentConnector = null;
        
        // Pick a random starting connector from the first piece
        var availableConnectors = openConnectors.Where(c => 
            connectorData.ContainsKey(c) && !connectorData[c].isUsed).ToList();
        
        if (availableConnectors.Count > 0)
        {
            currentConnector = availableConnectors[Random.Range(0, availableConnectors.Count)];
        }
        
        while (piecesGenerated < settings.caveLength && currentConnector != null)
        {
            DebugLogger.LogProceduralGeneration($"🔗 Linear connection {piecesGenerated + 1} from connector {currentConnector.name}");
            
            // Choose piece type - prefer tunnel pieces for continuation
            var prefabsToChooseFrom = new List<GameObject>();
            
            if (piecesGenerated >= settings.caveLength)
            {
                // Force end cap if we've reached or exceeded the exact target length
                prefabsToChooseFrom = deadEnds;
                DebugLogger.LogProceduralGeneration("🏁 Using end cap - target length reached");
            }
            else if (piecesGenerated >= settings.caveLength - 1 && !settings.forceCaveLength)
            {
                // Use end caps for the final piece (unless forcing cave length)
                prefabsToChooseFrom = deadEnds;
                DebugLogger.LogProceduralGeneration("🏁 Using end cap for final piece");
            }
            else
            {
                // Use tunnel pieces to continue the linear path
                prefabsToChooseFrom = validPrefabs.Where(p => !deadEnds.Contains(p)).ToList();
                if (prefabsToChooseFrom.Count == 0)
                    prefabsToChooseFrom = validPrefabs; // Fallback to any piece
            }
            
            if (prefabsToChooseFrom.Count == 0) 
            {
                DebugLogger.LogWarningProceduralGeneration("❌ No valid prefabs available for linear generation!");
                break;
            }
            
            var chosenPrefab = GetWeightedRandomPrefab(prefabsToChooseFrom);
            if (chosenPrefab == null) 
            {
                DebugLogger.LogWarningProceduralGeneration("❌ GetWeightedRandomPrefab returned null!");
                break;
            }
            
            DebugLogger.LogProceduralGeneration($"🎲 Linear chosen prefab: {chosenPrefab.name}");
            
            // Mark current connector as used
            if (connectorData.ContainsKey(currentConnector))
            {
                connectorData[currentConnector].isUsed = true;
            }
            
            // Create and align the new piece
            int remainingPieces = settings.caveLength - piecesGenerated;
            var newNode = ConnectCavePiece(chosenPrefab, currentConnector, piecesGenerated, remainingPieces);
            
            if (newNode != null)
            {
                piecesGenerated++;
                DebugLogger.LogProceduralGeneration($"✅ Linear piece {piecesGenerated} connected successfully");
                
                // For linear caves, pick ONE random unused connector from the new piece
                var newConnectors = newNode.connectors.Where(c => 
                    connectorData.ContainsKey(c) && !connectorData[c].isUsed).ToList();
                
                if (newConnectors.Count > 0)
                {
                    currentConnector = newConnectors[Random.Range(0, newConnectors.Count)];
                    DebugLogger.LogProceduralGeneration($"🎯 Next linear connector: {currentConnector.name}");
                }
                else
                {
                    currentConnector = null; // No more connectors, end generation
                    DebugLogger.LogProceduralGeneration("🛑 No more available connectors, ending linear generation");
                }
                
                if (settings.realtimePreview && settings.generationDelay > 0)
                {
                    yield return new EditorWaitForSeconds(settings.generationDelay);
                }
            }
            else
            {
                DebugLogger.LogErrorProceduralGeneration($"❌ Linear connection failed for {chosenPrefab.name}");
                // If connection failed, mark connector as unused again
                if (connectorData.ContainsKey(currentConnector))
                {
                    connectorData[currentConnector].isUsed = false;
                }
                break;
            }
        }
        
        FinalizeCaveGeneration(piecesGenerated);
    }
    
    void FinalizeCaveGeneration(int piecesGenerated)
    {
        // Cap remaining open ends if enabled
        if (settings.capOpenEnds)
        {
            // Find all unused connectors across all pieces
            var connectorsToCap = new List<Transform>();
            foreach (var connector in connectorData.Keys)
            {
                if (!connectorData[connector].isUsed)
                {
                    connectorsToCap.Add(connector);
                }
            }
            
            DebugLogger.LogProceduralGeneration($"🧢 Capping {connectorsToCap.Count} unused connectors");
            
            foreach (var connector in connectorsToCap)
            {
                if (deadEnds.Count > 0)
                {
                    var deadEndPrefab = GetWeightedRandomPrefab(deadEnds);
                    if (deadEndPrefab != null)
                    {
                        DebugLogger.LogProceduralGeneration($"🧢 Adding end cap to unused connector {connector.name} at {connector.position}");
                        var newNode = ConnectCavePiece(deadEndPrefab, connector, 0, 0);
                        if (newNode != null)
                        {
                            piecesGenerated++;
                        }
                    }
                }
            }
        }
        
        DebugLogger.LogProceduralGeneration($"✅ Cave generation complete! Generated {piecesGenerated} pieces with seed: {lastGenerationSeed}");
        
        // Auto-record debug snapshot if enabled
        if (autoRecordOnGeneration)
        {
            RecordDebugSnapshot($"Auto-recorded after generation (Seed: {lastGenerationSeed})");
            // Save this as the original generation for later comparison
            originalGenerationSnapshot = currentSnapshot;
            DebugLogger.LogProceduralGeneration("💾 Saved original generation snapshot for debugging comparisons");
        }
    }
    
    // Keep essential methods from original implementation
    GameObject InstantiateCavePiece(GameObject source, Transform parent)
    {
        GameObject instance;
        if (settings.useEggFiles)
        {
            instance = Instantiate(source, parent);
        }
        else
        {
            instance = (GameObject)PrefabUtility.InstantiatePrefab(source, parent);
        }
        
        // Clean the name for exporter compatibility - remove Unity suffixes
        if (instance.name.EndsWith("(Clone)"))
        {
            instance.name = instance.name.Replace("(Clone)", "");
        }
        if (instance.name.EndsWith(" Instance"))
        {
            instance.name = instance.name.Replace(" Instance", "");
        }
        
        return instance;
    }
    
    GameObject GetWeightedRandomPrefab(List<GameObject> prefabs)
    {
        var validOptions = prefabs
            .Where(p => prefabToggles.ContainsKey(p) && prefabToggles[p] && prefabLikelihoods[p] > 0)
            .ToList();
        
        if (validOptions.Count == 0) return null;
        
        int totalWeight = validOptions.Sum(p => prefabLikelihoods[p]);
        int randomPoint = Random.Range(0, totalWeight);
        
        foreach (var prefab in validOptions)
        {
            int weight = prefabLikelihoods[prefab];
            if (randomPoint < weight) return prefab;
            randomPoint -= weight;
        }
        
        return validOptions.LastOrDefault();
    }
    
    void ClearCave()
    {
        if (root != null)
        {
            DestroyImmediate(root);
            root = null;
        }
        
        openConnectors.Clear();
        occupiedPositions.Clear();
        connectorData.Clear();
        generatedPieces.Clear();
        currentIndex = 1;
    }
    
    void RegenerateLastCave()
    {
        if (generationHistory.Count > 0)
        {
            var lastSettings = generationHistory.Peek();
            JsonUtility.FromJsonOverwrite(lastSettings, settings);
            GenerateCaveWithEnhancements();
        }
    }
    
    void ExportCaveAsPrefab()
    {
        if (root == null)
        {
            DebugLogger.LogErrorProceduralGeneration("No cave to export!");
            return;
        }
        
        string path = EditorUtility.SaveFilePanel("Export Cave", "Assets/", "GeneratedCave", "prefab");
        if (!string.IsNullOrEmpty(path))
        {
            var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            if (prefab != null)
            {
                DebugLogger.LogProceduralGeneration($"Cave exported as prefab: {path}");
                EditorGUIUtility.PingObject(prefab);
            }
        }
    }
    
    // Load prefabs implementation (cleaned up from original)
    void LoadAllPrefabs()
    {
        allFoundPrefabs = new List<GameObject>();
        
        if (settings.useEggFiles)
        {
            string cavePath = "Assets/Resources/phase_4/models/caves/";
            if (Directory.Exists(cavePath))
            {
                string[] eggFiles = Directory.GetFiles(cavePath, "*.egg", SearchOption.AllDirectories);
                foreach (string eggPath in eggFiles)
                {
                    string assetPath = eggPath.Replace("\\", "/");
                    GameObject eggAsset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    if (eggAsset != null) allFoundPrefabs.Add(eggAsset);
                }
            }
        }
        else
        {
            string cavePath = "Assets/Resources/phase_4/models/caves/";
            if (Directory.Exists(cavePath))
            {
                string[] prefabFiles = Directory.GetFiles(cavePath, "*.prefab", SearchOption.AllDirectories);
                foreach (string prefabPath in prefabFiles)
                {
                    string assetPath = prefabPath.Replace("\\", "/");
                    GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    if (prefabAsset != null) allFoundPrefabs.Add(prefabAsset);
                }
            }
        }
        
        // Categorize by connector count
        categorizedPrefabs.Clear();
        foreach (var prefab in allFoundPrefabs)
        {
            int connectorCount = prefab.GetComponentsInChildren<Transform>(true)
                .Count(t => t.name.StartsWith("cave_connector_"));
            
            if (connectorCount == 0) continue;
            
            if (!categorizedPrefabs.ContainsKey(connectorCount))
                categorizedPrefabs[connectorCount] = new List<GameObject>();
            
            categorizedPrefabs[connectorCount].Add(prefab);
            
            if (!prefabToggles.ContainsKey(prefab)) prefabToggles[prefab] = true;
            if (!prefabLikelihoods.ContainsKey(prefab)) prefabLikelihoods[prefab] = 100;
            if (!prefabTags.ContainsKey(prefab)) prefabTags[prefab] = "";
        }
        
        DebugLogger.LogProceduralGeneration($"Loaded {allFoundPrefabs.Count} {(settings.useEggFiles ? ".egg files" : "prefabs")} for cave generation");
    }
    
    // Preset save/load (simplified from original)
    void SaveSelections()
    {
        var presetData = new CavePresetData();
        presetData.settings = settings;
        
        foreach (var kvp in prefabToggles)
        {
            string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(kvp.Key));
            int likelihood = prefabLikelihoods.ContainsKey(kvp.Key) ? prefabLikelihoods[kvp.Key] : 100;
            presetData.selections.Add(new SelectionEntry { guid = guid, isEnabled = kvp.Value, likelihood = likelihood });
        }
        
        string json = JsonUtility.ToJson(presetData, true);
        string path = EditorUtility.SaveFilePanel("Save Cave Preset", "Assets/Editor/Cave Generator/Cave_Presets/", "cave_preset.json", "json");
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, json);
            LoadCavePresetList(); // Refresh the cave preset list
        }
    }
    
    void LoadSelections()
    {
        string path = EditorUtility.OpenFilePanel("Load Cave Preset", "Assets/Editor/Cave Generator/Cave_Presets/", "json");
        if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;
        
        LoadPresetFromPath(path);
    }
    
    void LoadPresetFromPath(string path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;
        
        string json = File.ReadAllText(path);
        
        // Try to load as new format first
        try
        {
            var presetData = JsonUtility.FromJson<CavePresetData>(json);
            if (presetData.settings != null)
            {
                // Load settings
                settings = presetData.settings;
                
                // Load selections
                if (allFoundPrefabs == null) LoadAllPrefabs();
                
                var guidToDataMap = presetData.selections.ToDictionary(e => e.guid);
                
                foreach (var prefab in allFoundPrefabs)
                {
                    string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(prefab));
                    if (guidToDataMap.TryGetValue(guid, out var entry))
                    {
                        prefabToggles[prefab] = entry.isEnabled;
                        prefabLikelihoods[prefab] = entry.likelihood;
                    }
                }
                
                DebugLogger.LogProceduralGeneration($"Loaded preset with settings: {Path.GetFileNameWithoutExtension(path)}");
                return;
            }
        }
        catch { }
        
        // Fall back to old format
        try
        {
            var data = JsonUtility.FromJson<SelectionData>(json);
            
            if (allFoundPrefabs == null) LoadAllPrefabs();
            
            var guidToDataMap = data.entries.ToDictionary(e => e.guid);
            
            foreach (var prefab in allFoundPrefabs)
            {
                string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(prefab));
                if (guidToDataMap.TryGetValue(guid, out var entry))
                {
                    prefabToggles[prefab] = entry.isEnabled;
                    prefabLikelihoods[prefab] = entry.likelihood;
                }
            }
            
            DebugLogger.LogProceduralGeneration($"Loaded legacy preset (selections only): {Path.GetFileNameWithoutExtension(path)}");
        }
        catch
        {
            DebugLogger.LogErrorProceduralGeneration($"Failed to load preset: {path}");
        }
    }
    
    // Debug functionality
    void RecordDebugSnapshot(string notes = "")
    {
        if (root == null) return;
        
        var snapshot = new DebugSnapshot
        {
            timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            settings = JsonUtility.FromJson<GenerationSettings>(JsonUtility.ToJson(settings)),
            notes = notes
        };
        
        // Record piece data
        foreach (var piece in generatedPieces)
        {
            var debugPiece = new DebugPieceData
            {
                pieceName = piece.piece.name,
                position = piece.position,
                rotation = piece.rotation.eulerAngles
            };
            
            foreach (var connector in piece.connectors)
            {
                debugPiece.connectorNames.Add(connector.name);
                debugPiece.connectorPositions.Add(connector.position);
                debugPiece.connectorDirections.Add(connector.forward);
            }
            
            snapshot.pieces.Add(debugPiece);
        }
        
        // Record connection data
        foreach (var connectorInfo in connectorData.Values)
        {
            if (connectorInfo.isUsed && connectorInfo.connectedTo != null)
            {
                var fromPiece = GetCavePieceFromConnector(connectorInfo.connector);
                var toPiece = GetCavePieceFromConnector(connectorInfo.connectedTo);
                
                var connection = new DebugConnectionData
                {
                    fromPiece = fromPiece?.name ?? "unknown",
                    toPiece = toPiece?.name ?? "unknown",
                    fromConnector = connectorInfo.connector.name,
                    toConnector = connectorInfo.connectedTo.name,
                    fromPosition = connectorInfo.connector.position,
                    toPosition = connectorInfo.connectedTo.position,
                    fromRotation = connectorInfo.connector.eulerAngles,
                    toRotation = connectorInfo.connectedTo.eulerAngles,
                    fromDirection = connectorInfo.connector.forward,
                    toDirection = connectorInfo.connectedTo.forward
                };
                
                connection.connectionDistance = Vector3.Distance(connection.fromPosition, connection.toPosition);
                connection.angleDifference = Vector3.Angle(connection.fromDirection, -connection.toDirection);
                // Realistic thresholds for good cave connections
                // Distance: within 1 unit is good, up to 2 units is acceptable
                // Angle: within 30 degrees is good alignment
                connection.isCorrectlyAligned = connection.connectionDistance < 1.0f && connection.angleDifference < 30f;
                
                snapshot.connections.Add(connection);
            }
        }
        
        previousSnapshot = currentSnapshot;
        currentSnapshot = snapshot;
        
        DebugLogger.LogProceduralGeneration($"🔍 Debug snapshot recorded at {snapshot.timestamp}");
    }
    
    void AnalyzeExistingCave()
    {
        // Find cave system in scene
        GameObject[] caveRoots = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None)
            .Where(go => go.name.Contains("Cave") && go.transform.parent == null)
            .ToArray();
            
        if (caveRoots.Length == 0)
        {
            DebugLogger.LogWarningProceduralGeneration("No cave system found in scene! Make sure your cave root object has 'Cave' in its name.");
            return;
        }
        
        if (caveRoots.Length > 1)
        {
            DebugLogger.LogWarningProceduralGeneration($"Multiple cave systems found. Analyzing: {caveRoots[0].name}");
        }
        
        GameObject caveRoot = caveRoots[0];
        DebugLogger.LogProceduralGeneration($"🔍 Analyzing existing cave system: {caveRoot.name}");
        
        // Clear and rebuild data structures
        generatedPieces.Clear();
        connectorData.Clear();
        
        // Find only the CavePiece wrapper objects (not all children)
        var cavePieceWrappers = new List<Transform>();
        
        // Look for direct children that are CavePiece wrappers
        for (int i = 0; i < caveRoot.transform.childCount; i++)
        {
            Transform child = caveRoot.transform.GetChild(i);
            if (child.name.StartsWith("CavePiece_"))
            {
                cavePieceWrappers.Add(child);
            }
        }
        
        DebugLogger.LogProceduralGeneration($"🔍 Found {cavePieceWrappers.Count} CavePiece wrappers");
        
        foreach (var pieceTransform in cavePieceWrappers)
        {
            var connectors = pieceTransform.GetComponentsInChildren<Transform>()
                .Where(t => t.name.StartsWith("cave_connector_"))
                .ToList();
                
            DebugLogger.LogProceduralGeneration($"   Piece {pieceTransform.name}: {connectors.Count} connectors");
                
            if (connectors.Count > 0)
            {
                var node = new CavePieceNode
                {
                    piece = pieceTransform.gameObject,
                    position = pieceTransform.position,
                    rotation = pieceTransform.rotation,
                    connectors = connectors,
                    depth = 0,
                    isDeadEnd = connectors.Count == 1
                };
                
                generatedPieces.Add(node);
                
                // Add connectors to data
                foreach (var connector in connectors)
                {
                    connectorData[connector] = new ConnectorInfo
                    {
                        connector = connector,
                        type = "default",
                        isUsed = false,
                        direction = connector.forward
                    };
                }
            }
        }
        
        // Analyze connections
        AnalyzeConnections();
        
        // Record snapshot
        RecordDebugSnapshot($"Scene analysis of {caveRoot.name}");
        
        DebugLogger.LogProceduralGeneration($"✅ Analysis complete! Found {generatedPieces.Count} cave pieces with {connectorData.Count} connectors");
    }
    
    void GetAllChildren(Transform parent, List<Transform> children)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            children.Add(child);
            GetAllChildren(child, children);
        }
    }
    
    void AnalyzeConnections()
    {
        // Clear previous connection data
        foreach (var connectorInfo in connectorData.Values)
        {
            connectorInfo.isUsed = false;
            connectorInfo.connectedTo = null;
        }
        
        var connectorList = connectorData.Keys.ToList();
        
        for (int i = 0; i < connectorList.Count; i++)
        {
            var connector1 = connectorList[i];
            var piece1 = GetCavePieceFromConnector(connector1);
            
            for (int j = i + 1; j < connectorList.Count; j++)
            {
                var connector2 = connectorList[j];
                var piece2 = GetCavePieceFromConnector(connector2);
                
                // Skip if connectors are on the same cave piece
                if (piece1 == piece2)
                    continue;
                
                // Check if connectors are close enough to be considered connected
                float distance = Vector3.Distance(connector1.position, connector2.position);
                
                // Use a tight threshold for connection detection (within 0.5 units = properly aligned)
                if (distance < 0.5f)
                {
                    // Check if they're facing roughly opposite directions (should be connecting)
                    float angleDiff = Vector3.Angle(connector1.forward, -connector2.forward);
                    
                    // Tight angle threshold for good connections
                    if (angleDiff < 30f)
                    {
                        // Mark as connected
                        connectorData[connector1].isUsed = true;
                        connectorData[connector1].connectedTo = connector2;
                        connectorData[connector2].isUsed = true;
                        connectorData[connector2].connectedTo = connector1;
                        
                        DebugLogger.LogProceduralGeneration($"✅ REAL CONNECTION: {piece1?.name}[{connector1.name}] ↔ {piece2?.name}[{connector2.name}] (Distance: {distance:F3}m, Angle: {angleDiff:F1}°)");
                    }
                    else
                    {
                        DebugLogger.LogProceduralGeneration($"❌ Poor alignment: {piece1?.name}[{connector1.name}] ↔ {piece2?.name}[{connector2.name}] (Distance: {distance:F3}m, Angle: {angleDiff:F1}°)");
                    }
                }
            }
        }
    }
    
    Transform GetCavePieceFromConnector(Transform connector)
    {
        // Walk up the hierarchy to find the CavePiece_ wrapper
        Transform current = connector;
        while (current != null)
        {
            // Look specifically for the CavePiece_ wrapper
            if (current.name.StartsWith("CavePiece_"))
            {
                return current;
            }
            current = current.parent;
        }
        return null; // Couldn't find cave piece wrapper
    }
    
    void DrawDebugTab()
    {
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Cave Generation Debugging", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        autoRecordOnGeneration = EditorGUILayout.Toggle("Auto-record on generation", autoRecordOnGeneration);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🔧 Fixed Caves", GUILayout.Height(30), GUILayout.MinWidth(120)))
        {
            AnalyzeFixedCaves();
        }
        
        if (GUILayout.Button("📋 Copy Connection Data", GUILayout.Height(30)))
        {
            CopyConnectionDataToClipboard();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("📝 Copy Fix Summary", GUILayout.Height(30)))
        {
            CopyFixSummaryToClipboard();
        }
        
        if (GUILayout.Button("💾 Export Debug Data", GUILayout.Height(30)))
        {
            ExportDebugData();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
        
        if (currentSnapshot != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label($"Current Snapshot: {currentSnapshot.timestamp}", EditorStyles.boldLabel);
            
            debugScroll = EditorGUILayout.BeginScrollView(debugScroll);
            
            // Cave pieces summary
            EditorGUILayout.LabelField("Cave Pieces", EditorStyles.boldLabel);
            foreach (var piece in currentSnapshot.pieces)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField($"🧩 {piece.pieceName}", EditorStyles.miniBoldLabel);
                EditorGUILayout.LabelField($"Position: {piece.position}");
                EditorGUILayout.LabelField($"Rotation: {piece.rotation}");
                EditorGUILayout.LabelField($"Connectors: {piece.connectorNames.Count}");
                
                for (int i = 0; i < piece.connectorNames.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"  • {piece.connectorNames[i]}", GUILayout.Width(120));
                    EditorGUILayout.LabelField($"Pos: {piece.connectorPositions[i].ToString("F2")}", GUILayout.Width(150));
                    EditorGUILayout.LabelField($"Dir: {piece.connectorDirections[i].ToString("F2")}");
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }
            
            // Connection analysis
            if (currentSnapshot.connections.Count > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Connection Analysis", EditorStyles.boldLabel);
                
                int correctConnections = 0;
                int incorrectConnections = 0;
                
                foreach (var connection in currentSnapshot.connections)
                {
                    var style = connection.isCorrectlyAligned ? EditorStyles.label : EditorStyles.boldLabel;
                    var color = connection.isCorrectlyAligned ? Color.green : Color.red;
                    
                    EditorGUILayout.BeginVertical("box");
                    
                    var oldColor = GUI.color;
                    GUI.color = color;
                    EditorGUILayout.LabelField($"🔗 {connection.fromPiece} → {connection.toPiece}", style);
                    GUI.color = oldColor;
                    
                    EditorGUILayout.LabelField($"From: {connection.fromConnector} | To: {connection.toConnector}");
                    EditorGUILayout.LabelField($"Distance: {connection.connectionDistance:F3} | Angle Diff: {connection.angleDifference:F1}°");
                    EditorGUILayout.LabelField($"From Dir: {connection.fromDirection.ToString("F2")}");
                    EditorGUILayout.LabelField($"To Dir: {connection.toDirection.ToString("F2")}");
                    
                    if (!connection.isCorrectlyAligned)
                    {
                        EditorGUILayout.BeginVertical("box");
                        EditorGUILayout.LabelField("⚠️ ALIGNMENT ISSUES:", EditorStyles.boldLabel);
                        if (connection.connectionDistance >= 0.1f)
                            EditorGUILayout.LabelField($"• Distance too large: {connection.connectionDistance:F3} (should be < 0.1)");
                        if (connection.angleDifference >= 5f)
                            EditorGUILayout.LabelField($"• Angle misalignment: {connection.angleDifference:F1}° (should be < 5°)");
                        EditorGUILayout.EndVertical();
                        incorrectConnections++;
                    }
                    else
                    {
                        correctConnections++;
                    }
                    
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                }
                
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField($"📊 Connection Summary", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Correct connections: {correctConnections}");
                EditorGUILayout.LabelField($"Incorrect connections: {incorrectConnections}");
                EditorGUILayout.LabelField($"Success rate: {(correctConnections * 100f / (correctConnections + incorrectConnections)):F1}%");
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.HelpBox("No debug data recorded. Generate a cave or click 'Record Current State' to begin debugging.", MessageType.Info);
        }
    }
    
    void ExportDebugData()
    {
        if (currentSnapshot == null)
        {
            DebugLogger.LogWarningProceduralGeneration("No debug data to export!");
            return;
        }
        
        string json = JsonUtility.ToJson(currentSnapshot, true);
        string path = EditorUtility.SaveFilePanel("Export Debug Data", "Assets/", $"cave_debug_{currentSnapshot.timestamp.Replace(":", "-").Replace(" ", "_")}.json", "json");
        
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, json);
            DebugLogger.LogProceduralGeneration($"Debug data exported to: {path}");
        }
    }
    
    void AnalyzeFixedCaves()
    {
        if (originalGenerationSnapshot == null)
        {
            DebugLogger.LogWarningProceduralGeneration("⚠️ No original generation data found! Generate a cave first, then fix it manually before using this feature.");
            return;
        }
        
        DebugLogger.LogProceduralGeneration("🔧 ANALYZING FIXED CAVES...");
        DebugLogger.LogProceduralGeneration("🔍 Comparing current cave state against original generation");
        
        // Capture current state of the scene
        AnalyzeExistingCave();
        
        if (currentSnapshot == null)
        {
            DebugLogger.LogWarningProceduralGeneration("❌ Could not analyze current cave state!");
            return;
        }
        
        // Compare with original generation
        DebugLogger.LogProceduralGeneration($"📊 CAVE FIX ANALYSIS RESULTS:");
        DebugLogger.LogProceduralGeneration($"Original Generation: {originalGenerationSnapshot.timestamp}");
        DebugLogger.LogProceduralGeneration($"Current Fixed State: {currentSnapshot.timestamp}");
        DebugLogger.LogProceduralGeneration($"");
        
        // Analyze pieces that were moved or rotated
        var movedPieces = new List<string>();
        var rotatedPieces = new List<string>();
        var significantChanges = 0;
        
        foreach (var currentPiece in currentSnapshot.pieces)
        {
            var originalPiece = originalGenerationSnapshot.pieces.FirstOrDefault(p => p.pieceName == currentPiece.pieceName);
            if (originalPiece != null)
            {
                float positionDiff = Vector3.Distance(originalPiece.position, currentPiece.position);
                float rotationDiff = Quaternion.Angle(
                    Quaternion.Euler(originalPiece.rotation), 
                    Quaternion.Euler(currentPiece.rotation)
                );
                
                if (positionDiff > 0.1f) // 10cm threshold
                {
                    movedPieces.Add($"  🔄 {currentPiece.pieceName}: moved {positionDiff:F2}m");
                    significantChanges++;
                }
                
                if (rotationDiff > 1.0f) // 1 degree threshold
                {
                    rotatedPieces.Add($"  🔄 {currentPiece.pieceName}: rotated {rotationDiff:F1}° (was {Vector3ToString(originalPiece.rotation)} → now {Vector3ToString(currentPiece.rotation)})");
                    significantChanges++;
                }
            }
        }
        
        // Report findings
        if (movedPieces.Count > 0)
        {
            DebugLogger.LogProceduralGeneration("📍 PIECES THAT WERE MOVED:");
            foreach (var piece in movedPieces)
            {
                DebugLogger.LogProceduralGeneration(piece);
            }
            DebugLogger.LogProceduralGeneration("");
        }
        
        if (rotatedPieces.Count > 0)
        {
            DebugLogger.LogProceduralGeneration("🔄 PIECES THAT WERE ROTATED:");
            foreach (var piece in rotatedPieces)
            {
                DebugLogger.LogProceduralGeneration(piece);
            }
            DebugLogger.LogProceduralGeneration("");
        }
        
        // Analyze connection improvements (filter out duplicate/self connections)
        var originalReal = originalGenerationSnapshot.connections.Where(c => 
            c.fromPiece != c.toPiece && c.fromPiece != "unknown" && c.toPiece != "unknown").ToList();
        var currentReal = currentSnapshot.connections.Where(c => 
            c.fromPiece != c.toPiece && c.fromPiece != "unknown" && c.toPiece != "unknown").ToList();
            
        int originalCorrect = originalReal.Count(c => c.isCorrectlyAligned);
        int currentCorrect = currentReal.Count(c => c.isCorrectlyAligned);
        int improvement = currentCorrect - originalCorrect;
        
        DebugLogger.LogProceduralGeneration($"🔗 CONNECTION ANALYSIS:");
        DebugLogger.LogProceduralGeneration($"  Original Real Connections: {originalReal.Count} (excluding duplicates/self-connections)");
        DebugLogger.LogProceduralGeneration($"  Current Real Connections: {currentReal.Count} (excluding duplicates/self-connections)");
        DebugLogger.LogProceduralGeneration($"  Original: {originalCorrect}/{originalReal.Count} correctly aligned ({(originalReal.Count > 0 ? originalCorrect * 100f / originalReal.Count : 0):F1}%)");
        DebugLogger.LogProceduralGeneration($"  Fixed: {currentCorrect}/{currentReal.Count} correctly aligned ({(currentReal.Count > 0 ? currentCorrect * 100f / currentReal.Count : 0):F1}%)");
        
        if (improvement > 0)
        {
            DebugLogger.LogProceduralGeneration($"  ✅ IMPROVEMENT: +{improvement} better connections!");
        }
        else if (improvement < 0)
        {
            DebugLogger.LogProceduralGeneration($"  ❌ REGRESSION: {improvement} worse connections");
        }
        else
        {
            DebugLogger.LogProceduralGeneration($"  ➡️ No change in connection quality");
        }
        
        // Show some problematic connections for context
        var badConnections = currentSnapshot.connections.Where(c => !c.isCorrectlyAligned).Take(3).ToList();
        if (badConnections.Count > 0)
        {
            DebugLogger.LogProceduralGeneration($"  📋 Sample problematic connections:");
            foreach (var conn in badConnections)
            {
                DebugLogger.LogProceduralGeneration($"    • {conn.fromPiece} ↔ {conn.toPiece}: {conn.connectionDistance:F2}m apart, {conn.angleDifference:F1}° angle diff");
            }
        }
        
        // Identify what was fixed
        var fixedConnections = new List<string>();
        foreach (var currentConn in currentSnapshot.connections.Where(c => c.isCorrectlyAligned))
        {
            var originalConn = originalGenerationSnapshot.connections.FirstOrDefault(c => 
                c.fromPiece == currentConn.fromPiece && c.toPiece == currentConn.toPiece);
            
            if (originalConn != null && !originalConn.isCorrectlyAligned)
            {
                fixedConnections.Add($"  ✅ {currentConn.fromPiece} ↔ {currentConn.toPiece}");
            }
        }
        
        if (fixedConnections.Count > 0)
        {
            DebugLogger.LogProceduralGeneration($"");
            DebugLogger.LogProceduralGeneration($"🔧 CONNECTIONS THAT WERE FIXED:");
            foreach (var conn in fixedConnections)
            {
                DebugLogger.LogProceduralGeneration(conn);
            }
        }
        
        if (movedPieces.Count == 0 && rotatedPieces.Count == 0)
        {
            DebugLogger.LogProceduralGeneration("✨ No pieces were moved or rotated - cave is identical to generation!");
        }
        
        DebugLogger.LogProceduralGeneration($"");
        DebugLogger.LogProceduralGeneration($"📋 SUMMARY: Made {significantChanges} manual adjustments ({movedPieces.Count} moved, {rotatedPieces.Count} rotated) to fix {fixedConnections.Count} connections");
    }
    
    string Vector3ToString(Vector3 v)
    {
        return $"({v.x:F1}, {v.y:F1}, {v.z:F1})";
    }
    
    void CopyConnectionDataToClipboard()
    {
        if (currentSnapshot == null)
        {
            DebugLogger.LogWarningProceduralGeneration("No current snapshot available. Analyze a cave first!");
            return;
        }
        
        var output = new System.Text.StringBuilder();
        output.AppendLine("=== CAVE CONNECTION DATA ===");
        output.AppendLine($"Timestamp: {currentSnapshot.timestamp}");
        output.AppendLine($"Total Pieces: {currentSnapshot.pieces.Count}");
        output.AppendLine($"Total Connections: {currentSnapshot.connections.Count}");
        output.AppendLine();
        
        output.AppendLine("--- PIECE POSITIONS/ROTATIONS ---");
        foreach (var piece in currentSnapshot.pieces)
        {
            output.AppendLine($"Piece: {piece.pieceName}");
            output.AppendLine($"  Position: {Vector3ToString(piece.position)}");
            output.AppendLine($"  Rotation: {Vector3ToString(piece.rotation)}");
            output.AppendLine($"  Connectors: {piece.connectorNames.Count}");
            for (int i = 0; i < piece.connectorNames.Count; i++)
            {
                output.AppendLine($"    {piece.connectorNames[i]}: Pos={Vector3ToString(piece.connectorPositions[i])}, Dir={Vector3ToString(piece.connectorDirections[i])}");
            }
            output.AppendLine();
        }
        
        output.AppendLine("--- CONNECTION ANALYSIS ---");
        int correctCount = 0;
        int duplicateCount = 0;
        foreach (var conn in currentSnapshot.connections)
        {
            // Check for suspicious duplicate connections (same piece connecting to itself or unknown pieces)
            bool isDuplicate = conn.fromPiece == conn.toPiece || 
                              conn.fromPiece == "unknown" || 
                              conn.toPiece == "unknown";
            
            string status;
            if (isDuplicate)
            {
                status = "🔄 DUPLICATE/SELF";
                duplicateCount++;
            }
            else if (conn.isCorrectlyAligned)
            {
                status = "✅ GOOD";
                correctCount++;
            }
            else
            {
                status = "❌ BAD";
            }
            
            output.AppendLine($"{status} {conn.fromPiece} ↔ {conn.toPiece}");
            output.AppendLine($"  Distance: {conn.connectionDistance:F3}m, Angle: {conn.angleDifference:F1}°");
            output.AppendLine($"  From[{conn.fromConnector}]: {Vector3ToString(conn.fromPosition)} Dir: {Vector3ToString(conn.fromDirection)}");
            output.AppendLine($"  To[{conn.toConnector}]: {Vector3ToString(conn.toPosition)} Dir: {Vector3ToString(conn.toDirection)}");
            output.AppendLine();
        }
        
        output.AppendLine($"--- SUMMARY ---");
        int realConnections = currentSnapshot.connections.Count - duplicateCount;
        output.AppendLine($"Real Cave Connections: {realConnections} (excluding {duplicateCount} duplicates/self-connections)");
        if (realConnections > 0)
        {
            output.AppendLine($"Good Connections: {correctCount}/{realConnections} ({(correctCount * 100f / realConnections):F1}%)");
        }
        else
        {
            output.AppendLine($"Good Connections: 0/0 (No real connections detected)");
        }
        
        string finalOutput = output.ToString();
        EditorGUIUtility.systemCopyBuffer = finalOutput;
        DebugLogger.LogProceduralGeneration("📋 Connection data copied to clipboard! Paste it in your message.");
        DebugLogger.LogProceduralGeneration($"Data size: {finalOutput.Length} characters");
    }
    
    void CopyFixSummaryToClipboard()
    {
        if (originalGenerationSnapshot == null || currentSnapshot == null)
        {
            DebugLogger.LogWarningProceduralGeneration("Need both original and current snapshots. Generate a cave, fix it, then analyze first!");
            return;
        }
        
        var output = new System.Text.StringBuilder();
        output.AppendLine("=== CAVE FIX SUMMARY ===");
        output.AppendLine($"Original Generation: {originalGenerationSnapshot.timestamp}");
        output.AppendLine($"After Manual Fixes: {currentSnapshot.timestamp}");
        output.AppendLine();
        
        // Track changes
        var changes = new List<string>();
        var significantChanges = 0;
        
        foreach (var currentPiece in currentSnapshot.pieces)
        {
            var originalPiece = originalGenerationSnapshot.pieces.FirstOrDefault(p => p.pieceName == currentPiece.pieceName);
            if (originalPiece != null)
            {
                float positionDiff = Vector3.Distance(originalPiece.position, currentPiece.position);
                float rotationDiff = Quaternion.Angle(
                    Quaternion.Euler(originalPiece.rotation), 
                    Quaternion.Euler(currentPiece.rotation)
                );
                
                if (positionDiff > 0.1f)
                {
                    changes.Add($"MOVED {currentPiece.pieceName}: {positionDiff:F2}m");
                    changes.Add($"  From: {Vector3ToString(originalPiece.position)}");
                    changes.Add($"  To: {Vector3ToString(currentPiece.position)}");
                    significantChanges++;
                }
                
                if (rotationDiff > 1.0f)
                {
                    changes.Add($"ROTATED {currentPiece.pieceName}: {rotationDiff:F1}°");
                    changes.Add($"  From: {Vector3ToString(originalPiece.rotation)}");
                    changes.Add($"  To: {Vector3ToString(currentPiece.rotation)}");
                    significantChanges++;
                }
            }
        }
        
        output.AppendLine("--- MANUAL CHANGES MADE ---");
        if (changes.Count > 0)
        {
            foreach (var change in changes)
            {
                output.AppendLine(change);
            }
        }
        else
        {
            output.AppendLine("No significant changes detected");
        }
        output.AppendLine();
        
        // Connection comparison
        int originalCorrect = originalGenerationSnapshot.connections.Count(c => c.isCorrectlyAligned);
        int currentCorrect = currentSnapshot.connections.Count(c => c.isCorrectlyAligned);
        
        output.AppendLine("--- CONNECTION QUALITY ---");
        output.AppendLine($"Before Fixes: {originalCorrect}/{originalGenerationSnapshot.connections.Count} good ({(originalCorrect * 100f / originalGenerationSnapshot.connections.Count):F1}%)");
        output.AppendLine($"After Fixes: {currentCorrect}/{currentSnapshot.connections.Count} good ({(currentCorrect * 100f / currentSnapshot.connections.Count):F1}%)");
        output.AppendLine($"Improvement: {currentCorrect - originalCorrect} connections");
        output.AppendLine();
        
        output.AppendLine($"--- SUMMARY ---");
        output.AppendLine($"Manual Adjustments: {significantChanges}");
        output.AppendLine($"Connection Improvement: {currentCorrect - originalCorrect}");
        
        string finalOutput = output.ToString();
        EditorGUIUtility.systemCopyBuffer = finalOutput;
        DebugLogger.LogProceduralGeneration("📝 Fix summary copied to clipboard! Paste it in your message.");
        DebugLogger.LogProceduralGeneration($"Summary size: {finalOutput.Length} characters");
    }
    
    void CompareSnapshots()
    {
        if (currentSnapshot == null || previousSnapshot == null)
        {
            DebugLogger.LogWarningProceduralGeneration("Need both current and previous snapshots to compare!");
            return;
        }
        
        DebugLogger.LogProceduralGeneration("🔍 SNAPSHOT COMPARISON:");
        DebugLogger.LogProceduralGeneration($"Previous: {previousSnapshot.timestamp} | Current: {currentSnapshot.timestamp}");
        
        // Compare piece counts
        DebugLogger.LogProceduralGeneration($"Pieces - Previous: {previousSnapshot.pieces.Count} | Current: {currentSnapshot.pieces.Count}");
        
        // Compare connection quality
        int prevCorrect = previousSnapshot.connections.Count(c => c.isCorrectlyAligned);
        int currCorrect = currentSnapshot.connections.Count(c => c.isCorrectlyAligned);
        
        DebugLogger.LogProceduralGeneration($"Correct Connections - Previous: {prevCorrect}/{previousSnapshot.connections.Count} | Current: {currCorrect}/{currentSnapshot.connections.Count}");
        
        // Identify problematic connections
        var problemConnections = currentSnapshot.connections.Where(c => !c.isCorrectlyAligned).ToList();
        if (problemConnections.Count > 0)
        {
            DebugLogger.LogProceduralGeneration("❌ PROBLEMATIC CONNECTIONS:");
            foreach (var conn in problemConnections)
            {
                DebugLogger.LogProceduralGeneration($"  • {conn.fromPiece} → {conn.toPiece}: Distance={conn.connectionDistance:F3}, Angle={conn.angleDifference:F1}°");
            }
        }
    }
}

[System.Serializable]
public class SelectionEntry
{
    public string guid;
    public bool isEnabled;
    public int likelihood;
}

[System.Serializable]
public class SelectionData
{
    public List<SelectionEntry> entries = new List<SelectionEntry>();
}

[System.Serializable]
public class CavePresetData
{
    public ProceduralCaveGenerator.GenerationSettings settings;
    public List<SelectionEntry> selections = new List<SelectionEntry>();
}