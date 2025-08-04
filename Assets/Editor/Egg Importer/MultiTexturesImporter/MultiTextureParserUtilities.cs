using UnityEngine;
using System;
using System.Globalization;
using System.Text;
using POTCO.Editor;

public class MultiTextureParserUtilities
{
    // Cache commonly used separators to avoid allocating new arrays
    private static readonly char[] SpaceSeparator = { ' ' };
    private static readonly char[] WhitespaceSeparators = { ' ', '\n', '\r', '\t' };
    private static readonly char[] SpaceNewlineCarriageReturnSeparators = { ' ', '\n', '\r' };
    
    // Reusable StringBuilder for string concatenation
    private static readonly StringBuilder StringBuilderCache = new StringBuilder();
    public int FindMatchingBrace(string[] lines, int startLine)
    {
        int braceDepth = 0;
        for (int i = startLine; i < lines.Length; i++)
        {
            foreach (char c in lines[i])
            {
                if (c == '{') braceDepth++;
                if (c == '}') braceDepth--;
            }
            if (braceDepth == 0 && i >= startLine) return i;
        }
        return -1;
    }

    public string GetGroupName(string line)
    {
        var parts = line.Split(SpaceSeparator, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length <= 1) return "unnamed";
        
        string name = parts[1];
        // Handle case where the group line is just "<Group> {"
        if (name == "{") return "unnamed";
        
        return name;
    }

    public void ParseTransformMatrix(string[] lines, int start, int end, ref Matrix4x4 matrix)
    {
        matrix = Matrix4x4.identity;
        
        for (int i = start; i <= end; i++)
        {
            string line = lines[i].Trim();
            if (line.StartsWith("<Matrix4>"))
            {
                int matrixStart = line.IndexOf('{');
                int matrixEnd = line.LastIndexOf('}');
                
                if (matrixStart != -1 && matrixEnd != -1)
                {
                    string matrixData = line.Substring(matrixStart + 1, matrixEnd - matrixStart - 1).Trim();
                    ParseMatrix4x4String(matrixData, ref matrix);
                }
                else
                {
                    // Multi-line matrix - use StringBuilder for better performance
                    StringBuilderCache.Clear();
                    for (int j = i; j <= end && j < lines.Length; j++)
                    {
                        StringBuilderCache.Append(lines[j]).Append(' ');
                        if (lines[j].Contains("}"))
                        {
                            string fullMatrix = StringBuilderCache.ToString();
                            int mStart = fullMatrix.IndexOf('{');
                            int mEnd = fullMatrix.LastIndexOf('}');
                            if (mStart != -1 && mEnd != -1)
                            {
                                string matrixData = fullMatrix.Substring(mStart + 1, mEnd - mStart - 1).Trim();
                                ParseMatrix4x4String(matrixData, ref matrix);
                            }
                            break;
                        }
                    }
                }
                break;
            }
        }
    }

    private void ParseMatrix4x4String(string matrixData, ref Matrix4x4 matrix)
    {
        var values = matrixData.Split(WhitespaceSeparators, StringSplitOptions.RemoveEmptyEntries);
        if (values.Length >= 16)
        {
            for (int idx = 0; idx < 16; idx++)
            {
                if (float.TryParse(values[idx], NumberStyles.Float, CultureInfo.InvariantCulture, out float val))
                {
                    matrix[idx / 4, idx % 4] = val;
                }
            }
        }
    }

