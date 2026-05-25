using UnityEngine;

namespace Survivor.Data
{
    [CreateAssetMenu(fileName = "New Map", menuName = "Survivor/Map")]
    public class MapData : ScriptableObject
    {
        public string nameKey;      // ±¾µØ»¯ Key

        public string mapName;
        public string sceneName;
    }
}