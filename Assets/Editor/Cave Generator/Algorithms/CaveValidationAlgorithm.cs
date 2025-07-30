using UnityEngine;
using POTCO.Editor;
using System.Collections.Generic;
using System.Linq;

namespace CaveGenerator.Algorithms
{
    public static class CaveValidationAlgorithm
    {
        public static bool CheckForOverlap(Vector3 position, float radius, HashSet<Vector3> occupiedPositions)
        {
            return occupiedPositions.Any(pos => Vector3.Distance(pos, position) < radius);
        }
        
        public static bool CheckForOverlapExcluding(Vector3 position, float radius, Transform excludeConnector, HashSet<Vector3> occupiedPositions)
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
        
        public static Transform GetCavePieceFromConnector(Transform connector)
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
        
        public static bool ValidateConnectionQuality(Transform fromConnector, Transform toConnector, float maxDistance = 2.0f, float maxAngle = 60f)
        {
            float connectionDistance = Vector3.Distance(fromConnector.position, toConnector.position);
            float connectionAngle = Vector3.Angle(fromConnector.forward, -toConnector.forward);
            
            DebugLogger.LogProceduralGeneration($"🔍 Connection Quality Check: Distance={connectionDistance:F3}m, Angle={connectionAngle:F1}°");
            DebugLogger.LogProceduralGeneration($"   From connector: {fromConnector.name} at {fromConnector.position}, Dir: {fromConnector.forward}");
            DebugLogger.LogProceduralGeneration($"   To connector: {toConnector.name} at {toConnector.position}, Dir: {toConnector.forward}");
            
            // Reject connections that are too far off
            if (connectionDistance > maxDistance || connectionAngle > maxAngle)
            {
                DebugLogger.LogWarningProceduralGeneration($"❌ Rejected poor connection: Distance={connectionDistance:F3}m, Angle={connectionAngle:F1}° between {fromConnector.name} and {toConnector.name}");
                DebugLogger.LogWarningProceduralGeneration($"   Thresholds: Distance must be ≤{maxDistance}m, Angle must be ≤{maxAngle}°");
                return false;
            }
            
            return true;
        }
    }
}