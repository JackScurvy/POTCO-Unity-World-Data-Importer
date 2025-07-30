using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using UnityEngine;
using WorldDataExporter.Data;
using POTCO.Editor;

namespace WorldDataExporter.Utilities
{
    public static class PythonFileGenerator
    {
        public static bool GeneratePythonFile(List<ExportedObject> exportedObjects, ExportSettings settings, ExportStatistics stats)
        {
            try
            {
                var content = new StringBuilder();
                
                // Generate file header
                GenerateHeader(content, settings);
                
                // Generate the main objectStruct
                GenerateObjectStruct(content, exportedObjects, settings);
                
                // Write to file
                File.WriteAllText(settings.outputPath, content.ToString());
                
                DebugLogger.LogWorldExporter($"📄 Generated Python file: {settings.outputPath}");
                return true;
            }
            catch (System.Exception ex)
            {
                DebugLogger.LogErrorWorldExporter($"❌ Failed to generate Python file: {ex.Message}");
                stats.AddWarning($"File generation failed: {ex.Message}");
                return false;
            }
        }
        
        private static void GenerateHeader(StringBuilder content, ExportSettings settings)
        {
            // Match exact POTCO format - no comments, just imports
            content.AppendLine("from pandac.PandaModules import Point3, VBase3, Vec4, Vec3");
        }
        
        private static void GenerateObjectStruct(StringBuilder content, List<ExportedObject> exportedObjects, ExportSettings settings)
        {
            content.AppendLine("objectStruct = {");
            content.AppendLine("    'Objects': {");
            
            // Find root objects (objects without parents)
            var rootObjects = exportedObjects.Where(obj => obj.parent == null).ToList();
            
            // Generate each root object and its children
            for (int i = 0; i < rootObjects.Count; i++)
            {
                GenerateObjectEntry(content, rootObjects[i], 2, settings, i == rootObjects.Count - 1);
            }
            
            content.AppendLine("    }");
            content.AppendLine("}");
        }
        
        
        private static void GenerateObjectEntry(StringBuilder content, ExportedObject obj, int indentLevel, ExportSettings settings, bool isLast = false)
        {
            string indent = new string(' ', indentLevel * 4);
            string childIndent = new string(' ', (indentLevel + 1) * 4);
            
            content.AppendLine($"{indent}'{obj.id}': {{");
            
            // Generate properties BEFORE Objects (exclude Visual and late properties)
            GenerateEarlyProperties(content, obj, indentLevel + 1, settings);
            
            // Generate child objects if any
            if (obj.children.Count > 0)
            {
                content.AppendLine($"{childIndent}'Objects': {{");
                
                for (int i = 0; i < obj.children.Count; i++)
                {
                    GenerateObjectEntry(content, obj.children[i], indentLevel + 2, settings, i == obj.children.Count - 1);
                }
                
                content.AppendLine($"{childIndent}}},");
            }
            
            // Generate remaining properties AFTER Objects (transform, Visual)
            GenerateLateProperties(content, obj, indentLevel + 1, settings, isLast);
            
            // Don't add closing brace - Visual block handles the complete object closure
        }
        
