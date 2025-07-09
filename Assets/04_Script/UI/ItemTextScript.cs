using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class ItemTextScript : MonoBehaviour, IPoolable
{
    [SerializeField] private TextMeshProUGUI ItemText;
    private Sequence fadeSequence;
    private Coroutine autoPushCoroutine;

    public string ItemName => "ItemText";
    public GameObject GameObject => gameObject;

    public void SettingText(float count, string name, Color color = default, bool issp = false)
    {
        ItemText.color = color;
        
        if (count == 0)
        {
            ItemText.text = $"{name}";
        }
        else
        {
            ItemText.text = $"{name} + {count}";
        }
            

        float scaledSize = (name == "Gold")
            ? Mathf.Clamp(count / 200f, 0.5f, 2f)
            : Mathf.Clamp(count / 12f, 0.75f, 2f);

        transform.localScale = Vector3.one * scaledSize;

        SetTextAlpha(1f);
        StartFade(scaledSize, issp);

        // 자동 푸시 타이머 시작
        StartAutoPushTimer();
    }

    private void StartFade(float targetScale, bool issp = false)
    {
        if (fadeSequence != null && fadeSequence.IsActive())
        {
            fadeSequence.Kill();
        }

        
        if (issp)
        {
            transform.localScale = Vector3.one * (1.25f);
            fadeSequence = DOTween.Sequence()
            .Append(transform.DOScale(targetScale, 0.2f).SetEase(Ease.OutBack))
            .AppendInterval(2.5f)
            .Append(transform.DOMoveY(transform.position.y + 0.5f, 5f).SetEase(Ease.OutSine))
            .Join(ItemText.DOFade(0f, 5f))
            .AppendCallback(() =>
            {
                PushPush();
            });
        }
        else
        {
            transform.localScale = Vector3.one * (targetScale * 2.5f);
            fadeSequence = DOTween.Sequence()
                .Append(transform.DOScale(targetScale, 0.13f).SetEase(Ease.OutBack))
                .AppendInterval(0.3f)
                .Append(transform.DOMoveY(transform.position.y + 0.5f, 1f).SetEase(Ease.OutSine))
                .Join(ItemText.DOFade(0f, 5f))
                .AppendCallback(() =>
                {
                    PushPush();
                });
        }
            
    }

    private void StartAutoPushTimer()
    {
        if (autoPushCoroutine != null)
            StopCoroutine(autoPushCoroutine);

        autoPushCoroutine = StartCoroutine(AutoPushRoutine());
    }

    private IEnumerator AutoPushRoutine()
    {
        yield return new WaitForSeconds(10f);
        PushPush(); // 10초 후 자동 반환
    }

    private void PushPush()
    {
        if (autoPushCoroutine != null)
        {
            StopCoroutine(autoPushCoroutine);
            autoPushCoroutine = null;
        }

        PoolManager.Instance.Push(this);
    }

    public void ResetItem()
    {
        if (fadeSequence != null && fadeSequence.IsActive())
        {
            fadeSequence.Kill();
        }

        if (autoPushCoroutine != null)
        {
            StopCoroutine(autoPushCoroutine);
            autoPushCoroutine = null;
        }

        SetTextAlpha(1f);
        transform.localScale = Vector3.one;
    }

    private void SetTextAlpha(float alpha)
    {
        ItemText.alpha = alpha;
    }
}
