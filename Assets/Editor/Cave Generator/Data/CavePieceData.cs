using UnityEngine;
using System.Collections.Generic;

namespace CaveGenerator.Data
{
    public class ConnectorInfo
    {
        public Transform connector;
        public string type = "default";
        public bool isUsed = false;
        public Transform connectedTo;
        public Vector3 direction;
    }
    
    [System.Serializable]
    public class CavePieceNode
    {
        public GameObject piece;
        public Vector3 position;
        public Quaternion rotation;
        public List<Transform> connectors;
        public int depth;
        public bool isDeadEnd;
    }
}