    public void ApplyMatrix4x4ToTransform(Transform transform, Matrix4x4 matrix)
    {
        if (!IsValidMatrix(matrix))
        {
            DebugLogger.LogWarningEggImporter($"Invalid matrix for transform {transform.name}, using identity");
            matrix = Matrix4x4.identity;
        }

        Vector3 position = new Vector3(matrix.m03, matrix.m13, matrix.m23);
        Vector3 scale = new Vector3(
            new Vector3(matrix.m00, matrix.m10, matrix.m20).magnitude,
            new Vector3(matrix.m01, matrix.m11, matrix.m21).magnitude,
            new Vector3(matrix.m02, matrix.m12, matrix.m22).magnitude
        );

        Matrix4x4 rotMatrix = matrix;
        rotMatrix.m03 = rotMatrix.m13 = rotMatrix.m23 = 0;
        rotMatrix.m33 = 1;

        for (int col = 0; col < 3; col++)
        {
            if (scale[col] > 0.0001f)
            {
                rotMatrix[0, col] /= scale[col];
                rotMatrix[1, col] /= scale[col];
                rotMatrix[2, col] /= scale[col];
            }
        }

        Quaternion rotation = ExtractRotation(rotMatrix);

        // Apply Panda3D to Unity coordinate conversion
        Vector3 unityPos = new Vector3(position.x, position.z, position.y);
        Vector3 unityScale = new Vector3(scale.x, scale.z, scale.y);
        Quaternion unityRot = new Quaternion(rotation.x, rotation.z, rotation.y, -rotation.w);

        if (!IsValidVector(unityPos)) unityPos = Vector3.zero;
        if (!IsValidVector(unityScale) || unityScale.magnitude < 0.0001f) unityScale = Vector3.one;
        if (!IsValidQuaternion(unityRot)) unityRot = Quaternion.identity;

        transform.localPosition = unityPos;
        transform.localRotation = unityRot;
        transform.localScale = unityScale;
    }

    private Quaternion ExtractRotation(Matrix4x4 matrix)
    {
        float tr = matrix.m00 + matrix.m11 + matrix.m22;
        float qw, qx, qy, qz;

        if (tr > 0)
        {
            float s = 0.5f / Mathf.Sqrt(tr + 1.0f);
            qw = 0.25f / s;
            qx = (matrix.m21 - matrix.m12) * s;
            qy = (matrix.m02 - matrix.m20) * s;
            qz = (matrix.m10 - matrix.m01) * s;
        }
        else if ((matrix.m00 > matrix.m11) && (matrix.m00 > matrix.m22))
        {
            float s = 2.0f * Mathf.Sqrt(1.0f + matrix.m00 - matrix.m11 - matrix.m22);
            qw = (matrix.m21 - matrix.m12) / s;
            qx = 0.25f * s;
            qy = (matrix.m01 + matrix.m10) / s;
            qz = (matrix.m02 + matrix.m20) / s;
        }
        else if (matrix.m11 > matrix.m22)
        {
            float s = 2.0f * Mathf.Sqrt(1.0f + matrix.m11 - matrix.m00 - matrix.m22);
            qw = (matrix.m02 - matrix.m20) / s;
            qx = (matrix.m01 + matrix.m10) / s;
            qy = 0.25f * s;
            qz = (matrix.m12 + matrix.m21) / s;
        }
        else
        {
            float s = 2.0f * Mathf.Sqrt(1.0f + matrix.m22 - matrix.m00 - matrix.m11);
            qw = (matrix.m10 - matrix.m01) / s;
            qx = (matrix.m02 + matrix.m20) / s;
            qy = (matrix.m12 + matrix.m21) / s;
            qz = 0.25f * s;
        }

        return new Quaternion(qx, qy, qz, qw);
    }

