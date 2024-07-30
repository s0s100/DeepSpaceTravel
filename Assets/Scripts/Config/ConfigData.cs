using System;

namespace Config
{
    [Serializable]
    public struct ConfigData
    {
        public PlayerControlConfig PlayerControlConfig;
        public BackgroundConfig BackgroundConfig;
        public GenerationConfig GenerationConfig;
    }
}