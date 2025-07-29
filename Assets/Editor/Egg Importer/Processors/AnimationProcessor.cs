using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

public class AnimationProcessor
{
    private ParserUtilities _parserUtils;
    
    // Cache commonly used separators to avoid repeated allocations
    private static readonly char[] SpaceSeparator = { ' ' };
    private static readonly char[] WhitespaceSeparators = { ' ', '\n', '\r', '\t' };
    private static readonly char[] SpaceTabSeparators = { ' ', '\t' };
    
    // Reusable StringBuilder for string concatenation
    private static readonly StringBuilder StringBuilderCache = new StringBuilder();
    
    public AnimationProcessor()
    {
        _parserUtils = new ParserUtilities();
    }

    public void ParseAnimations(string[] lines, GameObject rootGO, AssetImportContext ctx, GameObject rootBoneObject)
    {
        Debug.Log("🎬 ANIMATION: Starting animation parsing");

        int bundleCount = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (line.StartsWith("<Bundle>"))
            {
                bundleCount++;
                var parts = line.Split(SpaceSeparator, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                {
                    string bundleName = parts[1];
                    Debug.Log($"🎬 ANIMATION: Found animation bundle #{bundleCount}: '{bundleName}'");

                    var clip = new AnimationClip { name = bundleName + "_anim" };
                    Debug.Log($"🎬 ANIMATION: Created clip: '{clip.name}'");

                    string armaturePath = rootBoneObject != null ? rootBoneObject.name : "";
                    Debug.Log($"🎬 ANIMATION: Armature path: '{armaturePath}'");

                    int bundleEnd = _parserUtils.FindMatchingBrace(lines, i);
                    if (bundleEnd != -1)
                    {
                        Debug.Log($"🎬 ANIMATION: Bundle spans lines {i} to {bundleEnd}");

                        ParseAnimationBundle(lines, i + 1, bundleEnd, clip, armaturePath);

                        clip.wrapMode = WrapMode.Loop;
                        clip.legacy = false;

                        Debug.Log($"🎬 ANIMATION: Configured clip - WrapMode: {clip.wrapMode}, Legacy: {clip.legacy}");

                        var curveBindings = AnimationUtility.GetCurveBindings(clip);
                        Debug.Log($"🎬 ANIMATION: Clip has {curveBindings.Length} curve bindings");

                        if (curveBindings.Length > 0)
                        {
                            foreach (var binding in curveBindings)
                            {
                                Debug.Log($"🎬 ANIMATION: Curve - Path: '{binding.path}', Property: '{binding.propertyName}', Type: {binding.type}");
                            }

                            Debug.Log($"🎬 ANIMATION: Adding clip '{clip.name}' to asset context");
                            ctx.AddObjectToAsset(clip.name, clip);

                            Debug.Log($"🎬 ANIMATION: Starting controller creation...");
                            CreateAnimatorControllerForClip(clip, rootGO, ctx);

                            Debug.Log($"🎮 ANIMATION: SUCCESS! Animation '{clip.name}' is ready to use!");
                        }
                        else
                        {
                            Debug.LogWarning($"❌ ANIMATION: Animation clip '{clip.name}' has no curves - skipping");

                            Debug.Log("🔍 ANIMATION: Debugging clip creation...");
                            if (clip == null)
                            {
                                Debug.LogError("🔍 ANIMATION: Clip is null!");
                            }
                            else
                            {
                                Debug.Log($"🔍 ANIMATION: Clip exists: {clip.name}");
                                Debug.Log($"🔍 ANIMATION: Clip length: {clip.length}");
                                Debug.Log($"🔍 ANIMATION: Clip frameRate: {clip.frameRate}");
                            }
                        }

                        i = bundleEnd;
                    }
                    else
                    {
                        Debug.LogError($"❌ ANIMATION: Could not find matching brace for bundle at line {i}");
                        i++;
                    }
                }
                else
                {
                    Debug.LogWarning($"⚠️ ANIMATION: Bundle line malformed: '{line}'");
                    i++;
                }
            }
            else
            {
                i++;
            }
        }

        Debug.Log($"🎬 ANIMATION: Completed. Found {bundleCount} bundles total");
        }

