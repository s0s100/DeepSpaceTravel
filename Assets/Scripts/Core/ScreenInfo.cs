using UnityEngine;

namespace Core
{
    // Class to retreive screen information such as width, height, etc.
    public static class ScreenInfo
    {
        private static float maxXPos = 0.0f;
        private static float maxYPos = 0.0f;

        public static float GetMaxXPos()
        {
            if (maxXPos == 0.0f)
            {
                Camera camera = Camera.main;
                float screenSize = Screen.width;
                Vector3 rightScreenPos = new(screenSize, 0, 0);
                Vector3 cameraPos = camera.ScreenToWorldPoint(rightScreenPos);
                maxXPos = cameraPos.x;
            }

            return maxXPos;
        }

        public static float GetMinXPos()
        {
            return -GetMaxXPos();
        }

        public static float GetMaxYPos()
        {
            if (maxYPos == 0.0f)
            {
                Camera camera = Camera.main;
                float screenSize = Screen.height;
                Vector3 topScreenPos = new(0, screenSize, 0);
                Vector3 cameraPos = camera.ScreenToWorldPoint(topScreenPos);
                maxYPos = cameraPos.y;
            }

            return maxYPos;
        }

        public static float GetMinYPos()
        {
            return -GetMaxYPos();
        }

        public static Vector2 GetWorldTouchPos(Vector2 screenTouchPos)
        {
            return Camera.main.ScreenToWorldPoint(screenTouchPos);
        }

        public static float GetFullScreenScale(Sprite sprite)
        {
            if (sprite == null)
            {
                Debug.LogError("Sprite is not found");
                return 0.0f;
            }

            float screenHeight = Camera.main.orthographicSize * 2.0f;
            float screenWidth = screenHeight * Screen.width / Screen.height;

            float spriteWidth = sprite.bounds.size.x;
            float spriteHeight = sprite.bounds.size.y;

            // Calculate the desired scale to fill the screen
            float scaleX = screenWidth / spriteWidth;
            float scaleY = screenHeight / spriteHeight;
            float scale = Mathf.Max(scaleX, scaleY);

            return scale;
        }
    }
}