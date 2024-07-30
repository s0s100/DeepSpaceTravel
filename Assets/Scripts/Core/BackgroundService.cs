using UnityEngine;
using UniRx;
using System;
using Zenject;
using Config;

namespace Core
{
    public class BackgroundService : MonoBehaviour
    {
        [Inject] private readonly ConfigData _configData;
        //private const float CloudSizeResize = 5.0f;
        //private const float DefaultGenerationShift = 2.0f;

        //[SerializeField] private float foregroundGenerationTime = 2.0f;
        //[SerializeField] private float foregroundExistanceTime = 10.0f;
        //[SerializeField] private float foregroundMovementSpeed = 1.0f;

        //[SerializeField] private Sprite backgroundSprite;
        //[SerializeField] private Sprite[] foregroundSprites;

        IDisposable cloudSubsctiption;

        private void Start()
        {
            GenerateBackground();
            PeriodicallyGenerateClouds();
        }

        private void PeriodicallyGenerateClouds()
        {
            var timer = Observable.Interval(TimeSpan.FromSeconds(_configData.BackgroundConfig.foregroundGenerationTime));
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
            foregroundObject.transform.localScale *= _configData.BackgroundConfig.cloudSizeResize;
            foregroundObject.transform.position = GetRandomTopPosition();

            foregroundRenderer.sprite = RandomForegroundSprite();
            foregroundRenderer.sortingLayerName = "Foreground";

            cloudMovement.MovementSpeed = _configData.BackgroundConfig.foregroundMovementSpeed;
            cloudMovement.DeletionTime = _configData.BackgroundConfig.foregroundExistanceTime;
        }

        private Sprite RandomForegroundSprite()
        {
            int randomNum = UnityEngine.Random.Range(0, _configData.BackgroundConfig.foregroundSprites.Length);
            return _configData.BackgroundConfig.foregroundSprites[randomNum];
        }

        private Vector2 GetRandomTopPosition()
        {
            float minXValue = ScreenInfo.GetMinXPos();
            float maxXValue = ScreenInfo.GetMaxXPos();
            float maxYValue = ScreenInfo.GetMaxYPos();

            maxYValue += _configData.BackgroundConfig.defaultGenerationShift;
            float randomXPos = UnityEngine.Random.Range(minXValue, maxXValue);
            Vector2 randomPos = new(randomXPos, maxYValue);

            return randomPos;
        }

        private void GenerateBackground()
        {
            var backgroundObject = new GameObject("Background Image");
            var backgroundRenderer = backgroundObject.AddComponent<SpriteRenderer>();

            backgroundObject.transform.parent = transform;

            backgroundRenderer.sprite = _configData.BackgroundConfig.backgroundSprite;
            backgroundRenderer.sortingLayerName = "Background";

            float spriteScale = ScreenInfo.GetFullScreenScale(backgroundRenderer.sprite);
            backgroundRenderer.transform.localScale = new Vector3(spriteScale, spriteScale, 1f);
        }

        private void OnDestroy()
        {
            cloudSubsctiption.Dispose();
        }
    }
}