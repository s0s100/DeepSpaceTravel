using Config;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private ConfigScriptableObject config;

        public override void InstallBindings()
        {
            Container.BindInstance(config.Data).AsSingle();
        }
    }
}