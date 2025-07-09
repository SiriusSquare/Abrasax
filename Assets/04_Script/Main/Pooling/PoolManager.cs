
using Code.Core;
using Code.Core.Pooling;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;


public class PoolManager : MonoSingleton<PoolManager>
{
    [SerializeField] private PoolingListSO poolList;

    private Dictionary<string, Pool> _pools;
    public static bool HasInstance => Instance != null;
    protected override void Awake()
    {
        base.Awake();
        _pools = new Dictionary<string, Pool>();

        foreach (PoolItem item in poolList.items)
        {
            CreatePool(item.prefab, item.count);
        }
    }

    private void CreatePool(GameObject item, int count)
    {
        IPoolable poolable = item.GetComponent<IPoolable>();
        if (poolable == null)
        {
            Debug.LogError($"Item {item.name} does not implement IPoolable.");
            return;
        }

        Pool pool = new Pool(poolable, transform, count);
        _pools.Add(poolable.ItemName, pool);
    }

    public IPoolable Pop(string itemName)
    {
        if (_pools.ContainsKey(itemName))
        {
            IPoolable item = _pools[itemName].Pop();
            item.ResetItem(); 
            return item;
        }
        Debug.LogError($"Item {itemName} not found in pool.");
        return null;
    }

    public void Push(IPoolable returnItem)
    {
        if (_pools.ContainsKey(returnItem.ItemName))
        {
            _pools[returnItem.ItemName].Push(returnItem);
            return;
        }
        Debug.LogError($"Item {returnItem.ItemName} not found in pool.");

    }

    public void Remove(IPoolable item)
    {
        if (_pools.TryGetValue(item.ItemName, out var pool))
        {
            pool.Remove(item);
        }
    }
}