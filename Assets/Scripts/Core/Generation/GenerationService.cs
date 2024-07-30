using Config;
using UnityEngine;
using Zenject;

namespace Core
{
    public class GenerationService : MonoBehaviour
    {
        [Inject] private ConfigData _config;

        private void Start()
        {
            var generationPatterns = _config.GenerationConfig.generationPatterns;
            foreach (var pattern in generationPatterns)
            {
                pattern.Initialize(transform);
            }
        }
    }
}