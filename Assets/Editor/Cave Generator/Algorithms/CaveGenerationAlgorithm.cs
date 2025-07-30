using UnityEngine;
using UnityEditor;
using Unity.EditorCoroutines.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CaveGenerator.Data;
using POTCO.Editor;

namespace CaveGenerator.Algorithms
{
    public class CaveGenerationAlgorithm
    {
        // Dependencies injected by main window
        public GenerationSettings settings;
        public GameObject root;
        public List<GameObject> validPrefabs;
        public List<GameObject> deadEnds;
        public List<Transform> openConnectors;
        public Dictionary<Transform, ConnectorInfo> connectorData;
        public List<CavePieceNode> generatedPieces;
        public HashSet<Vector3> occupiedPositions;
        public Dictionary<GameObject, int> prefabLikelihoods;
        public int currentIndex;
        public string lastGenerationSeed;
        
        public IEnumerator GenerateCaveCoroutine()
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
        
        public IEnumerator GenerateBranchingCave()
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
                if (depth > settings.maxDepth)
                {
                    DebugLogger.LogProceduralGeneration($"⏭️ Skipping connector {fromConnector.name} - depth {depth} exceeds max {settings.maxDepth}");
                    continue;
                }
                
                DebugLogger.LogProceduralGeneration($"🔗 Attempting to connect piece {piecesGenerated + 1} to connector {fromConnector.name} at depth {depth}");
                
                // Choose piece type based on depth and branching settings
                var prefabsToChooseFrom = new List<GameObject>();
                
                if (depth < settings.maxDepth && Random.value < settings.branchProbability)
                {
                    // Use tunnel pieces for branching
                    prefabsToChooseFrom = validPrefabs.Where(p => !deadEnds.Contains(p)).ToList();
                    DebugLogger.LogProceduralGeneration("🌿 Attempting to create branch");
                }
                else if (piecesGenerated >= settings.caveLength - 2 || depth >= settings.maxDepth)
                {
                    // Use end caps for finishing
                    prefabsToChooseFrom = deadEnds;
                    DebugLogger.LogProceduralGeneration("🏁 Using end cap pieces");
                }
                else
                {
                    // Use any piece
                    prefabsToChooseFrom = validPrefabs;
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
                var newNode = ConnectCavePiece(chosenPrefab, fromConnector, depth);
                
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
        
        public IEnumerator GenerateLinearCave()
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
                
                if (piecesGenerated >= settings.caveLength - 1)
                {
                    // Use end caps for the final piece
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
                var newNode = ConnectCavePiece(chosenPrefab, currentConnector, piecesGenerated);
                
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
        
        // These methods need to be implemented by the main class or injected as delegates
        public System.Func<GameObject, Transform, int, CavePieceNode> ConnectCavePiece;
        public System.Func<List<GameObject>, GameObject> GetWeightedRandomPrefab;
        public System.Action<int> FinalizeCaveGeneration;
    }
}