    public void ParseBundleBonesAndAnimations(string[] lines, int start, int end, EggJoint parentJoint, string currentPath, AnimationClip clip, Dictionary<string, EggJoint> joints)
    {
        Debug.Log($"🔍 BUNDLE BONES: Parsing from line {start} to {end}, path: '{currentPath}'");

        int i = start;
        while (i < end)
        {
            string line = lines[i].Trim();
            
            if (line.StartsWith("<Joint>"))
            {
                var parts = line.Split(SpaceSeparator, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                {
                    string jointName = parts[1];
                    string jointPath = string.IsNullOrEmpty(currentPath) ? jointName : currentPath + "/" + jointName;
                    
                    Debug.Log($"🦴 BUNDLE: Found joint '{jointName}' at path '{jointPath}'");

                    EggJoint joint = new EggJoint
                    {
                        name = jointName,
                        parent = parentJoint,
                        transform = Matrix4x4.identity,
                        defaultPose = Matrix4x4.identity
                    };

                    if (parentJoint != null)
                    {
                        parentJoint.children.Add(joint);
                    }

                    joints[jointName] = joint;

                    int jointEnd = _parserUtils.FindMatchingBrace(lines, i);
                    if (jointEnd != -1)
                    {
                        ParseBoneContentAndAnimation(lines, i + 1, jointEnd, joint, jointPath, clip);
                        ParseBundleBonesAndAnimations(lines, i + 1, jointEnd, joint, jointPath, clip, joints);
                        i = jointEnd + 1;
                    }
                    else
                    {
                        Debug.LogError($"❌ BUNDLE: Could not find matching brace for joint at line {i}");
                        i++;
                    }
                }
                else
                {
                    i++;
                }
            }
            else if (line.StartsWith("<Xfm$Anim_S$>"))
            {
                int xfmEnd = _parserUtils.FindMatchingBrace(lines, i);
                if (xfmEnd != -1)
                {
                    ParseXfmAnim(lines, i + 1, xfmEnd, clip, currentPath);
                    i = xfmEnd + 1;
                }
                else
                {
                    Debug.LogError($"❌ BUNDLE: Could not find matching brace for Xfm$Anim_S$ at line {i}");
                    i++;
                }
            }
            else
            {
                i++;
            }
        }
        }

    private void ParseAnimationBundle(string[] lines, int start, int end, AnimationClip clip, string currentPath)
    {
        Debug.Log($"📦 BUNDLE: Parsing bundle from line {start} to {end}, currentPath: '{currentPath}'");

        int i = start;
        int tableCount = 0;

        while (i < end)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line))
            {
                i++;
                continue;
            }

            if (line.StartsWith("<Table>"))
            {
                tableCount++;
                var parts = line.Split(SpaceSeparator, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                {
                    string tableName = parts[1].Trim('"');
                    Debug.Log($"📦 BUNDLE: Found table #{tableCount}: '{tableName}'");

                    if (tableName == "<skeleton>")
                    {
                        Debug.Log($"📦 BUNDLE: Entering skeleton table");
                        int tableEnd = _parserUtils.FindMatchingBrace(lines, i);
                        if (tableEnd != -1)
                        {
                            ParseAnimationBundle(lines, i + 1, tableEnd, clip, currentPath);
                            i = tableEnd + 1;
                        }
                        else
                        {
                            Debug.LogError($"❌ BUNDLE: Could not find matching brace for skeleton table at line {i}");
                            i++;
                        }
                    }
                    else
                    {
                        string bonePath = string.IsNullOrEmpty(currentPath) ? tableName : currentPath + "/" + tableName;
                        Debug.Log($"📦 BUNDLE: Processing bone table '{tableName}' with path '{bonePath}'");

                        int tableEnd = _parserUtils.FindMatchingBrace(lines, i);
                        if (tableEnd != -1)
                        {
                            ParseBoneAnimationTable(lines, i + 1, tableEnd, clip, bonePath);
                            i = tableEnd + 1;
                        }
                        else
                        {
                            Debug.LogError($"❌ BUNDLE: Could not find matching brace for bone table '{tableName}' at line {i}");
                            i++;
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"⚠️ BUNDLE: Table line malformed: '{line}'");
                    i++;
                }
            }
            else
            {
                i++;
            }
        }

        Debug.Log($"📦 BUNDLE: Completed. Processed {tableCount} tables");
        }

