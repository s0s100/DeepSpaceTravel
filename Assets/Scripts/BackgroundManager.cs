using UnityEngine;
using UniRx;
using System;

public class BackgroundManager : MonoBehaviour
{
    private const float CloudSizeResize = 5.0f;
    private const float DefaultGenerationShift = 2.0f;

    [SerializeField] private float foregroundGenerationTime = 2.0f;
    [SerializeField] private float foregroundExistanceTime = 10.0f;
    [SerializeField] private float foregroundMovementSpeed = 1.0f;

    [SerializeField] private Sprite backgroundSprite;
    [SerializeField] private Sprite[] foregroundSprites;

    IDisposable cloudSubsctiption;

    private void Start()
    {
        GenerateBackground();
        PeriodicallyGenerateClouds();
    }

    private void PeriodicallyGenerateClouds()
    {
        var timer = Observable.Interval(TimeSpan.FromSeconds(foregroundGenerationTime));
        cloudSubsctiption = timer.Subscribe(_ =>
        {
            GenerateCloud();
        });
    }

    private void GenerateCloud()
    {
        var foregroundObject = new GameObject("Foreground Cloud");
        var foregroundRenderer = foregroundObject.AddComponent<SpriteRenderer>();
        var cloudMovement = foregroundObject.AddComponent<CloudMovement>();

        foregroundObject.transform.parent = transform;
        foregroundObject.transform.position = Vector2.zero;
        foregroundObject.transform.localScale *= CloudSizeResize;
        foregroundObject.transform.position = GetRandomTopPosition();

        foregroundRenderer.sprite = RandomForegroundSprite();
        foregroundRenderer.sortingLayerName = "Foreground";

        cloudMovement.MovementSpeed = foregroundMovementSpeed;
        cloudMovement.DeletionTime = foregroundExistanceTime;
    }

    private Sprite RandomForegroundSprite()
    {
        int randomNum = UnityEngine.Random.Range(0, foregroundSprites.Length);
        return foregroundSprites[randomNum];
    }

    private Vector2 GetRandomTopPosition()
    {
        float minXValue = ScreenInfo.GetMinXPos();
        float maxXValue = ScreenInfo.GetMaxXPos();
        float maxYValue = ScreenInfo.GetMaxYPos();

        maxYValue += DefaultGenerationShift;
        float randomXPos = UnityEngine.Random.Range(minXValue, maxXValue);
        Vector2 randomPos = new(randomXPos, maxYValue);

        return randomPos;
    }

    private void GenerateBackground()
    {
        var backgroundObject = new GameObject("Background Image");
        var backgroundRenderer = backgroundObject.AddComponent<SpriteRenderer>();
        
        backgroundObject.transform.parent = transform;

        backgroundRenderer.sprite = backgroundSprite;
        backgroundRenderer.sortingLayerName = "Background";

        float spriteScale = ScreenInfo.GetFullScreenScale(backgroundRenderer.sprite);
        backgroundRenderer.transform.localScale = new Vector3(spriteScale, spriteScale, 1f);
    }

    private void OnDestroy()
    {
        cloudSubsctiption.Dispose();
    }
}
