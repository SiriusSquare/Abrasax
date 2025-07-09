using UnityEngine;

public class HeatWave : MonoBehaviour
{
    public float damage = 10f;
    public float duration = 5f;

    [SerializeField] private AttackTriggerScript attackTriggerPrefab;

    [SerializeField] private ParticleSystem heatWaveEffect;
    [SerializeField] private ParticleSystem heatWaveEffect2;
    [SerializeField] private GameObject effect;
    public Entity ownerEntity;
    public void ResetItem()
    {
        damage = 0f;
        duration = 10f;

        if (heatWaveEffect != null) heatWaveEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (heatWaveEffect2 != null) heatWaveEffect2.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }


    private void OnDisable()
    {
        heatWaveEffect.Stop();
        heatWaveEffect2.Stop();
    }
    public void OnSetting(float da, float du, Entity owner, GameObject effect)
    {
        damage = da;
        duration = du;
        ownerEntity = owner;
        
        if (attackTriggerPrefab != null)
        {
            attackTriggerPrefab.ChangeSetting(
                damage,
                1,
                new Vector2(0.25f, 6.25f),
                effect,
                0.07f,
                AttackType.HeatWave,
                AttackElement.Null,
                ownerEntity,
                duration
            );
        }
        heatWaveEffect.Stop();
        heatWaveEffect2.Stop();
        if (heatWaveEffect != null)
        {
            var main = heatWaveEffect.main;
            main.duration = duration;
            
        }
        if (heatWaveEffect2 != null)
        {
            var main2 = heatWaveEffect2.main;
            main2.duration = duration;
            
        }
        heatWaveEffect2.Play();
        heatWaveEffect.Play();
    }
}
