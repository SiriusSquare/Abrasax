using System;
using UnityEngine;

namespace Code.Core.Pooling
{
    [CreateAssetMenu(fileName = "PoolItem", menuName = "Scriptable Object/Pool/Item", order = 0)]
    public class PoolItem : ScriptableObject
    {
        public string poolName;
        public GameObject prefab;
        public int count;

        private void OnValidate()
        {
            if (prefab == null) return;

            IPoolable item = prefab.GetComponent<IPoolable>();

            if (item == null)
            {
                Debug.LogWarning($"Can not find iPoolable in {prefab.name}");
                prefab = null;
                return;
            }
            poolName = item.ItemName;
        }
    }
}