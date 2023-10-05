using System.Collections;
using System.Collections.Generic;
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

    public static Vector2 GetWorldTouchPos(Vector2 screenTouchPos)
    {
        return Camera.main.ScreenToWorldPoint(screenTouchPos);
    }

    // For now get a negative value of GetMaxXPos due to symmetric camera location
    public static float GetMinXPos()
    {
        return -GetMaxXPos();
    }
}
