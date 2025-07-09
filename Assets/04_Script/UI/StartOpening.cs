using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class StartOpening : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Text;
    private string[] openingtext;
    [SerializeField] private List<GameObject> openingObjects = new List<GameObject>();
    private List<Vector3> originalPositions = new List<Vector3>();
    private AudioSource audioSource;
    [SerializeField] private AudioClip clip;
    [SerializeField] private RawImage rawImage;

    void Start()
    {
        openingtext = "��/��/ /��/��/ /��/��/��/��/ /��/��/��".Split("/");

        // ��ġ ���� �� ��¦ �ű��
        foreach (var obj in openingObjects)
        {
            originalPositions.Add(obj.transform.position);
            obj.transform.position += Vector3.left * 10f;
        }

        StartCoroutine(OpeningStart());
    }

    private IEnumerator OpeningStart()
    {
        Text.text = "";
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < openingtext.Length; i++)
        {
            Text.text += openingtext[i];
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            if (openingtext[i] != " " && openingtext[i] != ".")
            {
                audioSource.clip = clip;
                audioSource.Play();
                yield return new WaitForSeconds(0.2f);
            }
        }
        yield return new WaitForSeconds(1.5f);
        // �ؽ�Ʈ �� ��� ���̵�ƿ�
        Color startColor = rawImage.color;
        Color targetColor = new Color(0, 0, 0, 0f);
        DOTween.To(() => startColor, x =>
        {
            startColor = x;
            rawImage.color = x;
            Text.color = x;
        }, targetColor, 1.5f).SetEase(Ease.InOutSine);
        Color textStartColor = Text.color;
        DOTween.To(() => textStartColor, x =>
        {
            textStartColor = x;
            Text.color = x;
        }, targetColor, 1.5f).SetEase(Ease.InOutSine);
        yield return new WaitForSeconds(1f);
        // ������Ʈ���� ���� ��ġ�� �̵�
        for (int i = 0; i < openingObjects.Count; i++)
        {
            openingObjects[i].transform.DOMove(originalPositions[i], 1f)
                .SetEase(Ease.InOutSine);
            yield return new WaitForSeconds(0.25f);
        }
        
        yield return null;
    }
}
