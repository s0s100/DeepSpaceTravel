using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{
    [SerializeField]
    private GenerationPattern[] generationPatterns;

    private void Start()
    {
        foreach (var pattern in generationPatterns)
        {
            pattern.Initialize();
        }
    }
}
