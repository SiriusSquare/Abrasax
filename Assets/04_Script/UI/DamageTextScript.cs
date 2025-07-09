using UnityEngine;
using TMPro;
using System.Collections;
using DG.Tweening;
using Code.Core.Pooling;
using System;
using NUnit.Framework;
using System.Collections.Generic;
public enum HitType
{
    Normal = 0,
    Critical = 1,
    SuperCritical = 2,
    ShieldDamage = 3,
    Guard = 4,
    Heal = 5,
    AddShield = 6,


}
public class DamageTextScript : MonoBehaviour, IPoolable
{
    [SerializeField] private TextMeshProUGUI DamageText;
    [SerializeField] private TextMeshProUGUI CriticalText;
    [SerializeField] private TextMeshProUGUI ElementText;
    [SerializeField] protected Color[] htColors = new Color[6];
    private Sequence fadeSequence;

    public string ItemName => "DamageText";
    public GameObject GameObject => gameObject;

    public void SettingText(float damage, AttackElement ele, AttackType type, HitType ht = 0, Color color = default)
    {

        DamageText.color = htColors[(int)ht];
        DamageText.text = $"{damage}";
        float scaledSize = Mathf.Clamp(damage / 175f, 0.75f, 2.5f);
        transform.localScale = Vector3.one * scaledSize;

        if (color != default)
        {
            DamageText.color = color;
            CriticalText.color = color;
        }
        else if ((int)ht == 1) // Critical Hit
        {
            CriticalText.text = "Critical";
            CriticalText.color = htColors[(int)ht];
            CriticalText.alpha = 1f;
        }
        else if ((int)ht == 2) // Super Critical Hit
        {
            CriticalText.text = "Super Critical";
            CriticalText.color = htColors[(int)ht];
            CriticalText.alpha = 1f;
        }
        else if ((int)ht == 3)
        {
            CriticalText.text = "Shield";
            CriticalText.color = htColors[(int)ht];
            CriticalText.alpha = 1f;
        }
        else if ((int)ht == 4)
        {
            DamageText.text = "Guard";
            scaledSize = 1f;
            transform.localScale = Vector3.one * scaledSize;
        }
        else if ((int)ht == 5)
        {
            CriticalText.text = "Healing";
            CriticalText.color = htColors[(int)ht];
            CriticalText.alpha = 1f;
        }
        else if ((int)ht == 6)
        {
            CriticalText.text = "AddShield";
            CriticalText.color = htColors[(int)ht];
            CriticalText.alpha = 1f;
        }
        else
        {
            CriticalText.text = string.Empty; // 일반 공격 시 크리티컬 텍스트 비우기
            CriticalText.alpha = 0f; // 크리티컬 텍스트 숨기기
        }
            //ElementText.text = $"{ele}/{type}";


            SetTextAlpha(1f);

        StartFade(scaledSize);
    }

    private void StartFade(float targetScale)
    {
        if (fadeSequence != null && fadeSequence.IsActive())
        {
            fadeSequence.Kill();
        }

        // 초기 스케일 팝 효과
        transform.localScale = Vector3.one * (targetScale * 2.5f); // 일시적으로 더 크게

        fadeSequence = DOTween.Sequence()
            .Append(transform.DOScale(targetScale, 0.13f).SetEase(Ease.OutBack)) // 팝 효과 후 원래 스케일로 축소
            .AppendInterval(0.3f) // 텍스트 유지 시간
            .Append(transform.DOMoveY(transform.position.y + 0.5f, 1f).SetEase(Ease.OutSine)) // 부드럽게 올라가기
            .Join(DamageText.DOFade(0f, 1f))
            .Join(CriticalText.DOFade(0f, 1f))
            //.Join(ElementText.DOFade(0f, 1.4f))
            .AppendCallback(() =>
            {
                PushPush();
            });
    }

    private void PushPush()
    {
        PoolManager.Instance.Push(this);
    }

    public void ResetItem()
    {
        if (fadeSequence != null && fadeSequence.IsActive())
        {
            fadeSequence.Kill();
        }

        SetTextAlpha(1f);
        transform.localScale = Vector3.one;
        

        
    }

    private void SetTextAlpha(float alpha)
    {
        DamageText.alpha = alpha;
        CriticalText.alpha = alpha;
        //ElementText.alpha = alpha;
    }
}
