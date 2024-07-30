using Config;
using UnityEngine;
using Zenject;
using Core;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private ConfigScriptableObject config;

        public override void InstallBindings()
        {
            Container.BindInstance(config.Data).AsSingle();
            Container.Bind<BackgroundService>().AsSingle();
            Container.Bind<GenerationService>().AsSingle();
        }
    }
}