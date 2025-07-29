using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldDataExporter.Data
{
    [Serializable]
    public class ExportedObject
    {
        public string id;
        public string objectType;
        public string name;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public string modelPath;
        public Color? visualColor;
        
        // Hierarchy
        public ExportedObject parent;
        public List<ExportedObject> children = new List<ExportedObject>();
        
        // POTCO Properties
        public bool? instanced;
        public bool? disableCollision;
        public string holiday;
        public string visSize;
        
        // Lighting Properties (for Light - Dynamic objects)
        public string lightType;           // AMBIENT, DIRECTIONAL, POINT, SPOT
        public float? intensity;
        public float? attenuation;
        public float? coneAngle;
        public float? dropOff;
        public bool? flickering;
        public float? flickRate;
        
        // Additional properties that might be needed
        public Dictionary<string, object> customProperties = new Dictionary<string, object>();
        
        public ExportedObject(string objectId)
        {
            id = objectId;
        }
        
        public void AddChild(ExportedObject child)
        {
            children.Add(child);
            child.parent = this;
        }
        
        public bool IsLightObject()
        {
            return objectType == "Light - Dynamic";
        }
        
        public bool IsCollisionObject()
        {
            return objectType != null && objectType.Contains("Collision Barrier");
        }
        
        public bool IsNodeObject()
        {
            return objectType != null && (objectType.Contains("Node") || objectType == "Townsperson");
        }
        
        public bool IsHolidayObject()
        {
            return !string.IsNullOrEmpty(holiday);
        }
    }
}