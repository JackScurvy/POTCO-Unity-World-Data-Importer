using UnityEngine;

namespace CaveGenerator.Data
{
    [System.Serializable]
    public class GenerationSettings
    {
        public int caveLength = 10;
        public float generationDelay = 0.1f;
        public bool capOpenEnds = true;
        public bool useEggFiles = false;
        public bool preventOverlaps = true;
        public float overlapCheckRadius = 5f;
        public int maxBranches = 3;
        public float branchProbability = 0.3f;
        public bool enableBranching = true;
        public int maxDepth = 8;
        public bool allowLoops = false;
        public int seed = -1; // -1 for random
        public bool visualizeConnectors = false;
        public bool realtimePreview = true;
    }
}