using Core;
using System;

namespace Config
{
    [Serializable]
    public struct GenerationConfig
    {
        public GenerationPattern[] generationPatterns;
        public float screenSpawnShift;
    }
}