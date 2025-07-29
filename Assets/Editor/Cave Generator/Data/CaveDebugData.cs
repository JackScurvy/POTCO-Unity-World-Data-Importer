using UnityEngine;
using System.Collections.Generic;

namespace CaveGenerator.Data
{
    [System.Serializable]
    public class DebugSnapshot
    {
        public string timestamp;
        public GenerationSettings settings;
        public List<DebugConnectionData> connections = new();
        public List<DebugPieceData> pieces = new();
        public string notes = "";
    }
    
    [System.Serializable]
    public class DebugConnectionData
    {
        public string fromPiece;
        public string toPiece;
        public string fromConnector;
        public string toConnector;
        public Vector3 fromPosition;
        public Vector3 toPosition;
        public Vector3 fromRotation;
        public Vector3 toRotation;
        public Vector3 fromDirection;
        public Vector3 toDirection;
        public float connectionDistance;
        public float angleDifference;
        public bool isCorrectlyAligned;
    }
    
    [System.Serializable]
    public class DebugPieceData
    {
        public string pieceName;
        public Vector3 position;
        public Vector3 rotation;
        public List<string> connectorNames = new();
        public List<Vector3> connectorPositions = new();
        public List<Vector3> connectorDirections = new();
    }
}