    public bool IsValidMatrix(Matrix4x4 matrix)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                float val = matrix[i, j];
                if (float.IsNaN(val) || float.IsInfinity(val))
                    return false;
            }
        }

        float det = matrix.determinant;
        if (Mathf.Abs(det) < 0.0001f || float.IsNaN(det) || float.IsInfinity(det))
            return false;

        return true;
    }

    public bool IsValidVector(Vector3 vector)
    {
        return !float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector.z) &&
               !float.IsInfinity(vector.x) && !float.IsInfinity(vector.y) && !float.IsInfinity(vector.z);
    }

    public bool IsValidQuaternion(Quaternion quat)
    {
        return !float.IsNaN(quat.x) && !float.IsNaN(quat.y) && !float.IsNaN(quat.z) && !float.IsNaN(quat.w) &&
               !float.IsInfinity(quat.x) && !float.IsInfinity(quat.y) && !float.IsInfinity(quat.z) && !float.IsInfinity(quat.w) &&
               Mathf.Abs(quat.x * quat.x + quat.y * quat.y + quat.z * quat.z + quat.w * quat.w - 1f) < 0.01f;
    }

    public Vector3 ParseVector3(string line)
    {
        try
        {
            int openBrace = line.IndexOf('{');
            int closeBrace = line.LastIndexOf('}');
            if (openBrace == -1 || closeBrace == -1) return Vector3.zero;
            
            string valuesString = line.Substring(openBrace + 1, closeBrace - openBrace - 1).Trim();
            var parts = valuesString.Split(SpaceSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 3)
            {
                return new Vector3(
                    float.Parse(parts[0], CultureInfo.InvariantCulture),
                    float.Parse(parts[1], CultureInfo.InvariantCulture),
                    float.Parse(parts[2], CultureInfo.InvariantCulture)
                );
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to parse Vector3 from line: " + line + "\n" + e);
        }
        return Vector3.zero;
    }

    public Quaternion ParseAngleAxis(string line)
    {
        try
        {
            int openBrace = line.IndexOf('{');
            int closeBrace = line.LastIndexOf('}');
            if (openBrace == -1 || closeBrace == -1) return Quaternion.identity;
            
            string valuesString = line.Substring(openBrace + 1, closeBrace - openBrace - 1).Trim();
            var parts = valuesString.Split(SpaceSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 4)
            {
                float angle = float.Parse(parts[0], CultureInfo.InvariantCulture);
                Vector3 axis = new Vector3(
                    float.Parse(parts[1], CultureInfo.InvariantCulture),
                    float.Parse(parts[2], CultureInfo.InvariantCulture),
                    float.Parse(parts[3], CultureInfo.InvariantCulture)
                );
                return Quaternion.AngleAxis(angle, axis);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to parse AngleAxis from line: " + line + "\n" + e);
        }
        return Quaternion.identity;
    }

    public Matrix4x4 ParseMatrix4(string[] lines, ref int i)
    {
        int blockEnd = FindMatchingBrace(lines, i);
        if (blockEnd == -1) return Matrix4x4.identity;
        i++;
        var matrix = Matrix4x4.identity;
        int row = 0;
        while (i < blockEnd && row < 4)
        {
            string line = lines[i].Trim();
            if (!string.IsNullOrEmpty(line))
            {
                var values = line.Split(SpaceSeparator, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length >= 4)
                {
                    for (int col = 0; col < 4; col++)
                    {
                        if (float.TryParse(values[col], NumberStyles.Float, CultureInfo.InvariantCulture, out float value)) { matrix[row, col] = value; }
                    }
                    row++;
                }
            }
            i++;
        }
        i = blockEnd;
        return matrix.transpose;
    }

    public Quaternion QuaternionFromMatrix(Matrix4x4 m)
    {
        // Safe quaternion extraction from rotation matrix
        Quaternion q = new Quaternion();
        try
        {
            float trace = m.m00 + m.m11 + m.m22;
            if (trace > 0)
            {
                float s = Mathf.Sqrt(trace + 1.0f);
                q.w = s * 0.5f;
                s = 0.5f / s;
                q.x = (m.m21 - m.m12) * s;
                q.y = (m.m02 - m.m20) * s;
                q.z = (m.m10 - m.m01) * s;
            }
            else
            {
                if (m.m00 > m.m11 && m.m00 > m.m22)
                {
                    float s = Mathf.Sqrt(1.0f + m.m00 - m.m11 - m.m22);
                    q.x = s * 0.5f;
                    s = 0.5f / s;
                    q.y = (m.m01 + m.m10) * s;
                    q.z = (m.m02 + m.m20) * s;
                    q.w = (m.m21 - m.m12) * s;
                }
                else if (m.m11 > m.m22)
                {
                    float s = Mathf.Sqrt(1.0f + m.m11 - m.m00 - m.m22);
                    q.y = s * 0.5f;
                    s = 0.5f / s;
                    q.x = (m.m01 + m.m10) * s;
                    q.z = (m.m12 + m.m21) * s;
                    q.w = (m.m02 - m.m20) * s;
                }
                else
                {
                    float s = Mathf.Sqrt(1.0f + m.m22 - m.m00 - m.m11);
                    q.z = s * 0.5f;
                    s = 0.5f / s;
                    q.x = (m.m02 + m.m20) * s;
                    q.y = (m.m12 + m.m21) * s;
                    q.w = (m.m10 - m.m01) * s;
                }
            }
            // Normalize the quaternion
            float magnitude = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
            if (magnitude > 0.0001f)
            {
                q.x /= magnitude;
                q.y /= magnitude;
                q.z /= magnitude;
                q.w /= magnitude;
            }
            else
            {
                q = Quaternion.identity;
            }
        }
        catch
        {
            q = Quaternion.identity;
        }
        return q;
    }
}