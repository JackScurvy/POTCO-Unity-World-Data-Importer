using UnityEngine;

namespace CaveGenerator.Algorithms
{
    public static class CaveAlignmentAlgorithm
    {
        public static void AlignCavePieces(GameObject wrapper, Transform fromConnector, Transform toConnector)
        {
            Debug.Log($"🔧 ALIGNING: {wrapper.name} to connect {fromConnector.name}[{fromConnector.position}] ↔ {toConnector.name}[{toConnector.position}]");
            
            // Step 1: Detection - Convert connector directions to cardinal labels
            string fromDirection = GetCardinalDirection(fromConnector.forward);
            string toDirection = GetCardinalDirection(toConnector.forward);
            
            Debug.Log($"   From connector facing: {fromDirection}, To connector facing: {toDirection}");
            
            // Step 2: Calculate required rotation using cardinal directions
            float fromAngle = GetAngleFromCardinal(fromDirection);
            float goalAngle = (fromAngle + 180f) % 360f; // Opposite direction
            float startAngle = GetAngleFromCardinal(toDirection);
            float requiredRotation = (goalAngle - startAngle + 360f) % 360f;
            
            // Normalize to shortest rotation (-180 to +180)
            if (requiredRotation > 180f)
                requiredRotation -= 360f;
                
            Debug.Log($"   From angle: {fromAngle}°, Goal angle: {goalAngle}°, Start angle: {startAngle}°");
            Debug.Log($"   Required Y rotation: {requiredRotation}°");
            
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
            
            Debug.Log($"   {qualityMsg} Final Distance: {finalDistance:F3}m, Angle: {finalAngle:F1}°");
        }
        
        public static string GetCardinalDirection(Vector3 direction)
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
        
        public static float GetAngleFromCardinal(string cardinal)
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
    }
}