        private static void GenerateEarlyProperties(StringBuilder content, ExportedObject obj, int indentLevel, ExportSettings settings)
        {
            string indent = new string(' ', indentLevel * 4);
            var properties = new List<string>();
            
            // Follow exact POTCO property order:
            // 1. Type
            if (!string.IsNullOrEmpty(obj.objectType))
            {
                properties.Add($"{indent}'Type': {CoordinateConverter.StringToPython(obj.objectType)}");
            }
            
            // 2. Name (only for object types defined in BUILDING_INTERIOR_LIST and AREA_TYPE constants)
            if (ObjectListParser.ObjectTypeHasName(obj.objectType))
            {
                string nameValue = string.IsNullOrEmpty(obj.name) ? "''" : CoordinateConverter.StringToPython(obj.name);
                properties.Add($"{indent}'Name': {nameValue}");
            }
            
            // 3. Instanced (only for Building Interior objects from BUILDING_INTERIOR_LIST)
            if (obj.instanced.HasValue && ObjectListParser.ObjectTypeHasInstanced(obj.objectType))
            {
                properties.Add($"{indent}'Instanced': {CoordinateConverter.BoolToPython(obj.instanced.Value)}");
            }
            
            // 4. DisableCollision (if applicable) - Building Interior objects don't have this property
            if (obj.disableCollision.HasValue && obj.objectType != "Building Interior")
            {
                properties.Add($"{indent}'DisableCollision': {CoordinateConverter.BoolToPython(obj.disableCollision.Value)}");
            }
            
            // 5. Holiday property (if applicable) - include empty values to match original format
            if (obj.holiday != null)
            {
                properties.Add($"{indent}'Holiday': {CoordinateConverter.StringToPython(obj.holiday)}");
            }
            
            // 6. Lighting properties (for Light - Dynamic objects) - before transform
            if (obj.IsLightObject())
            {
                GenerateLightingProperties(properties, obj, indent);
            }
            
            // Early properties end here - transform and Visual come after Objects
            
            // Write all early properties with commas
            for (int i = 0; i < properties.Count; i++)
            {
                content.AppendLine($"{properties[i]},");
            }
        }
        
        private static void GenerateLateProperties(StringBuilder content, ExportedObject obj, int indentLevel, ExportSettings settings, bool isLastObject)
        {
            string indent = new string(' ', indentLevel * 4);
            var properties = new List<string>();
            
            // Transform properties - POTCO order: Hpr, Pos, Scale  
            // Hpr can be Point3 or VBase3, Pos is always Point3, Scale is always VBase3
            bool useVBase3ForHpr = obj.rotation.x != 0 || obj.rotation.y != 0 || obj.rotation.z != 0;
            properties.Add($"{indent}'Hpr': {CoordinateConverter.FormatPanda3DVector3(obj.rotation, useVBase3: useVBase3ForHpr)}");
            properties.Add($"{indent}'Pos': {CoordinateConverter.FormatPanda3DVector3(obj.position, useVBase3: false)}");
            properties.Add($"{indent}'Scale': {CoordinateConverter.FormatPanda3DVector3(obj.scale, useVBase3: true)}");
            
            // VisSize property (if applicable) - include empty values to match original format
            if (obj.visSize != null)
            {
                properties.Add($"{indent}'VisSize': {CoordinateConverter.StringToPython(obj.visSize)}");
            }
            
            // Custom properties
            foreach (var kvp in obj.customProperties)
            {
                string value = FormatCustomProperty(kvp.Value);
                properties.Add($"{indent}'{kvp.Key}': {value}");
            }
            
            // Visual properties (always last) - needs to know if this object needs comma
            GenerateVisualProperties(properties, obj, indent, settings, !isLastObject);
            
            // Write all late properties with commas between them, but Visual (last) has no comma
            for (int i = 0; i < properties.Count; i++)
            {
                bool isLastProperty = (i == properties.Count - 1);
                content.AppendLine($"{properties[i]}{(isLastProperty ? "" : ",")}");
            }
        }
        
        private static void GenerateLightingProperties(List<string> properties, ExportedObject obj, string indent)
        {
            if (!string.IsNullOrEmpty(obj.lightType))
            {
                properties.Add($"{indent}'LightType': {CoordinateConverter.StringToPython(obj.lightType)}");
            }
            
            if (obj.intensity.HasValue)
            {
                properties.Add($"{indent}'Intensity': {CoordinateConverter.StringToPython(CoordinateConverter.FormatPOTCOFloat(obj.intensity.Value))}");
            }
            
            if (obj.attenuation.HasValue)
            {
                properties.Add($"{indent}'Attenuation': {CoordinateConverter.StringToPython(CoordinateConverter.FormatPOTCOFloat(obj.attenuation.Value))}");
            }
            
            if (obj.coneAngle.HasValue)
            {
                properties.Add($"{indent}'ConeAngle': {CoordinateConverter.StringToPython(CoordinateConverter.FormatPOTCOFloat(obj.coneAngle.Value))}");
            }
            
            if (obj.dropOff.HasValue)
            {
                properties.Add($"{indent}'DropOff': {CoordinateConverter.StringToPython(CoordinateConverter.FormatPOTCOFloat(obj.dropOff.Value))}");
            }
            
            if (obj.flickering.HasValue)
            {
                properties.Add($"{indent}'Flickering': {CoordinateConverter.BoolToPython(obj.flickering.Value)}");
            }
            
            if (obj.flickRate.HasValue)
            {
                properties.Add($"{indent}'FlickRate': {CoordinateConverter.FormatPOTCOFloat(obj.flickRate.Value)}");
            }
        }
        
