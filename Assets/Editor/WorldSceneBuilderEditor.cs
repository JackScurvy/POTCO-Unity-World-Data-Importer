using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class WorldSceneBuilderEditor : EditorWindow
{
    private string filePath = "";

    [MenuItem("POTCO/World Data Importer")]
    public static void ShowWindow()
    {
        GetWindow<WorldSceneBuilderEditor>("World Scene Importer");
    }

    void OnGUI()
    {
        GUILayout.Label("Import Pirates Online World Data .py File", EditorStyles.boldLabel);

        if (GUILayout.Button("Select World .py File"))
        {
            string selected = EditorUtility.OpenFilePanel("Select .py File", "", "py");
            if (!string.IsNullOrEmpty(selected))
            {
                filePath = selected;
                Debug.Log($"📄 Selected file: {filePath}");
            }
        }

        GUILayout.Label("Selected File: " + filePath);

        if (!string.IsNullOrEmpty(filePath))
        {
            if (GUILayout.Button("Build Scene"))
            {
                Debug.Log("🚧 Starting world build...");
                BuildSceneFromPython(filePath);
            }
        }
    }

    private void BuildSceneFromPython(string path)
    {
        Debug.Log($"📥 Reading file: {path}");
        string[] lines = File.ReadAllLines(path);

        Dictionary<string, GameObject> createdObjects = new();
        Stack<(GameObject go, int indent)> parentStack = new();
        GameObject root = null;

        Regex objIdRegex = new(@"^\s*'(\d+\.\d+\w*)':\s*{");
        Regex propRegex = new(@"^\s*'(\w+)':\s*(.*)");
        Regex modelPathRegex = new(@"'([^']+)'");

        for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            string line = lines[lineIndex];
            if (string.IsNullOrWhiteSpace(line)) continue;

            int indent = line.TakeWhile(char.IsWhiteSpace).Count();

            while (parentStack.Count > 0 && indent <= parentStack.Peek().indent)
            {
                parentStack.Pop();
            }

            GameObject currentGO = parentStack.Count > 0 ? parentStack.Peek().go : null;

            if (objIdRegex.IsMatch(line))
            {
                Match m = objIdRegex.Match(line);
                string currentId = m.Groups[1].Value;
                var newGO = new GameObject(currentId);
                createdObjects[currentId] = newGO;

                if (currentGO != null) newGO.transform.SetParent(currentGO.transform, false);
                else root = newGO;

                parentStack.Push((newGO, indent));
                continue;
            }

            if (propRegex.IsMatch(line) && currentGO != null)
            {
                Match pm = propRegex.Match(line);
                string key = pm.Groups[1].Value;
                string val = pm.Groups[2].Value.Trim().TrimEnd(',');

                switch (key)
                {
                    case "Pos":
                        if (currentGO != root) currentGO.transform.localPosition = ParseVector3(val);
                        break;
                    case "Hpr":
                        if (currentGO != root)
                        {
                            Vector3 hpr = ParseVector3(val);
                            currentGO.transform.localEulerAngles = new Vector3(hpr.y, -hpr.x, -hpr.z);
                        }
                        break;
                    case "Scale":
                        if (currentGO != root) currentGO.transform.localScale = ParseVector3(val, Vector3.one);
                        break;
                    case "Type":
                        if (currentGO != root)
                        {
                            currentGO.name = $"{val.Trim('\'')}_{currentGO.name}";
                        }
                        break;
                    case "Name":
                        string objectName = val.Trim('\'');
                        if (!string.IsNullOrEmpty(objectName))
                        {
                            currentGO.name = objectName;
                        }
                        break;
                    case "Model":
                        Match modelMatch = modelPathRegex.Match(val);
                        if (modelMatch.Success) InstantiatePrefab(modelMatch.Groups[1].Value, currentGO);
                        break;
                }
                continue;
            }
        }
        Debug.Log($"✅ Scene built successfully.");
    }

    private void InstantiatePrefab(string modelPath, GameObject parentGO)
    {
        string[] phaseFolders = Directory.GetDirectories("Assets/Resources", "phase_*", SearchOption.AllDirectories);
        GameObject prefab = null;

        foreach (string phase in phaseFolders)
        {
            string attemptPath = Path.Combine(phase, modelPath + ".prefab").Replace("\\", "/");
            prefab = AssetDatabase.LoadAssetAtPath<GameObject>(attemptPath);
            if (prefab != null)
            {
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                instance.name = prefab.name;
                instance.transform.SetParent(parentGO.transform, false);
                return;
            }
        }
        Debug.LogWarning($"❌ Prefab not found for model: '{modelPath}'.");
    }

    private Vector3 ParseVector3(string val, Vector3 fallback = default)
    {
        Match m = Regex.Match(val, @"\(?\s*([-+]?[0-9]*\.?[0-9]+)[f]?\s*,\s*([-+]?[0-9]*\.?[0-9]+)[f]?\s*,\s*([-+]?[0-9]*\.?[0-9]+)[f]?\s*\)?");
        if (!m.Success)
        {
            return fallback == default ? Vector3.zero : fallback;
        }

        float x = float.Parse(m.Groups[1].Value);
        float y = float.Parse(m.Groups[2].Value);
        float z = float.Parse(m.Groups[3].Value);

        return new Vector3(x, z, y);
    }
}