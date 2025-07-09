using Code.Core;
using Code.Core.Pooling;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PoolingList", menuName = "Scriptable Object/Pool/List")]
public class PoolingListSO : ScriptableObject
{
    public List<PoolItem> items;
}
