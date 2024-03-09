using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "Config", menuName = "ScriptableObjects/ConfigScriptableObject", order = 1)]
    public class ConfigScriptableObject : ScriptableObject
    {
        [SerializeField] private ConfigData data;

        public ConfigData Data => data;
    }
}