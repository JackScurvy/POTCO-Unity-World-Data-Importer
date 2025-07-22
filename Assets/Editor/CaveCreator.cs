using UnityEngine;
using UnityEditor;
using Unity.EditorCoroutines.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class ProceduralCaveGenerator : EditorWindow
{
    [MenuItem("POTCO/Procedural Cave Generator")]
    public static void ShowWindow() => GetWindow<ProceduralCaveGenerator>("Cave Generator");

    // --- Generation Settings ---
    int caveLength = 10;
    float generationDelay = 0.1f;
    bool capOpenEnds = true;

    // --- Internal State ---
    GameObject root;
    List<GameObject> allFoundPrefabs;
    List<GameObject> validPrefabs;
    Dictionary<int, List<GameObject>> categorizedPrefabs = new();
    List<GameObject> deadEnds;
    List<Transform> openConnectors = new();
    int currentIndex = 1;
    Vector2 prefabScroll;
    bool showPrefabs = true;

    Dictionary<GameObject, bool> prefabToggles = new();
    Dictionary<GameObject, int> prefabLikelihoods = new();

    void OnEnable()
    {
        LoadAllPrefabs();
    }

    void OnGUI()
    {
        GUILayout.Label("Generate Random Cave System", EditorStyles.boldLabel);
        caveLength = EditorGUILayout.IntField("Cave Length", caveLength);
        generationDelay = EditorGUILayout.Slider("Generation Delay", generationDelay, 0f, 2f);
        capOpenEnds = EditorGUILayout.Toggle("Cap Open Ends", capOpenEnds);

        if (GUILayout.Button("Generate Cave System"))
        {
            GenerateCavesWithDelay();
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Save Prefab Selection")) SaveSelections();
        if (GUILayout.Button("Load Prefab Selection")) LoadSelections();

        showPrefabs = EditorGUILayout.Foldout(showPrefabs, "Prefab Selection");
        if (showPrefabs)
        {
            if (allFoundPrefabs == null || allFoundPrefabs.Count == 0)
            {
                if (GUILayout.Button("Load Prefabs from Resources")) LoadAllPrefabs();
            }
            else
            {
                prefabScroll = EditorGUILayout.BeginScrollView(prefabScroll);
                foreach (var kvp in categorizedPrefabs.OrderBy(k => k.Key))
                {
                    string label = kvp.Key == 1 ? "Dead Ends" : $"{kvp.Key} Connectors";
                    GUILayout.Label(label, EditorStyles.boldLabel);

                    foreach (var prefab in kvp.Value)
                    {
                        DrawPrefabToggle(prefab);
                    }
                }
                EditorGUILayout.EndScrollView();
            }
        }
    }

    void DrawPrefabToggle(GameObject prefab)
    {
        EditorGUILayout.BeginHorizontal();

        if (!prefabToggles.ContainsKey(prefab)) prefabToggles[prefab] = true;
        if (!prefabLikelihoods.ContainsKey(prefab)) prefabLikelihoods[prefab] = 100;

        // Use ToggleLeft to display the full name next to the checkbox
        prefabToggles[prefab] = EditorGUILayout.ToggleLeft(prefab.name, prefabToggles[prefab]);

 
        GUILayout.FlexibleSpace();

        EditorGUI.BeginDisabledGroup(!prefabToggles[prefab]);

        GUILayout.Label("Spawn %:", GUILayout.Width(70));

        prefabLikelihoods[prefab] = EditorGUILayout.IntField(prefabLikelihoods[prefab], GUILayout.Width(40));

        prefabLikelihoods[prefab] = Mathf.Clamp(prefabLikelihoods[prefab], 0, 100);

        EditorGUI.EndDisabledGroup();

        if (GUILayout.Button("View", GUILayout.Width(50)))
        {
            EditorGUIUtility.PingObject(prefab);
        }
        EditorGUILayout.EndHorizontal();
    }

    void SaveSelections()
    {
        var selectionData = new SelectionData();
        foreach (var kvp in prefabToggles)
        {
            string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(kvp.Key));
            int likelihood = prefabLikelihoods.ContainsKey(kvp.Key) ? prefabLikelihoods[kvp.Key] : 100;
            selectionData.entries.Add(new SelectionEntry { guid = guid, isEnabled = kvp.Value, likelihood = likelihood });
        }

        string json = JsonUtility.ToJson(selectionData, true);
        string path = EditorUtility.SaveFilePanel("Save Prefab Selection", "", "cave_selection.json", "json");
        if (!string.IsNullOrEmpty(path)) File.WriteAllText(path, json);
    }

    void LoadSelections()
    {
        string path = EditorUtility.OpenFilePanel("Load Prefab Selection", "", "json");
        if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;

        string json = File.ReadAllText(path);
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
    }

    [System.Serializable]
    class SelectionEntry
    {
        public string guid;
        public bool isEnabled;
        public int likelihood;
    }
    [System.Serializable]
    class SelectionData
    {
        public List<SelectionEntry> entries = new List<SelectionEntry>();
    }

    void LoadAllPrefabs()
    {
        allFoundPrefabs = Resources.LoadAll<GameObject>("phase_4/models/caves/")
            .Where(p => p != null && PrefabUtility.GetPrefabAssetType(p) == PrefabAssetType.Regular).ToList();

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
        }
    }

    GameObject GetWeightedRandomPrefab(List<GameObject> prefabs)
    {
        var validOptions = prefabs
            .Where(p => prefabToggles.ContainsKey(p) && prefabToggles[p] && prefabLikelihoods[p] > 0)
            .ToList();

        if (validOptions.Count == 0) return null;

        int totalLikelihood = validOptions.Sum(p => prefabLikelihoods[p]);
        int randomPoint = Random.Range(0, totalLikelihood);

        foreach (var prefab in validOptions)
        {
            int likelihood = prefabLikelihoods[prefab];
            if (randomPoint < likelihood)
            {
                return prefab;
            }
            randomPoint -= likelihood;
        }

        return validOptions.LastOrDefault();
    }

    void GenerateCavesWithDelay()
    {
        if (root != null) DestroyImmediate(root);
        root = new GameObject("GeneratedCaveSystem");

        validPrefabs = allFoundPrefabs.Where(p => prefabToggles.ContainsKey(p) && prefabToggles[p]).ToList();
        deadEnds = validPrefabs.Where(p => categorizedPrefabs.ContainsKey(1) && categorizedPrefabs[1].Contains(p)).ToList();

        if (validPrefabs.Count == 0) { Debug.LogError("No valid prefabs selected or found."); return; }

        var startPrefabs = validPrefabs.Where(p => !deadEnds.Contains(p)).ToList();
        if (startPrefabs.Count == 0) { Debug.LogError("Cannot start generation. No non-dead-end prefabs are selected."); return; }

        var firstPrefab = GetWeightedRandomPrefab(startPrefabs);
        if (firstPrefab == null) { Debug.LogError("Cannot select a starting piece. Check likelihoods of non-dead-end prefabs."); return; }

        var firstWrapper = new GameObject("CavePiece_0");
        firstWrapper.transform.SetParent(root.transform);

        var firstInstance = (GameObject)PrefabUtility.InstantiatePrefab(firstPrefab, firstWrapper.transform);
        firstInstance.transform.localPosition = Vector3.zero;

        openConnectors = new List<Transform>(
            firstInstance.GetComponentsInChildren<Transform>().Where(t => t.name.StartsWith("cave_connector_"))
        );

        currentIndex = 1;
        EditorCoroutineUtility.StartCoroutineOwnerless(GenerateAllPieces());
    }

    IEnumerator GenerateAllPieces()
    {
        while (currentIndex < caveLength && openConnectors.Count > 0)
        {
            var fromConnector = openConnectors[Random.Range(0, openConnectors.Count)];
            openConnectors.Remove(fromConnector);

            var prefabsToChooseFrom = validPrefabs.Where(p => !deadEnds.Contains(p)).ToList();
            if (prefabsToChooseFrom.Count == 0 || (capOpenEnds && currentIndex >= caveLength - 1))
            {
                prefabsToChooseFrom = deadEnds;
            }

            if (prefabsToChooseFrom.Count == 0) continue;

            var chosenPrefab = GetWeightedRandomPrefab(prefabsToChooseFrom);
            if (chosenPrefab == null) continue;

            var wrapper = new GameObject($"CavePiece_{currentIndex}");
            wrapper.transform.SetParent(root.transform);
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(chosenPrefab, wrapper.transform);
            var allConnectors = instance.GetComponentsInChildren<Transform>().Where(t => t.name.StartsWith("cave_connector_")).ToList();
            var toConnector = allConnectors[Random.Range(0, allConnectors.Count)];

            AlignPiece(wrapper, fromConnector, toConnector);

            openConnectors.AddRange(allConnectors.Where(c => c != toConnector));
            currentIndex++;
            if (generationDelay > 0) yield return new EditorWaitForSeconds(generationDelay);
        }

        if (capOpenEnds)
        {
            Debug.Log($"🔹 Main generation finished. Now capping {openConnectors.Count} open ends...");
            yield return CapOpenConnectors();
            Debug.Log("✅ Cave generation and capping complete.");
        }
        else { Debug.Log("✅ Cave generation complete. Open ends were not capped."); }
    }

    IEnumerator CapOpenConnectors()
    {
        if (deadEnds == null || deadEnds.Count == 0) { Debug.LogWarning("⚠️ No dead-end prefabs selected/found. Cannot cap open connectors."); yield break; }

        var connectorsToCap = new List<Transform>(openConnectors);
        openConnectors.Clear();

        foreach (var fromConnector in connectorsToCap)
        {
            if (fromConnector == null) continue;

            var deadEndPrefab = GetWeightedRandomPrefab(deadEnds);
            if (deadEndPrefab == null) { Debug.LogWarning("⚠️ Could not select a dead-end piece. Check likelihoods."); continue; }

            var wrapper = new GameObject($"CaveDeadEnd_{currentIndex}");
            wrapper.transform.SetParent(root.transform);
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(deadEndPrefab, wrapper.transform);
            var toConnector = instance.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name.StartsWith("cave_connector_"));

            if (toConnector == null) { Debug.LogWarning($"Prefab '{deadEndPrefab.name}' is a dead-end but has no connector. Skipping."); DestroyImmediate(wrapper); continue; }

            AlignPiece(wrapper, fromConnector, toConnector);
            currentIndex++;
            if (generationDelay > 0) yield return new EditorWaitForSeconds(generationDelay);
        }
    }


    void AlignPiece(GameObject wrapper, Transform fromConnector, Transform toConnector)
    {
        string fromDir = GetDirectionFromVector(fromConnector.forward);
        string toDir = GetDirectionFromVector(toConnector.forward);
        float yRotation = GetYRotationToMatch(fromDir, toDir);

        wrapper.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        Vector3 offset = fromConnector.position - toConnector.position;
        wrapper.transform.position += offset;
    }

    string GetDirectionFromVector(Vector3 forward)
    {
        forward.y = 0;
        forward.Normalize();
        if (Vector3.Angle(forward, Vector3.forward) < 45f) return "TOP";
        if (Vector3.Angle(forward, Vector3.right) < 45f) return "RIGHT";
        if (Vector3.Angle(forward, Vector3.back) < 45f) return "BOTTOM";
        if (Vector3.Angle(forward, Vector3.left) < 45f) return "LEFT";
        return "UNKNOWN";
    }

    float GetAngleFromDirection(string dir)
    {
        switch (dir) { case "TOP": return 0f; case "RIGHT": return 90f; case "BOTTOM": return 180f; case "LEFT": return 270f; default: return 0f; }
    }

    float GetYRotationToMatch(string fromDir, string toDir)
    {
        float fromAngle = GetAngleFromDirection(fromDir);
        float toAngle = GetAngleFromDirection(toDir);
        float targetAngle = fromAngle + 180f;
        float requiredRotation = targetAngle - toAngle;
        return Mathf.DeltaAngle(0, requiredRotation);
    }
}