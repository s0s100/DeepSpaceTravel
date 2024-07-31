using Zenject;
using Core;
using Config;
using UnityEngine;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private ConfigScriptableObject config;
        [SerializeField] private BackgroundService backgroundService;
        [SerializeField] private GenerationService generationService;

        public override void InstallBindings()
        {
            Container.BindInstance(config.Data).AsSingle();
            Container.BindInstance(backgroundService).AsSingle();
            Container.BindInstance(generationService).AsSingle();
        }
    }
}