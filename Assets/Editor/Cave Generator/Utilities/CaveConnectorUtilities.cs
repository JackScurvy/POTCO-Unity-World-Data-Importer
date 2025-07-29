using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using CaveGenerator.Data;

namespace CaveGenerator.Utilities
{
    public static class CaveConnectorUtilities
    {
        public static List<Transform> FindAllConnectors(GameObject piece)
        {
            return piece.GetComponentsInChildren<Transform>()
                .Where(t => t.name.StartsWith("cave_connector_"))
                .ToList();
        }
        
        public static int CountConnectors(GameObject piece)
        {
            return piece.GetComponentsInChildren<Transform>()
                .Count(t => t.name.StartsWith("cave_connector_"));
        }
        
        public static GameObject InstantiateCavePiece(GameObject source, Transform parent, bool useEggFiles)
        {
            GameObject instance;
            if (useEggFiles)
            {
                instance = Object.Instantiate(source, parent);
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
        
        public static GameObject GetWeightedRandomPrefab(List<GameObject> prefabs, 
            Dictionary<GameObject, bool> prefabToggles, 
            Dictionary<GameObject, int> prefabLikelihoods)
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
        
        public static void VisualizeConnectors(GameObject piece, Color openColor, Color usedColor, 
            Dictionary<Transform, ConnectorInfo> connectorData)
        {
            var connectors = FindAllConnectors(piece);
            foreach (var connector in connectors)
            {
                bool isUsed = connectorData.ContainsKey(connector) && connectorData[connector].isUsed;
                Color color = isUsed ? usedColor : openColor;
                
                // Draw connector position and direction
                Gizmos.color = color;
                Gizmos.DrawSphere(connector.position, 0.1f);
                Gizmos.DrawRay(connector.position, connector.forward * 0.5f);
            }
        }
    }
}