using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

public class GameOverScreen : MonoBehaviour
{
    private string[] difficultyLevels = { "���� ����", "����", "�븻", "�ϵ�", "����Į����", "�����佺" };
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
        "\"���ٱ⿡�� ������ ����� �帧�� Ÿ�� �����´�.\"",
        "\"�� �ұ��� ����� �����ϴٴ� ���� �ƴ϶�. �ʾ��ٴ� ���̿���.\"",
        "\"�� �ڸ�.\"",
        "\"������� ���͵��� ���� ���� ��ϵ��� ���� ���� ���� ���͵��� �������� ���̴���.\"",
        "\"�׳༮�� �ٶ� ��.. ���� ���� �������� ��....\"",
        "\"������.. �� ��ȸ�� �ٽ� �ѹ� ���� ���� �״ϱ�\"",
        "\"���� �̰����� ����ذ��ž�. ���� �ϰ������� �𸣰�����.\"",
        "\"����. �̷��� ���� ���� ���� �ʾҾ�...\"",
        "\"���� �Ǵ� �� ���� �����̰� ��ȫ������ �����ϰ� �� ���̴�.\"",
        "\"������� �ڽ��� �������� ���� ���Ѵ�.\"",
        "\"������ ����޴� ���� ����޴� ���� ���ΰ�?\"",
        "\"���� ���谡 ������ ���Դ�.\"",


    };
    private bool isGameOver = false;

    private void Awake()
    {
        rawImage.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Player�� ������ GameOver ȣ��
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
        gameOverText[3].text = $"���� ���� ���� : {StageManager.Instance.Floor}��";
        gameOverText[4].text = $"���� [{difficultyLevels[CoreScript.Instance.Difficulty]}] ���̵� �ְ� ���� : {PlayerPrefs.GetInt("BestScore" + CoreScript.Instance.Difficulty, 0)}��";
        
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