    private void ParseBoneAnimationTable(string[] lines, int start, int end, AnimationClip clip, string bonePath)
    {
        Debug.Log($"🦴 BONE: Parsing bone '{bonePath}' from line {start} to {end}");

        int i = start;
        int xfmCount = 0;
        int childTableCount = 0;

        while (i < end)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line))
            {
                i++;
                continue;
            }

            if (line.StartsWith("<Table>"))
            {
                childTableCount++;
                var parts = line.Split(SpaceSeparator, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                {
                    string tableName = parts[1].Trim('"');
                    string childPath = bonePath + "/" + tableName;
                    Debug.Log($"🦴 BONE: Found child table #{childTableCount}: '{tableName}' -> '{childPath}'");

                    int tableEnd = _parserUtils.FindMatchingBrace(lines, i);
                    if (tableEnd != -1)
                    {
                        ParseBoneAnimationTable(lines, i + 1, tableEnd, clip, childPath);
                        i = tableEnd + 1;
                    }
                    else
                    {
                        Debug.LogError($"❌ BONE: Could not find matching brace for child table '{tableName}' at line {i}");
                        i++;
                    }
                }
                else
                {
                    Debug.LogWarning($"⚠️ BONE: Child table line malformed: '{line}'");
                    i++;
                }
            }
            else if (line.StartsWith("<Xfm$Anim_S$>"))
            {
                xfmCount++;
                Debug.Log($"🦴 BONE: Found transform animation #{xfmCount} for bone: {bonePath}");
                int xfmEnd = _parserUtils.FindMatchingBrace(lines, i);
                if (xfmEnd != -1)
                {
                    ParseXfmAnim(lines, i + 1, xfmEnd, clip, bonePath);
                    i = xfmEnd + 1;
                }
                else
                {
                    Debug.LogError($"❌ BONE: Could not find matching brace for Xfm$Anim_S$ at line {i}");
                    i++;
                }
            }
            else
            {
                i++;
            }
        }

        Debug.Log($"🦴 BONE: Completed '{bonePath}'. Found {xfmCount} transforms, {childTableCount} child tables");
        }

    private void ParseBoneContentAndAnimation(string[] lines, int start, int end, EggJoint joint, string bonePath, AnimationClip clip)
    {
        Debug.Log($"🔍 BONE CONTENT: Parsing bone '{joint.name}' from line {start} to {end}");

        for (int i = start; i < end; i++)
        {
            string line = lines[i].Trim();

            if (line.StartsWith("<Transform>"))
            {
                Debug.Log($"🔍 BONE CONTENT: Found transform for joint '{joint.name}'");
                int transEnd = _parserUtils.FindMatchingBrace(lines, i);
                if (transEnd != -1)
                {
                    _parserUtils.ParseTransformMatrix(lines, i + 1, transEnd, ref joint.transform);
                    joint.defaultPose = joint.transform;
                    Debug.Log($"🔍 BONE CONTENT: Set transform for joint '{joint.name}'");
                }
            }
            else if (line.StartsWith("<Xfm$Anim_S$>"))
            {
                Debug.Log($"🔍 BONE CONTENT: Found animation data for joint '{joint.name}'");
                int xfmEnd = _parserUtils.FindMatchingBrace(lines, i);
                if (xfmEnd != -1)
                {
                    ParseXfmAnim(lines, i + 1, xfmEnd, clip, bonePath);
                    i = xfmEnd;
                }
            }
        }
        }

    private void ParseXfmAnim(string[] lines, int start, int end, AnimationClip clip, string bonePath)
    {
        Debug.Log($"🔄 TRANSFORM: Parsing transform animation for '{bonePath}' from line {start} to {end}");

        float fps = 24f;
        // Pre-size channels dictionary based on typical animation channels (x,y,z,h,p,r,i,j,k)
        var channels = new Dictionary<string, List<float>>(9);
        int numKeyframes = 0;

        int i = start;
        while (i < end)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line))
            {
                i++;
                continue;
            }

            if (line.StartsWith("<Scalar> fps"))
            {
                int openBrace = line.IndexOf('{');
                int closeBrace = line.LastIndexOf('}');
                if (openBrace != -1 && closeBrace != -1)
                {
                    string val = line.Substring(openBrace + 1, closeBrace - openBrace - 1).Trim();
                    if (float.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsedFps))
                    {
                        fps = parsedFps;
                        Debug.Log($"🔄 TRANSFORM: Set FPS to {fps}");
                    }
                }
                i++;
            }
            else if (line.StartsWith("<S$Anim>"))
            {
                var headerParts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string channelName = headerParts.Length > 1 ? headerParts[1] : "UNKNOWN";
                Debug.Log($"🔄 TRANSFORM: Processing channel '{channelName}'");

                if (line.Contains("<V>") && line.Contains("}"))
                {
                    Debug.Log($"🔄 TRANSFORM: Single-line format detected for channel '{channelName}'");

                    int vStart = line.IndexOf("<V>");
                    if (vStart != -1)
                    {
                        string vPart = line.Substring(vStart);
                        int firstBrace = vPart.IndexOf('{');
                        int lastBrace = vPart.LastIndexOf('}');

                        if (firstBrace != -1 && lastBrace != -1 && lastBrace > firstBrace)
                        {
                            string valuesString = vPart.Substring(firstBrace + 1, lastBrace - firstBrace - 1).Trim();
                            Debug.Log($"🔄 TRANSFORM: Extracted single-line values: '{valuesString}'");

                            if (!string.IsNullOrWhiteSpace(valuesString))
                            {
                                var stringValues = valuesString.Split(SpaceTabSeparators, StringSplitOptions.RemoveEmptyEntries);
                                // Pre-size values list based on typical keyframe counts (estimate 60-120 frames)
                                var values = new List<float>(stringValues.Length);

                                foreach (string sv in stringValues)
                                {
                                    if (float.TryParse(sv, NumberStyles.Float, CultureInfo.InvariantCulture, out float val))
                                    {
                                        values.Add(val);
                                    }
                                }

                                if (values.Count > 0)
                                {
                                    channels[channelName] = values;
                                    if (values.Count > numKeyframes)
                                        numKeyframes = values.Count;
                                    Debug.Log($"✅ TRANSFORM: Parsed {values.Count} keyframes for single-line channel '{channelName}'");
                                }
                            }
                        }
                    }
                    i++;
                }
                else
                {
                    Debug.Log($"🔄 TRANSFORM: Multi-line format for channel '{channelName}'");

                    int sAnimEnd = _parserUtils.FindMatchingBrace(lines, i);
                    if (sAnimEnd == -1)
                    {
                        Debug.LogError($"❌ TRANSFORM: Could not find matching brace for S$Anim at line {i}");
                        i++;
                        continue;
                    }

                    bool foundVBlock = false;
                    for (int j = i + 1; j <= sAnimEnd; j++)
                    {
                        string vLine = lines[j].Trim();
                        if (vLine.StartsWith("<V>"))
                        {
                            foundVBlock = true;
                            Debug.Log($"🔄 TRANSFORM: Found <V> block at line {j}");

                            int vEnd = _parserUtils.FindMatchingBrace(lines, j);
                            if (vEnd != -1)
                            {
                                Debug.Log($"🔄 TRANSFORM: <V> block spans lines {j} to {vEnd}");

                                // Use StringBuilder for efficient string concatenation
                                StringBuilderCache.Clear();
                                for (int k = j; k <= vEnd; k++)
                                {
                                    StringBuilderCache.Append(lines[k]).Append(' ');
                                }
                                string fullVBlock = StringBuilderCache.ToString();

                                int lastOpenBrace = fullVBlock.LastIndexOf('{');
                                int firstCloseBraceAfter = fullVBlock.IndexOf('}', lastOpenBrace);

                                if (lastOpenBrace != -1 && firstCloseBraceAfter != -1)
                                {
                                    string valuesString = fullVBlock.Substring(lastOpenBrace + 1, firstCloseBraceAfter - lastOpenBrace - 1).Trim();
                                    Debug.Log($"🔄 TRANSFORM: Extracted multi-line values (length {valuesString.Length})");

                                    if (!string.IsNullOrWhiteSpace(valuesString))
                                    {
                                        var stringValues = valuesString.Split(WhitespaceSeparators, StringSplitOptions.RemoveEmptyEntries);
                                        // Pre-size values list based on parsed string count
                                        var values = new List<float>(stringValues.Length);

                                        Debug.Log($"🔄 TRANSFORM: Found {stringValues.Length} string values to parse");

                                        foreach (string sv in stringValues)
                                        {
                                            if (float.TryParse(sv, NumberStyles.Float, CultureInfo.InvariantCulture, out float val))
                                            {
                                                values.Add(val);
                                            }
                                        }

                                        if (values.Count > 0)
                                        {
                                            channels[channelName] = values;
                                            if (values.Count > numKeyframes)
                                                numKeyframes = values.Count;
                                            Debug.Log($"✅ TRANSFORM: Parsed {values.Count} keyframes for multi-line channel '{channelName}'");
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }

                    if (!foundVBlock)
                    {
                        Debug.LogWarning($"⚠️ TRANSFORM: No <V> block found for channel '{channelName}'");
                    }

                    i = sAnimEnd + 1;
                }
            }
            else
            {
                i++;
            }
        }

        Debug.Log($"🔄 TRANSFORM: Finished parsing. Found {channels.Count} channels, {numKeyframes} max keyframes");
        foreach (var channel in channels)
        {
            Debug.Log($"🔄 TRANSFORM: Channel '{channel.Key}' has {channel.Value.Count} values");
        }

        if (channels.Count > 0 && numKeyframes > 0)
        {
            Debug.Log($"🔄 TRANSFORM: Creating animation curves for bone '{bonePath}'");
            CreateAnimationCurvesForBone(clip, bonePath, channels, numKeyframes, fps);
        }
        else
        {
            Debug.LogWarning($"⚠️ TRANSFORM: No valid animation data found for bone '{bonePath}'");
        }
        }

    private void CreateAnimatorControllerForClip(AnimationClip clip, GameObject rootGO, AssetImportContext ctx)
    {
        Debug.Log($"🎯 LEGACY: Setting up legacy animation for clip '{clip.name}'");

        try
        {
            clip.legacy = true;
            clip.wrapMode = WrapMode.Loop;

            Debug.Log($"🎯 LEGACY: Configured clip as legacy with loop wrap mode");

            var animator = rootGO.GetComponent<Animator>();
            if (animator != null)
            {
                UnityEngine.Object.DestroyImmediate(animator);
                Debug.Log($"🎯 LEGACY: Removed Animator component");
            }

            var animationComponent = rootGO.GetComponent<Animation>();
            if (animationComponent == null)
            {
                animationComponent = rootGO.AddComponent<Animation>();
                Debug.Log($"🎯 LEGACY: Added Animation component");
            }

            animationComponent.AddClip(clip, clip.name);
            animationComponent.clip = clip;
            animationComponent.playAutomatically = true;

            Debug.Log($"✅ LEGACY: Complete! Legacy animation '{clip.name}' ready to play automatically");
            Debug.Log($"🎮 READY: Just drag '{rootGO.name}' into your scene and it will animate immediately!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ LEGACY: Exception during setup: {e.Message}\nStack: {e.StackTrace}");
        }
        }

    private void CreateAnimationCurvesForBone(AnimationClip clip, string bonePath, Dictionary<string, List<float>> channels, int numKeyframes, float fps)
    {
        Debug.Log($"📈 CURVES: Creating curves for bone '{bonePath}' with {numKeyframes} keyframes at {fps} fps");

        // Pre-size keyframe lists to avoid resizing during population
        var posXKeys = new List<Keyframe>(numKeyframes);
        var posYKeys = new List<Keyframe>(numKeyframes);
        var posZKeys = new List<Keyframe>(numKeyframes);
        var rotXKeys = new List<Keyframe>(numKeyframes);
        var rotYKeys = new List<Keyframe>(numKeyframes);
        var rotZKeys = new List<Keyframe>(numKeyframes);
        var rotWKeys = new List<Keyframe>(numKeyframes);
        var scaleXKeys = new List<Keyframe>(numKeyframes);
        var scaleYKeys = new List<Keyframe>(numKeyframes);
        var scaleZKeys = new List<Keyframe>(numKeyframes);

        // Cache channel lookups to avoid repeated dictionary access
        var xChannel = channels.TryGetValue("x", out var xList) ? xList : null;
        var yChannel = channels.TryGetValue("y", out var yList) ? yList : null;
        var zChannel = channels.TryGetValue("z", out var zList) ? zList : null;
        var hChannel = channels.TryGetValue("h", out var hList) ? hList : null;
        var pChannel = channels.TryGetValue("p", out var pList) ? pList : null;
        var rChannel = channels.TryGetValue("r", out var rList) ? rList : null;
        var iChannel = channels.TryGetValue("i", out var iList) ? iList : null;
        var jChannel = channels.TryGetValue("j", out var jList) ? jList : null;
        var kChannel = channels.TryGetValue("k", out var kList) ? kList : null;
        
        for (int k = 0; k < numKeyframes; k++)
        {
            float time = k / fps;

            float p_x = GetChannelValueDirect(xChannel, k);
            float p_y = GetChannelValueDirect(yChannel, k);
            float p_z = GetChannelValueDirect(zChannel, k);
            float p_h = GetChannelValueDirect(hChannel, k);
            float p_p = GetChannelValueDirect(pChannel, k);
            float p_r = GetChannelValueDirect(rChannel, k);
            float s_i = GetChannelValueDirect(iChannel, k, 1.0f);
            float s_j = GetChannelValueDirect(jChannel, k, 1.0f);
            float s_k = GetChannelValueDirect(kChannel, k, 1.0f);

            posXKeys.Add(new Keyframe(time, p_x));
            posYKeys.Add(new Keyframe(time, p_z));
            posZKeys.Add(new Keyframe(time, p_y));

            Quaternion pandaQuat = Quaternion.AngleAxis(p_h, Vector3.forward) *
                                   Quaternion.AngleAxis(p_p, Vector3.right) *
                                   Quaternion.AngleAxis(p_r, Vector3.up);
            Quaternion unityQuat = new Quaternion(pandaQuat.x, pandaQuat.z, pandaQuat.y, -pandaQuat.w);

            rotXKeys.Add(new Keyframe(time, unityQuat.x));
            rotYKeys.Add(new Keyframe(time, unityQuat.y));
            rotZKeys.Add(new Keyframe(time, unityQuat.z));
            rotWKeys.Add(new Keyframe(time, unityQuat.w));

            scaleXKeys.Add(new Keyframe(time, s_i));
            scaleYKeys.Add(new Keyframe(time, s_k));
            scaleZKeys.Add(new Keyframe(time, s_j));
        }

        int curvesAdded = 0;

        // Use cached channel references for faster checks
        if (xChannel != null || yChannel != null || zChannel != null)
        {
            clip.SetCurve(bonePath, typeof(Transform), "m_LocalPosition.x", new AnimationCurve(posXKeys.ToArray()));
            clip.SetCurve(bonePath, typeof(Transform), "m_LocalPosition.y", new AnimationCurve(posYKeys.ToArray()));
            clip.SetCurve(bonePath, typeof(Transform), "m_LocalPosition.z", new AnimationCurve(posZKeys.ToArray()));
            curvesAdded += 3;
            Debug.Log($"📈 CURVES: Set position curves for {bonePath}");
        }

        if (hChannel != null || pChannel != null || rChannel != null)
        {
            clip.SetCurve(bonePath, typeof(Transform), "m_LocalRotation.x", new AnimationCurve(rotXKeys.ToArray()));
            clip.SetCurve(bonePath, typeof(Transform), "m_LocalRotation.y", new AnimationCurve(rotYKeys.ToArray()));
            clip.SetCurve(bonePath, typeof(Transform), "m_LocalRotation.z", new AnimationCurve(rotZKeys.ToArray()));
            clip.SetCurve(bonePath, typeof(Transform), "m_LocalRotation.w", new AnimationCurve(rotWKeys.ToArray()));
            curvesAdded += 4;
            Debug.Log($"📈 CURVES: Set rotation curves for {bonePath}");
        }

        if (iChannel != null || jChannel != null || kChannel != null)
        {
            clip.SetCurve(bonePath, typeof(Transform), "m_LocalScale.x", new AnimationCurve(scaleXKeys.ToArray()));
            clip.SetCurve(bonePath, typeof(Transform), "m_LocalScale.y", new AnimationCurve(scaleYKeys.ToArray()));
            clip.SetCurve(bonePath, typeof(Transform), "m_LocalScale.z", new AnimationCurve(scaleZKeys.ToArray()));
            curvesAdded += 3;
            Debug.Log($"📈 CURVES: Set scale curves for {bonePath}");
        }

        Debug.Log($"📈 CURVES: Added {curvesAdded} curves for '{bonePath}' with orientation fix");
        }

    private float GetChannelValue(Dictionary<string, List<float>> channels, string channelName, int keyframeIndex, float defaultValue = 0f)
    {
        if (!channels.ContainsKey(channelName) || channels[channelName].Count == 0)
            return defaultValue;

        var channel = channels[channelName];
        int index = Mathf.Min(keyframeIndex, channel.Count - 1);
        return channel[index];
    }
    
    // Optimized version that works directly with cached channel lists
    private float GetChannelValueDirect(List<float> channel, int keyframeIndex, float defaultValue = 0f)
    {
        if (channel == null || channel.Count == 0)
            return defaultValue;

        int index = Mathf.Min(keyframeIndex, channel.Count - 1);
        return channel[index];
    }
}