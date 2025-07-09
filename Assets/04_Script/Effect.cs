using UnityEngine;
using System.Collections;
using Code.Core.Pooling;
using System.Security.Cryptography;

public class Effect : MonoBehaviour, IPoolable
{
    [SerializeField] private string _itemName;
    public string ItemName => _itemName;
    public GameObject GameObject => gameObject;

    private ParticleSystem _particleSystem;
    [SerializeField] private float _duration;
    private WaitForSeconds _delaySec;
    private Coroutine _playRoutine;
    
    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        if (_particleSystem == null)
        {
            return;
        }

        _duration = _particleSystem.main.duration;
        _delaySec = new WaitForSeconds(_duration);
    }

    private void OnEnable()
    {
        if (_playRoutine != null)
            StopCoroutine(_playRoutine);

        _playRoutine = StartCoroutine(PlayWithDelay());
    }

    
    private IEnumerator PlayWithDelay()
    {
        yield return null;
        _particleSystem.Play(true);
        StartCoroutine(DelayAndGotoPool());
    }

    public void ResetItem()
    {
        _particleSystem.Stop();
        _particleSystem.Simulate(0);
    }
    private void OnDestroy()
    {
        if (PoolManager.HasInstance)
        {
            PoolManager.Instance.Remove(this);
        }
    }

    private IEnumerator DelayAndGotoPool()
    {
        yield return _delaySec;
        PoolManager.Instance.Push(this);
    }
    public void SetColor(Color color)
    {
        ParticleSystem[] allParticles = GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in allParticles)
        {
            ps.Stop(true);
            var main = ps.main;
            Color original = main.startColor.color;
            color.a = original.a;
            main.startColor = color;
            ps.Play(true);
        }
    }
    
}
