using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldDataExporter.Data
{
    [Serializable]
    public class POTCOObjectDefinition
    {
        public string objectType;
        public Dictionary<string, object> properties = new Dictionary<string, object>();
        public Dictionary<string, object> defaults = new Dictionary<string, object>();
        public POTCOVisualDefinition visual;
        public bool nonRpmNode = false;
        public bool raycast = true;
        public bool selectable = true;
        public bool linkable = false;
        
        public POTCOObjectDefinition(string type)
        {
            objectType = type;
            visual = new POTCOVisualDefinition();
        }
        
        public bool HasProperty(string propertyName)
        {
            return properties.ContainsKey(propertyName);
        }
        
        public object GetDefaultValue(string propertyName)
        {
            return defaults.ContainsKey(propertyName) ? defaults[propertyName] : null;
        }
        
        public List<string> GetAvailableModels()
        {
            return visual?.models ?? new List<string>();
        }
        
        public string GetDefaultModel()
        {
            var models = GetAvailableModels();
            return models.Count > 0 ? models[0] : null;
        }
        
        public bool IsLightObject()
        {
            return objectType == "Light - Dynamic";
        }
        
        public bool IsNodeObject()
        {
            return objectType.Contains("Node") || objectType == "Townsperson" || nonRpmNode;
        }
        
        public bool IsCollisionObject()
        {
            return objectType.Contains("Collision Barrier");
        }
        
        public bool IsHolidayObject()
        {
            return objectType == "Holiday Object";
        }
    }
    
    [Serializable]
    public class POTCOVisualDefinition
    {
        public List<string> models = new List<string>();
        public Color? color;
        public Vector3? scale;
        public Vector3? offset;
        
        public POTCOVisualDefinition()
        {
        }
    }
    
    [Serializable]
    public class POTCOPropertyDefinition
    {
        public string name;
        public string uiType; // PROP_UI_ENTRY, PROP_UI_SLIDE, etc.
        public List<object> parameters = new List<object>();
        public string callback;
        
        public POTCOPropertyDefinition(string propertyName, string propUIType)
        {
            name = propertyName;
            uiType = propUIType;
        }
        
        public bool IsSlider()
        {
            return uiType == "PROP_UI_Slider";
        }
        
        public bool IsComboBox()
        {
            return uiType == "Prop_UI_ComboBox";
        }
        
        public bool IsCheckBox()
        {
            return uiType == "PROP_UI_CheckBox";
        }
        
        public bool IsEntry()
        {
            return uiType == "Prop_UI_Entry";
        }
    }
}