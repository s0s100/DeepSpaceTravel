using Core.Player;
using System;

namespace Config
{
    [Serializable]
    public struct ControlConfig
    {
        public float minTouchDistance;
        public MovementType movementType;

        public float fCoefficient;
        public float cCoefficient;
        public float rCoefficient;
    }
}