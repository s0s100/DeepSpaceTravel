using System;
using UniRx;
using UnityEngine;

[CreateAssetMenu(fileName = "Generation pattern", menuName = "ScriptableObjects/Generation pattern", order = 1)]
public class GenerationPattern : ScriptableObject
{
    private const float SCREEN_TOP_SHIFT = 2.0f;

    // Which objects might generate
    [SerializeField]
    private GameObject[] generationObjects;

    [SerializeField]
    private float generationTime;
    // Adds random value to generation time
    [SerializeField]
    private float generationRandomTime;
    // Time before generation start
    [SerializeField]
    private float timeBeforeGeneration;

    // How many objects will spawn at the same time
    [SerializeField]
    private int minGenerationNum;
    [SerializeField]
    private int maxGenerationNum;

    private Transform parentObject;

    public void Initialize(Transform parentObject)
    {
        this.parentObject = parentObject;

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
        int randomValue = UnityEngine.Random.Range(minGenerationNum, maxGenerationNum + 1);
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
        newObject.transform.parent = parentObject;
    }

    private Vector2 DefineGenerationLocation()
    {
        Vector2 location = new();

        float minXPos = ScreenInfo.GetMinXPos();
        float maxXPos = ScreenInfo.GetMaxXPos();
        float randomXPos = UnityEngine.Random.Range(minXPos, maxXPos);

        float yPos = ScreenInfo.GetMaxYPos() + SCREEN_TOP_SHIFT;

        location.x = randomXPos;
        location.y = yPos;

        return location;
    }

}
