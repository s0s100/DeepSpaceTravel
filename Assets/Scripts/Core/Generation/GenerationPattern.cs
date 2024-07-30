using Config;
using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace Core
{
    [CreateAssetMenu(fileName = "Generation pattern", menuName = "ScriptableObjects/Generation pattern", order = 1)]
    public class GenerationPattern : ScriptableObject
    {
        [Inject] private readonly ConfigData _config;

        [SerializeField] private GameObject[] generationObjects;
        [SerializeField] private float generationTime;
        [SerializeField] private float generationRandomTime;
        [SerializeField] private float timeBeforeGeneration;
        // How many objects will spawn at the same time
        [SerializeField] private int minGenerationNum;
        [SerializeField] private int maxGenerationNum;

        private Transform _parentObject;

        public void Initialize(Transform parentObject)
        {
            _parentObject = parentObject;

            Observable.Timer(TimeSpan.FromSeconds(timeBeforeGeneration)).Subscribe(_ =>
                {
                    GenerationIteration();
                });
        }

        private void GenerationIteration()
        {
            GenerateObjects();

            var randomDelay = UnityEngine.Random.Range(0, generationRandomTime);
            var timeToGenerate = generationTime + randomDelay;

            Observable.Timer(TimeSpan.FromSeconds(timeToGenerate)).Subscribe(_ =>
                {
                    GenerationIteration();
                });
        }

        private void GenerateObjects()
        {
            //var randomValue = UnityEngine.Random.Range(minGenerationNum, maxGenerationNum + 1);
            for (int i = 0; i < generationObjects.Length; i++)
            {
                GenerateObject();
            }
        }

        private void GenerateObject()
        {
            int maxNumOfObject = generationObjects.Length;
            if (maxNumOfObject == 0)
            {
                Debug.LogWarning("No objects to create");
                return;
            }

            int randomValue = UnityEngine.Random.Range(0, maxNumOfObject);
            Vector2 location = DefineGenerationLocation();
            GameObject newObject = Instantiate(generationObjects[randomValue], location, Quaternion.identity);
            newObject.transform.parent = _parentObject;
        }

        private Vector2 DefineGenerationLocation()
        {
            Vector2 location = new();

            float minXPos = ScreenInfo.GetMinXPos();
            float maxXPos = ScreenInfo.GetMaxXPos();
            float randomXPos = UnityEngine.Random.Range(minXPos, maxXPos);

            float yPos = ScreenInfo.GetMaxYPos() + _config.GenerationConfig.screenSpawnShift;

            location.x = randomXPos;
            location.y = yPos;

            return location;
        }
    }
}