using System;
using UnityEngine;

namespace Code.Core
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static bool HasInstance => _instance != null && !_isShuttingDown;
        private static bool _isShuttingDown = false;
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();
                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject(typeof(T).Name);
                        _instance = singleton.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            T[] managers = FindObjectsByType<T>(FindObjectsSortMode.None);

            if (managers.Length > 1)
                Destroy(gameObject); //���� �̹� ���� �ٸ� �༮�� �ִٸ� ���� �����Ѵ�.
        }

        protected void OnDestroy()
        {
            if (_instance == this)
                _instance = null; //���� �ı��Ǹ� �ν��Ͻ��� null�� �ʱ�ȭ�Ѵ�.
        }
    }
}