using UnityEngine;

namespace Core
{
    public class GenerationService : MonoBehaviour
    {
        [SerializeField]
        private GenerationPattern[] generationPatterns;

        private void Start()
        {
            foreach (var pattern in generationPatterns)
            {
                pattern.Initialize(transform);
            }
        }
    }
}