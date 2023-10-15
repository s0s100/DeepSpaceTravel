using UnityEngine;
using UniRx;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] private float foregroundGenerationTime = 2.0f;
    [SerializeField] private float foregroundExistanceTime = 10.0f;
    [SerializeField] private float foregroundMovementSpeed = 0.5f;

    [SerializeField] private Sprite backgroundSprite;
    [SerializeField] private Sprite[] foregroundSprites;

    private void Start()
    {
        GenerateBackground();
        PeriodicallyGenerateClouds();
    }

    private void PeriodicallyGenerateClouds()
    {
        var timer = Observable.Interval(System.TimeSpan.FromSeconds(foregroundGenerationTime));
        timer.Subscribe(_ =>
        {
            GenerateCloud();
        }).AddTo(this);
    }

    private void GenerateCloud()
    {
        var foregroundObject = new GameObject("Foreground Cloud");
        var foregroundRenderer = foregroundObject.AddComponent<SpriteRenderer>();
        var cloudMovement = foregroundObject.AddComponent<CloudMovement>();

        foregroundObject.transform.parent = transform;
        foregroundObject.transform.position = Vector2.zero;

        foregroundRenderer.sprite = RandomForegroundSprite();
        foregroundRenderer.sortingLayerName = "Foreground";

        cloudMovement.MovementSpeed = foregroundMovementSpeed;
        cloudMovement.DeletionTime = foregroundExistanceTime;
    }

    private Sprite RandomForegroundSprite()
    {
        int randomNum = Random.Range(0, foregroundSprites.Length);
        return foregroundSprites[randomNum];
    }

    private Vector2 GetRandomTopPosition()
    {
        //float topPosition = ScreenInfo.
        return Vector2.zero;
    }

    private void GenerateBackground()
    {
        var backgroundObject = new GameObject("Background Image");
        var backgroundRenderer = backgroundObject.AddComponent<SpriteRenderer>();
        
        backgroundObject.transform.parent = transform;
        backgroundRenderer.sprite = backgroundSprite;

        float spriteScale = ScreenInfo.GetFullScreenScale(backgroundRenderer.sprite);
        backgroundRenderer.transform.localScale = new Vector3(spriteScale, spriteScale, 1f);
    }
}
