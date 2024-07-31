using UnityEngine;
using UniRx;
using System;
using Zenject;
using Config;

namespace Core
{
    public class BackgroundService : MonoBehaviour
    {
        [Inject] private readonly ConfigData _config;

        private IDisposable _cloudSubsctiption;

        private void Start()
        {
            GenerateBackground();
            PeriodicallyGenerateClouds();
        }

        private void PeriodicallyGenerateClouds()
        {
            var timer = Observable.Interval(TimeSpan.FromSeconds(_config.BackgroundConfig.foregroundGenerationTime));
            _cloudSubsctiption = timer.Subscribe(_ =>
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
            foregroundObject.transform.localScale *= _config.BackgroundConfig.cloudSizeResize;
            foregroundObject.transform.position = GetRandomTopPosition();

            foregroundRenderer.sprite = RandomForegroundSprite();
            foregroundRenderer.sortingLayerName = "Foreground";

            cloudMovement.MovementSpeed = _config.BackgroundConfig.foregroundMovementSpeed;
            cloudMovement.DeletionTime = _config.BackgroundConfig.foregroundExistanceTime;
        }

        private Sprite RandomForegroundSprite()
        {
            int randomNum = UnityEngine.Random.Range(0, _config.BackgroundConfig.foregroundSprites.Length);
            return _config.BackgroundConfig.foregroundSprites[randomNum];
        }

        private Vector2 GetRandomTopPosition()
        {
            float minXValue = ScreenInfo.GetMinXPos();
            float maxXValue = ScreenInfo.GetMaxXPos();
            float maxYValue = ScreenInfo.GetMaxYPos();

            maxYValue += _config.BackgroundConfig.defaultGenerationShift;
            float randomXPos = UnityEngine.Random.Range(minXValue, maxXValue);
            Vector2 randomPos = new(randomXPos, maxYValue);

            return randomPos;
        }

        private void GenerateBackground()
        {
            var backgroundObject = new GameObject("Background Image");
            var backgroundRenderer = backgroundObject.AddComponent<SpriteRenderer>();

            backgroundObject.transform.parent = transform;

            backgroundRenderer.sprite = _config.BackgroundConfig.backgroundSprite;
            backgroundRenderer.sortingLayerName = "Background";

            float spriteScale = ScreenInfo.GetFullScreenScale(backgroundRenderer.sprite);
            backgroundRenderer.transform.localScale = new Vector3(spriteScale, spriteScale, 1f);
        }

        private void OnDestroy()
        {
            _cloudSubsctiption.Dispose();
        }
    }
}