using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

public class GameOverScreen : MonoBehaviour
{
    private string[] difficultyLevels = { "슈퍼 이지", "이지", "노말", "하드", "아포칼립스", "아자토스" };
    [SerializeField] private AudioClip gameOverSound;
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI[] gameOverText;
    [SerializeField] private RawImage rawImage;

    [Header("GameOver Fade Settings")]
    [SerializeField] private Color startColor = Color.white;

    [Header("Tooltips")]
    [SerializeField]
    private string[] tooltips = new string[]
    {
        "\"강줄기에서 종말이 운명의 흐름을 타고 내려온다.\"",
        "\"그 불길한 연기는 위험하다는 뜻이 아니라. 늦었다는 뜻이였다.\"",
        "\"빈 자리.\"",
        "\"너희들이 저것들을 보는 눈이 어떠하든지 간에 나의 눈엔 저것들이 위선으로 보이느라.\"",
        "\"그녀석이 바라볼 때.. 절떄 몸을 움직이지 마....\"",
        "\"집중해.. 이 기회는 다시 한번 오지 않을 테니까\"",
        "\"나는 이것으로 상승해갈거야. 언제 하강할지는 모르겠지만.\"",
        "\"나는. 이렇게 끝을 내고 싶지 않았어...\"",
        "\"그의 피는 이 땅을 물들이고 진홍빛만을 갈망하게 될 것이다.\"",
        "\"사람들은 자신의 몰락조차 알지 못한다.\"",
        "\"몰락한 존경받는 왕은 존경받는 왕인 것인가?\"",
        "\"나의 세계가 밖으로 나왔다.\"",


    };
    private bool isGameOver = false;

    private void Awake()
    {
        rawImage.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Player가 없으면 GameOver 호출
        if (!isGameOver && GameObject.FindGameObjectWithTag("Player") == null)
        {
            isGameOver = true;
            MusicScriptMain.Instance.StopAndReset();
            MusicScriptMain.Instance.PlayMusic(gameOverSound);
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        PlayerPrefs.SetInt("BestScore" + CoreScript.Instance.Difficulty, Mathf.Max(PlayerPrefs.GetInt("BestScore" + CoreScript.Instance.Difficulty, 0), StageManager.Instance.Floor));
        rawImage.gameObject.SetActive(true);
        rawImage.color = new Color(0, 0, 0, 0f);
        gameOverText[0].color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        gameOverText[1].color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        gameOverText[2].color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        gameOverText[3].color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        gameOverText[4].color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        string randomTip = tooltips[Random.Range(0, tooltips.Length)];
        gameOverText[1].text = randomTip;
        gameOverText[3].text = $"최종 도달 층수 : {StageManager.Instance.Floor}층";
        gameOverText[4].text = $"나의 [{difficultyLevels[CoreScript.Instance.Difficulty]}] 난이도 최고 층수 : {PlayerPrefs.GetInt("BestScore" + CoreScript.Instance.Difficulty, 0)}층";
        
        StartCoroutine(FadeInGameOver());
    }

    private IEnumerator FadeInGameOver()
    {
        Color targetColor = new Color(1f, 1f, 1f, 1f);

        DOTween.To(() => rawImage.color, x => rawImage.color = x, new Color(0, 0, 0, 1f), 1f).SetEase(Ease.InOutSine);
        DOTween.To(() => gameOverText[0].color, x => gameOverText[0].color = x, targetColor, 1f).SetEase(Ease.InOutSine);
        DOTween.To(() => gameOverText[1].color, x => gameOverText[1].color = x, targetColor, 1f).SetEase(Ease.InOutSine);
        DOTween.To(() => gameOverText[2].color, x => gameOverText[2].color = x, targetColor, 1f).SetEase(Ease.InOutSine);
        DOTween.To(() => gameOverText[3].color, x => gameOverText[3].color = x, targetColor, 1f).SetEase(Ease.InOutSine);
        DOTween.To(() => gameOverText[4].color, x => gameOverText[4].color = x, targetColor, 1f).SetEase(Ease.InOutSine);
        yield return new WaitForSeconds(1f);
    }
}
