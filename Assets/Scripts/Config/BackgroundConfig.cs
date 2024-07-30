using System;
using UnityEngine;

namespace Config
{
    [Serializable]
    public struct BackgroundConfig
    {
        public float cloudSizeResize;
        public float defaultGenerationShift;
        public float foregroundGenerationTime;
        public float foregroundExistanceTime;
        public float foregroundMovementSpeed;

        public Sprite backgroundSprite;
        public Sprite[] foregroundSprites;
    }
}