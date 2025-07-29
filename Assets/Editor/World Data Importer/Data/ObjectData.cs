using UnityEngine;
using System.Collections.Generic;

namespace WorldDataImporter.Data
{
    public class ObjectData
    {
        public string id;
        public GameObject gameObject;
        public int indent;
        public Dictionary<string, string> properties = new Dictionary<string, string>();
        
        // Advanced properties
        public Color? visualColor;
        public bool? disableCollision;
        public string holiday;
        public string objectType;
        public bool isInstanced;
        public string visSize;
        
        // Light properties
        public string lightType;        // AMBIENT, POINT, SPOT
        public float? intensity;
        public float? attenuation;
        public float? coneAngle;
        public float? dropOff;
        public bool? flickering;
        public float? flickRate;
    }
    
    [System.Serializable]
    public class ImportStatistics
    {
        public int totalObjects;
        public int successfulImports;
        public int missingModels;
        public int colorOverrides;
        public int collisionDisabled;
        public int collisionRemoved;
        public int lightsCreated;
        public Dictionary<string, int> objectTypeCount = new Dictionary<string, int>();
        public List<string> missingModelPaths = new List<string>();
        public float importTime;
    }
}