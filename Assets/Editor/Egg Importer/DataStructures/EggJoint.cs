using UnityEngine;
using System.Collections.Generic;

public class EggJoint
{
    public string name;
    public Matrix4x4 transform;
    public Matrix4x4 defaultPose;
    public EggJoint parent;
    public List<EggJoint> children = new List<EggJoint>();
    public Dictionary<int, float> vertexWeights = new Dictionary<int, float>();
}