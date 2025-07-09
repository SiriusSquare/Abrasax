using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinalerScript : MonoBehaviour
{



    private Animator animator;
    private string decurationName = "End of The Line";
    private StageManager stageManager;
    private GameObject loadstage;
    [SerializeField] private GameObject text;
    private CanvasGroup textCanvasGroup;
    private Vector3 textOriginalPos;
    [SerializeField] private GameObject final;
    private bool activated = false;
    private void Awake()
    {

        // 텍스트 CanvasGroup 컴포넌트 가져오기
        if (text != null)
        {
            textCanvasGroup = text.GetComponent<CanvasGroup>();
            if (textCanvasGroup == null)
            {
                Debug.LogError("text 오브젝트에 CanvasGroup 컴포넌트가 필요합니다.");
            }
            textOriginalPos = text.transform.localPosition;

            // 초기 숨김 세팅
            textCanvasGroup.alpha = 0f;
            text.transform.localPosition = textOriginalPos;
            text.SetActive(false);
        }
        ShowElevatorText(decurationName);


    }

    private bool isTextVisible = false;
    private Sequence textSequence;
    private void ShowElevatorText(string message)
    {
        if (text == null || textCanvasGroup == null) return;
        if (isTextVisible) return;

        isTextVisible = true;

        
        textSequence?.Kill(true);

        text.SetActive(true);

        var textComponentTMP = text.GetComponent<TMPro.TextMeshProUGUI>();
        if (textComponentTMP != null)
            textComponentTMP.text = message;
        else
        {
            var textComponent = text.GetComponent<UnityEngine.UI.Text>();
            if (textComponent != null)
                textComponent.text = message;
        }

        text.transform.localPosition = textOriginalPos + new Vector3(0, -5f, 0);

        textCanvasGroup.alpha = 0f;

        textSequence = DOTween.Sequence();
        textSequence.Append(text.transform.DOLocalMoveY(textOriginalPos.y, 0.25f).SetEase(Ease.OutCubic));
        textSequence.Join(textCanvasGroup.DOFade(1f, 0.25f));
        textSequence.Play();
    }

    private void HideElevatorText()
    {
        if (text == null || textCanvasGroup == null) return;
        if (!isTextVisible) return;

        isTextVisible = false;

        textSequence?.Kill(true);

        textSequence = DOTween.Sequence();
        textSequence.Append(text.transform.DOLocalMoveY(textOriginalPos.y - 2f, 0.25f).SetEase(Ease.InCubic));
        textSequence.Join(textCanvasGroup.DOFade(0f, 0.25f));
        textSequence.OnComplete(() => text.SetActive(false));
        textSequence.Play();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && activated == false)
        {

            final.SetActive(true);
            StartCoroutine(Final());
        }
    }

    private IEnumerator Final()
    {
        final.SetActive(true);
        activated = true;
        yield return new WaitForSeconds(2f);
        
        SceneManager.LoadScene("Title");
        yield return null;
    }
}
