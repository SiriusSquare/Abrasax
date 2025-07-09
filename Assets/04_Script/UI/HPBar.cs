using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Unity.VisualScripting;

public class HPBar : MonoBehaviour
{
    [SerializeField] private Image hpBarImage;
    [SerializeField] private Image subHpBarImage;
    [SerializeField] private TextMeshProUGUI hpText;

    [SerializeField] private Image shieldBarImage;
    [SerializeField] private Image subShieldBarImage;

    [SerializeField] private Image staminaBarImage;
    [SerializeField] private Image subStaminaBarImage;

    [SerializeField] private Image manaImage;
    [SerializeField] private Image subManaImage;

    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI CoinText;
    [SerializeField] private TextMeshProUGUI APText;
    [SerializeField] private Entity entity;

    private Tween hpTween;
    private Tween shieldTween;
    private Tween staminaTween;
    private Tween manaTween;

    private float maxHPWidth;
    private float maxShieldWidth;
    private float maxStaminaWidth;
    private float maxManaWidth;
    public void StartSetting()
    {
        if (entity == null) return;

        maxHPWidth = hpBarImage.rectTransform.sizeDelta.x;
        maxShieldWidth = shieldBarImage.rectTransform.sizeDelta.x;
        maxStaminaWidth = staminaBarImage.rectTransform.sizeDelta.x;

        SetHP();
        SetStamina();
        if (manaImage != null && subManaImage != null)
        {
            maxManaWidth = manaImage.rectTransform.sizeDelta.x;
            SetMana();
        }
    }
    private void Update()
    {
        if (manaText != null)
            manaText.text = $"{entity.MaxMana / entity.Mana * 100}%";

        if (CoinText != null)
            CoinText.text = $"Gold : {entity.statObject.Gold}";
        if (APText != null)
            APText.text = $"Arrow Point : {entity.statObject.ArrowPoint}";

    }
    public void SetHP()
    {
        if (entity == null) return;
        entity.MaxHealth = entity.statObject.MaxHealth;
        entity.MaxStamina = entity.statObject.MaxStamina;
        entity.KnockbackResistance = entity.statObject.KnockbackResistance;
        entity.MaxMana = entity.statObject.MaxMana;

        float hpRatio = entity.Health / entity.MaxHealth;
        float shieldRatio = entity.Shield / entity.MaxShield;

        hpTween?.Kill();
        shieldTween?.Kill();

        // 즉시 메인 바 업데이트
        hpBarImage.rectTransform.sizeDelta = new Vector2(maxHPWidth * hpRatio, hpBarImage.rectTransform.sizeDelta.y);
        shieldBarImage.rectTransform.sizeDelta = new Vector2(maxShieldWidth * shieldRatio, shieldBarImage.rectTransform.sizeDelta.y);

        // 서브 바는 애니메이션
        hpTween = DOTween.To(
            () => subHpBarImage.rectTransform.sizeDelta.x,
            x => subHpBarImage.rectTransform.sizeDelta = new Vector2(x, subHpBarImage.rectTransform.sizeDelta.y),
            maxHPWidth * hpRatio,
            0.25f
        ).SetEase(Ease.InOutSine);

        shieldTween = DOTween.To(
            () => subShieldBarImage.rectTransform.sizeDelta.x,
            x => subShieldBarImage.rectTransform.sizeDelta = new Vector2(x, subShieldBarImage.rectTransform.sizeDelta.y),
            maxShieldWidth * shieldRatio,
            0.25f
        ).SetEase(Ease.InOutSine);

        if (hpText != null)
        {
            if (shieldRatio > 0)
                hpText.text = $"{entity.Health} / {entity.MaxHealth} + {entity.Shield}";
            else
                hpText.text = $"{entity.Health} / {entity.MaxHealth}";
        }
    }

    public void SetStamina()
    {
        if (entity == null) return;

        float staminaRatio = entity.Stamina / entity.MaxStamina;

        staminaTween?.Kill();

        staminaBarImage.rectTransform.sizeDelta = new Vector2(maxStaminaWidth * staminaRatio, staminaBarImage.rectTransform.sizeDelta.y);

        staminaTween = DOTween.To(
            () => subStaminaBarImage.rectTransform.sizeDelta.x,
            x => subStaminaBarImage.rectTransform.sizeDelta = new Vector2(x, subStaminaBarImage.rectTransform.sizeDelta.y),
            maxStaminaWidth * staminaRatio,
            0.1f
        ).SetEase(Ease.InOutSine);
    }

    public void SetMana()
    {
        if (entity == null || manaImage == null || subManaImage == null) return;
        entity.MaxHealth = entity.statObject.MaxHealth;
        entity.MaxStamina = entity.statObject.MaxStamina;
        entity.KnockbackResistance = entity.statObject.KnockbackResistance;
        entity.MaxMana = entity.statObject.MaxMana;
        float manaRatio = entity.Mana / entity.MaxMana;

        manaTween?.Kill();

       manaImage.rectTransform.sizeDelta = new Vector2(maxManaWidth * manaRatio, manaImage.rectTransform.sizeDelta.y);

        manaTween = DOTween.To(
            () => subManaImage.rectTransform.sizeDelta.x,
            x => subManaImage.rectTransform.sizeDelta = new Vector2(x, subManaImage.rectTransform.sizeDelta.y),
            maxManaWidth * manaRatio,
            0.25f
        ).SetEase(Ease.InOutSine);
    }
}
