using System;
using UnityEngine;
using System.Collections.Generic;

namespace WorldDataImporter.Data
{
    [Serializable]
    public class ImportSettings
    {
        [Header("Basic Settings")]
        public string filePath = "";
        public bool useEggFiles = true;
        public bool importObjectListData = true;
        
        [Header("Advanced Import Options")]
        public bool applyColorOverrides = true;
        public bool importCollisions = true;
        public bool addLighting = true;
        public bool importNodes = false;
        
        [Header("Filtering Options")]
        public bool importHolidayObjects = true;
        public List<string> excludeObjectTypes = new List<string>();
        public List<string> includeObjectTypes = new List<string>();
        
        [Header("Performance Options")]
        public bool showImportStatistics = true;
        public bool logDetailedInfo = false;
        
        [Header("Generation Delay")]
        public bool useGenerationDelay = false;
        public float delayBetweenObjects = 0.01f; // Delay in seconds between creating objects
    }
}