        private static void GenerateVisualProperties(List<string> properties, ExportedObject obj, string indent, ExportSettings settings, bool needsComma = true)
        {
            var visualContent = new StringBuilder();
            var visualItems = new List<string>();
            
            DebugLogger.LogWorldExporter($"🎨 Generating Visual properties for '{obj.name}' - Color: {(obj.visualColor.HasValue ? "Yes" : "No")}, Model: '{obj.modelPath ?? "null"}'");
            
            // Visual color (if present, comes first in POTCO format)
            if (obj.visualColor.HasValue)
            {
                string colorTuple = CoordinateConverter.UnityToPanda3DColor(obj.visualColor.Value);
                visualItems.Add($"'Color': {colorTuple}");
                DebugLogger.LogWorldExporter($"🎨 Added Color to Visual block: {colorTuple}");
            }
            
            // Model path (only if present and not null)
            if (!string.IsNullOrEmpty(obj.modelPath))
            {
                visualItems.Add($"'Model': {CoordinateConverter.StringToPython(obj.modelPath)}");
                DebugLogger.LogWorldExporter($"📦 Added Model to Visual block: {obj.modelPath}");
            }
            else
            {
                DebugLogger.LogWarningWorldExporter($"⚠️ Object '{obj.name}' has no model path - Visual block will be missing Model!");
            }
            
            DebugLogger.LogWorldExporter($"🔍 Visual items count: {visualItems.Count}");
            
            // Create Visual block if we have visual properties OR if this should be a visual object
            bool shouldHaveVisual = visualItems.Count > 0 || (!string.IsNullOrEmpty(obj.modelPath) && obj.objectType != "Collision Barrier" && obj.objectType != "Locator Node");
            
            // Don't add default white color - POTCO files only include Color when it's non-default
            // The original files omit Color property when it's the default white (1,1,1,1)
            
            // Create Visual block if we have visual items OR if we need to add a Model
            if (shouldHaveVisual)
            {
                visualContent.Append($"{indent}'Visual': {{");
                
                for (int i = 0; i < visualItems.Count; i++)
                {
                    if (i > 0 || visualItems.Count > 1)
                    {
                        visualContent.AppendLine();
                        visualContent.Append($"{indent}    ");
                    }
                    else
                    {
                        visualContent.AppendLine();
                        visualContent.Append($"{indent}    ");
                    }
                    
                    bool isLastItem = i == visualItems.Count - 1;
                    visualContent.Append($"{visualItems[i]}{(isLastItem ? "" : ",")}");
                }
                
                visualContent.Append($" }}{(needsComma ? " }," : " }")}"); // Comma only if needed
                
                // Add the complete Visual block as a single property
                properties.Add(visualContent.ToString());
            }
        }
        
        private static string FormatCustomProperty(object value)
        {
            switch (value)
            {
                case string str:
                    return CoordinateConverter.StringToPython(str);
                case bool boolean:
                    return CoordinateConverter.BoolToPython(boolean);
                case float floatVal:
                    return CoordinateConverter.FormatPOTCOFloat(floatVal);
                case int intVal:
                    return intVal.ToString();
                case Vector3 vec:
                    return CoordinateConverter.FormatPanda3DVector3(vec);
                case Color color:
                    return CoordinateConverter.UnityToPanda3DColor(color);
                default:
                    return CoordinateConverter.StringToPython(value.ToString());
            }
        }
    }
}