using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldDataExporter.Data
{
    public enum ExportSource
    {
        EntireScene,
        SelectedObjects,
        RootObject
    }

    [Serializable]
    public class ExportSettings
    {
        [Header("Basic Settings")]
        public ExportSource exportSource = ExportSource.EntireScene;
        public string outputPath = "";
        
        [Header("Object Type Filtering")]
        public bool exportLighting = true;
        public bool exportCollisions = true;
        public bool exportNodes = false;
        public bool exportHolidayObjects = true;
        
        
        [Header("Advanced Options")]
        public List<string> excludeObjectTypes = new List<string>();
        public List<string> includeObjectTypes = new List<string>();
        public bool preserveHierarchy = true;
        public bool generateUniqueIds = true;
    }
}