using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] private Sprite backgroundSprite;

    private void Start()
    {
        GenerateBackground();
    }

    private void GenerateBackground()
    {
        GameObject backgroundObject = new GameObject("Background Image");
        SpriteRenderer backgroundRenderer = backgroundObject.AddComponent<SpriteRenderer>();
        
        
        
        backgroundObject.transform.parent = transform;
        backgroundRenderer.sprite = backgroundSprite;

        float spriteScale = ScreenInfo.GetFullScreenScale(backgroundRenderer.sprite);
        backgroundRenderer.transform.localScale = new Vector3(spriteScale, spriteScale, 1f);
    }
}
