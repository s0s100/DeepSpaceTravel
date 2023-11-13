using System;
using UniRx;
using UnityEngine;

[CreateAssetMenu(fileName = "Generation pattern", menuName = "ScriptableObjects/Generation pattern", order = 1)]
public class GenerationPattern : ScriptableObject
{
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
    private float minGenerationNum;
    [SerializeField]
    private float maxGenerationNum;

    public void Initialize()
    {
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
        Debug.Log(Time.time);
    }
}
