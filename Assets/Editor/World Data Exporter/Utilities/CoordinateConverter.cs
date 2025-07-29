using System;
using UnityEngine;

namespace WorldDataExporter.Utilities
{
    public static class CoordinateConverter
    {
        /// <summary>
        /// Converts Unity position to Panda3D position format
        /// Unity: (x, y, z) where Y is up
        /// Panda3D: (x, z, y) where Z is up (from original import: new Vector3(x, z, y))
        /// For export we need to reverse this: (unity.x, unity.z, unity.y)
        /// </summary>
        public static Vector3 UnityToPanda3DPosition(Vector3 unityPos)
        {
            return new Vector3(unityPos.x, unityPos.z, unityPos.y);
        }

        /// <summary>
        /// Converts Unity rotation to Panda3D HPR (Heading, Pitch, Roll) format
        /// Using YXZ_NNN format: Y->heading, X->pitch, Z->roll, all negative
        /// </summary>
        public static Vector3 UnityToPanda3DHPR(Vector3 unityEuler)
        {
            UnityEngine.Debug.Log($"🔄 Converting Unity Euler {unityEuler} to Panda3D HPR (YXZ_NNN format)");
            
            float heading = -unityEuler.y;  // hpr.x (heading) = -Unity.Y
            float pitch = -unityEuler.x;    // hpr.y (pitch) = -Unity.X
            float roll = -unityEuler.z;     // hpr.z (roll) = -Unity.Z
            
            Vector3 result = new Vector3(heading, pitch, roll);
            UnityEngine.Debug.Log($"🔄 Result: {result} (H:{heading}, P:{pitch}, R:{roll})");
            
            return result;
        }

        /// <summary>
        /// Converts Unity scale to Panda3D scale - swap Y and Z
        /// </summary>
        public static Vector3 UnityToPanda3DScale(Vector3 unityScale)
        {
            return new Vector3(unityScale.x, unityScale.z, unityScale.y);
        }

        /// <summary>
        /// Converts Unity Color to Panda3D color tuple format
        /// </summary>
        public static string UnityToPanda3DColor(Color unityColor)
        {
            // POTCO uses very high precision for colors like: 0.69999998807907104
            return $"({unityColor.r:F17}, {unityColor.g:F17}, {unityColor.b:F17}, {unityColor.a:F17})";
        }

        /// <summary>
        /// Generates POTCO-style object ID
        /// Format: timestamp.sequence + username
        /// </summary>
        public static string GeneratePOTCOId()
        {
            // Generate timestamp similar to POTCO format
            double timestamp = (System.DateTime.UtcNow - new System.DateTime(1970, 1, 1)).TotalSeconds;
            
            // Add some randomness for uniqueness
            int sequence = UnityEngine.Random.Range(10, 99);
            
            // Use a default username that won't conflict with Unity filter
            string username = "export";
            
            return $"{timestamp:F2}{username}{sequence:D2}";
        }

        /// <summary>
        /// Formats a float value to match POTCO precision
        /// </summary>
        public static string FormatPOTCOFloat(float value)
        {
            // POTCO uses specific precision patterns - match original format
            if (value == 0.0f)
                return "0.0";
            
            // For very small decimals, use high precision like original
            if (Math.Abs(value) < 0.001f && value != 0)
                return value.ToString("F5");
                
            // For normal values, use reasonable precision
            string formatted = value.ToString("F5");
            
            // Remove trailing zeros but keep at least one decimal place
            formatted = formatted.TrimEnd('0');
            if (formatted.EndsWith("."))
                formatted += "0";
                
            return formatted;
        }

        /// <summary>
        /// Formats a Vector3 as Panda3D Point3 or VBase3
        /// </summary>
        public static string FormatPanda3DVector3(Vector3 vector, bool useVBase3 = false)
        {
            string type = useVBase3 ? "VBase3" : "Point3";
            return $"{type}({FormatPOTCOFloat(vector.x)}, {FormatPOTCOFloat(vector.y)}, {FormatPOTCOFloat(vector.z)})";
        }

        /// <summary>
        /// Converts boolean to Python format
        /// </summary>
        public static string BoolToPython(bool value)
        {
            return value ? "True" : "False";
        }

        /// <summary>
        /// Escapes string for Python format 
        /// </summary>
        public static string StringToPython(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "''";
                
            // Escape single quotes and backslashes
            value = value.Replace("\\", "\\\\").Replace("'", "\\'");
            return $"'{value}'";
        }
    }
}