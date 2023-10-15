using UnityEngine;

// Class to retreive screen information such as width, height and so on
public static class ScreenInfo
{
    public static float GetMaxXPos()
    {
        Camera camera = Camera.main;
        float screenSize = Screen.width;
        Vector3 rightScreenPos = new(screenSize, 0, 0);
        Vector3 cameraPos = camera.ScreenToWorldPoint(rightScreenPos);

        return cameraPos.x;
    }

    // For now get a negative value of GetMaxXPos due to symmetric camera location
    public static float GetMinXPos()
    {
        return -GetMaxXPos();
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
