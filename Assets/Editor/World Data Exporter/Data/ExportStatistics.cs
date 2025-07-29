using System;
using System.Collections.Generic;

namespace WorldDataExporter.Data
{
    [Serializable]
    public class ExportStatistics
    {
        public int totalObjectsExported = 0;
        public int lightingObjectsExported = 0;
        public int collisionObjectsExported = 0;
        public int nodeObjectsExported = 0;
        public float exportTime = 0f;
        public float fileSizeKB = 0f;
        
        public Dictionary<string, int> objectTypeCount = new Dictionary<string, int>();
        public List<string> warnings = new List<string>();
        public List<string> exportedObjectIds = new List<string>();
        
        public void AddObjectType(string type)
        {
            if (objectTypeCount.ContainsKey(type))
                objectTypeCount[type]++;
            else
                objectTypeCount[type] = 1;
        }
        
        public void AddWarning(string warning)
        {
            warnings.Add(warning);
        }
    }
}