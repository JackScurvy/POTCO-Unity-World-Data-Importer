using UnityEngine;
using System.Collections.Generic;

public class EggVertex
{
    public Vector3 position;
    public Vector3 normal;
    public Vector2 uv;
    public Color color = Color.white;
    public Dictionary<string, float> boneWeights = new Dictionary<string, float>();
}