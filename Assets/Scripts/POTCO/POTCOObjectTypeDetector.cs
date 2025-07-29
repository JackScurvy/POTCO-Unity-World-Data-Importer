using UnityEngine;
using System.Collections.Generic;

namespace POTCO
{
    /// <summary>
    /// Runtime-accessible wrapper for POTCO object type detection
    /// This allows the POTCOTypeInfo component to access ObjectList data without editor dependencies
    /// </summary>
    public static class POTCOObjectTypeDetector
    {
        /// <summary>
        /// Try to detect object type from ObjectList.py data
        /// This should only be used as a fallback when ObjectListParser is not available
        /// </summary>
        public static string DetectObjectType(string modelName)
        {
            if (string.IsNullOrEmpty(modelName)) return "Prop";
            
            // This method should not hardcode patterns - it should use ObjectList.py data
            // For now, just return "Prop" as a safe default
            // The real detection should happen through ObjectListParser
            return "Prop";
        }
        
        /// <summary>
        /// Get basic object type list for when ObjectListParser is not available
        /// </summary>
        public static List<string> GetBasicObjectTypes()
        {
            return new List<string>
            {
                "Prop",
                "Furniture", 
                "Light - Dynamic",
                "Building Interior",
                "Collision Barrier",
                "Vegetation",
                "Rock",
                "Ship Part",
                "Weapon", 
                "Townsperson",
                "Effect",
                "Spawn Node",
                "GUI Element",
                "Island",
                "Region",
                "Island Game Area",
                "Connector Tunnel",
                "Connector Door",
                "Locator Node"
            };
        }
        
        /// <summary>
        /// Check if an object type typically has a Name property in POTCO
        /// </summary>
        public static bool ObjectTypeHasName(string objectType)
        {
            if (string.IsNullOrEmpty(objectType)) return false;
            
            string[] typesWithNames = {
                "Building Interior",
                "Island", 
                "Region",
                "Island Game Area",
                "Townsperson"
            };
            
            return System.Array.Exists(typesWithNames, t => t.Equals(objectType, System.StringComparison.OrdinalIgnoreCase));
        }
        
        /// <summary>
        /// Check if an object type typically has an Instanced property in POTCO
        /// </summary>
        public static bool ObjectTypeHasInstanced(string objectType)
        {
            if (string.IsNullOrEmpty(objectType)) return false;
            
            string[] typesWithInstanced = {
                "Prop",
                "Furniture",
                "Vegetation",
                "Rock"
            };
            
            return System.Array.Exists(typesWithInstanced, t => t.Equals(objectType, System.StringComparison.OrdinalIgnoreCase));
        }
    }
}