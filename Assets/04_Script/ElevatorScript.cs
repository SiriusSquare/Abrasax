using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ZoneType
{
    NormalZone = 0,
    HardZone = 1,
    SpecialZone = 2,
    MysteryZone = 3,
    DivineZone = 4,
    EldrichZone = 5,
    UtilityZone = 6,
    UltraRareZone = 7,
    SecretZone = 8,
    BossZone = 9,
}

[System.Serializable]
public class ZoneRoomData
{
    public string[] RoomNames;
    public float[] RoomChances;

    public ZoneRoomData(string[] names, float[] chances)
    {
        RoomNames = names;
        RoomChances = chances;
    }
}

public class ElevatorScript : MonoBehaviour
{
    private string[] zoneNames = new string[]
    {
        "노멀 존",
        "하드 존",
        "스패셜 존",
        "미스터리 존",
        "디바인 존",
        "엘드리치 존",
        "유틸리티 존",
        "울트라 레어 존"
    };

    public int stageIndex = 0;
    private float randomValue = 0.0f;
    private Animator animator;
    private string decurationName = "";
    private StageManager stageManager;
    private GameObject loadstage;
    [SerializeField] private GameObject text;
    private CanvasGroup textCanvasGroup;
    private Vector3 textOriginalPos;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("애니메이터가 읎다");
            }
        }
        if (stageManager == null)
        {
            stageManager = FindAnyObjectByType<StageManager>();
            if (stageManager == null)
            {
                Debug.LogError("스테이지 메네이스가 없다");
            }
        }

        if (text != null)
        {
            textCanvasGroup = text.GetComponent<CanvasGroup>();
            if (textCanvasGroup == null)
            {
                Debug.LogError("text 오브젝트에 CanvasGroup 컴포넌트가 읎다!");
            }
            textOriginalPos = text.transform.localPosition;

            textCanvasGroup.alpha = 0f;
            text.transform.localPosition = textOriginalPos;
            text.SetActive(false);
        }
        else
        {
            Debug.LogWarning("text 오브젝트가 할당되지 아났다!");
        }
        StageRoll(SettingZoneRandom(), SettingZoneRandom(), SettingZoneRandom());

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

    private readonly Dictionary<ZoneType, ZoneRoomData> zoneRoomDataMap = new Dictionary<ZoneType, ZoneRoomData>
    {
        {
            ZoneType.NormalZone,
            new ZoneRoomData(
                new string[] { "쉬운 방", "보통 방", "어려운 방", "매우 어려운 방" },
                new float[] { 0.65f, 0.90f, 0.95f, 1f })
        },
        {
            ZoneType.HardZone,
            new ZoneRoomData(
                new string[] { "미칠듯이 어려운 방", "극도로 어려운 방", "공포에 질릴 정도로 어려운 방", "파국적인 종말을 불러올만큼 어려운 방" },
                new float[] { 0.55f, 0.8f, 0.95f, 1f })
        },
        {
            ZoneType.SpecialZone,
            new ZoneRoomData(
                new string[] { "금고 방","희귀 특수 방" },
                new float[] { 0.9f, 1f })
        },
        {
            ZoneType.DivineZone,
            new ZoneRoomData(
                new string[] { "축복 방"},
                new float[] { 1f })
        },
        {
            ZoneType.EldrichZone,
            new ZoneRoomData(
                new string[] { "저주 방" },
                new float[] { 1f })
        },
        {
            ZoneType.UtilityZone,
            new ZoneRoomData(
                new string[] { "상점" },
                new float[] { 1f })
        },
        {
            ZoneType.UltraRareZone,
            new ZoneRoomData(
                new string[] { "일반 울트라레어 방", "초희귀 방" },
                new float[] { 0.9f, 1f })
        },
    };

    public ZoneType SettingZoneRandom()
    {
        randomValue = UnityEngine.Random.Range(0.0f, 1.0f);

        // 기본 확률
        if (randomValue < 0.8f)
            stageIndex = (int)ZoneType.NormalZone;
        else if (randomValue < 0.875f)
            stageIndex = (int)ZoneType.SpecialZone;
        else if (randomValue < 0.925f)
            stageIndex = (int)ZoneType.DivineZone;
        else if (randomValue < 0.975f)
            stageIndex = (int)ZoneType.EldrichZone;
        else if (randomValue < 0.99f)
            stageIndex = (int)ZoneType.UtilityZone;
        else
            stageIndex = (int)ZoneType.UltraRareZone;

        if (StageManager.Instance.Floor < 20 && (ZoneType)stageIndex == ZoneType.EldrichZone)
        {
            stageIndex = (int)ZoneType.NormalZone;

        }

        return (ZoneType)stageIndex;
    }

    string GetRandomRoomName(ZoneType zone)
    {
        if (!zoneRoomDataMap.TryGetValue(zone, out ZoneRoomData data))
            return "알 수 없는 방";

        float rand = Random.Range(0f, 1f);
        for (int i = 0; i < data.RoomChances.Length; i++)
        {
            if (rand <= data.RoomChances[i])
                if (data.RoomNames[i] == "매우 어려운 방" && StageManager.Instance.Floor < 20)
                {
                    return data.RoomNames[0];
                }
            return data.RoomNames[i];
        }
        return "알 수 없는 방";
    }
    public void StageRoll(ZoneType zone1, ZoneType zone2, ZoneType zone3)
    {
        int change1 = Random.Range(0, 100);
        int change2 = Random.Range(change1, 100);
        int ceange = Random.Range(0, 100);

        if (ceange < change1) stageIndex = (int)zone1;
        else if (ceange < change2) stageIndex = (int)zone2;
        else stageIndex = (int)zone3;
        string actualRoomName = GetRandomRoomName((ZoneType)stageIndex);

        string otherRoomName1 = GetRandomRoomName(zone1);
        string otherRoomName2 = GetRandomRoomName(zone2);
        string otherRoomName3 = GetRandomRoomName(zone3);

        List<string> roomNames = new List<string> {
        actualRoomName,
        otherRoomName1,
        otherRoomName2,
        otherRoomName3
    };

        for (int i = 0; i < roomNames.Count; i++)
        {
            int randIdx = Random.Range(0, roomNames.Count);
            var temp = roomNames[i];
            roomNames[i] = roomNames[randIdx];
            roomNames[randIdx] = temp;
        }
        loadstage = GetRandomRoomPrefab((ZoneType)stageIndex);
        int change4 = Random.Range(0, 100);
        if (change4 < 5) decurationName = $"이 엘레베이터는 [{roomNames[0]}]으로 이동하는 것 같습니다.";
        else if (change4 < 20) decurationName = $"{change1}% 확률로 [{roomNames[0]}], {change2 - change1}% 확률로 [{roomNames[1]}], {100 - change2}% 확률로 [{roomNames[2]}]로 이동하는 것 같습니다.";
        else if (change4 < 25) decurationName = $"{change1}% 확률로 [{roomNames[0]}], {100 - change1}% 확률로 다른 방으로 이동하는 것 같습니다.";
        else if (change4 < 32) decurationName = $"{change2 - change1}% 확률로 [{roomNames[1]}], {100 - (change2 - change1)}% 확률로 다른 방으로 이동하는 것 같습니다.";
        else if (change4 < 40) decurationName = $"{100 - change2}% 확률로 [{roomNames[2]}], 다른 가능성도 있습니다.";
        else if (change4 < 55) decurationName = $"[{roomNames[0]}], [{roomNames[1]}], [{roomNames[2]}], [{roomNames[3]}] 중 하나로 이동하는 것 같습니다.";
        else if (change4 < 80) decurationName = $"{string.Join(", ", roomNames)} 어디로든 갈 수 있습니다.";
        else if (change4 < 90) decurationName = $"이 엘레베이터는 [{string.Join("], [", roomNames)}] 외 다른 곳으로 가지 않습니다.";
        else if (change4 < 95) decurationName = $"{string.Join(", ", roomNames)} 어디로든 갈 수도 있고 아닐 수도 있습니다.";
        else decurationName = "이 엘레베이터가 어디로 가는 것인지 알 수 없습니다.";
    }

    public GameObject GetRandomRoomPrefab(ZoneType zone)
    {
        if (!zoneRoomDataMap.TryGetValue(zone, out ZoneRoomData data))
        {
            Debug.LogWarning($"존 데이터가 읎어요: {zone}");
            return null;
        }

        float rand;
        if (zone == ZoneType.NormalZone)
        {
            rand = UnityEngine.Random.Range(0f + (StageManager.Instance.Floor / 100), 1f);
        }
        else
        {
            rand = UnityEngine.Random.Range(0f, 1f);
        }
            string selectedRoomName = null;

        for (int i = 0; i < data.RoomChances.Length; i++)
        {
            if (rand <= data.RoomChances[i])
            {
                selectedRoomName = data.RoomNames[i];
                break;
            }
        }

        if (selectedRoomName == null)
        {
            return null;
        }

        string zoneFolderName = zone.ToString();
        string roomFolderName = ConvertRoomNameToEnglish(selectedRoomName);

        string path = $"Rooms/{zoneFolderName}/{roomFolderName}";
        GameObject[] roomPrefabs = Resources.LoadAll<GameObject>(path);
        GameObject selectedRoom;

        if (roomPrefabs == null || roomPrefabs.Length == 0)
        {
            Debug.LogError($"해당 경로에 룸 프리팹 없음 {path}");
            return null;
        }
        else
        {
            selectedRoom = roomPrefabs[Random.Range(0, roomPrefabs.Length)];
        }

        return selectedRoom;
    }


    private string ConvertRoomNameToEnglish(string koreanName)
    {
        switch (koreanName)
        {
            case "쉬운 방": return "EasyRoom";
            case "보통 방": return "NormalRoom";
            case "어려운 방": return "HardRoom";
            case "매우 어려운 방": return "VeryHardRoom";

            case "미칠듯이 어려운 방": return "InsaneRoom";
            case "극도로 어려운 방": return "ExtremeRoom";
            case "공포에 질릴 정도로 어려운 방": return "HorrorRoom";
            case "파국적인 종말을 불러올만큼 어려운 방": return "ApocalypseRoom";

            case "금고 방": return "TreasureRoom";

            case "희귀 특수 방": return "RareSpecialRoom";


            case "축복 방": return "AngelRoom";


            case "저주 방": return "DemonRoom";


            case "상점": return "Shop";

            case "일반 울트라레어 방": return "StandardUltraRareRoom";
            case "초희귀 방": return "SuperRareRoom";

            default:
                Debug.LogWarning($"영문 룸 이름으로 변환할 수 없습니다: {koreanName}");
                return "NormalRoom";
        }
    }

    public string GetEnglishZoneName(ZoneType zone)
    {
        switch (zone)
        {
            case ZoneType.NormalZone: return "NormalZone";
            case ZoneType.HardZone: return "HardZone";
            case ZoneType.SpecialZone: return "SpecialZone";
            case ZoneType.MysteryZone: return "MysteryZone";
            case ZoneType.DivineZone: return "DivineZone";
            case ZoneType.EldrichZone: return "EldrichZone";
            case ZoneType.UtilityZone: return "UtilityZone";
            case ZoneType.UltraRareZone: return "UltraRareZone";
            case ZoneType.SecretZone: return "SecretZone";
            case ZoneType.BossZone: return "BossZone";
            default: return "UnknownZone";
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTextVisible)
        {
            ShowElevatorText(decurationName);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isTextVisible)
        {
            HideElevatorText();
        }
    }
    private void Update()
    {
        if (loadstage == null)
        {
            StageRoll(SettingZoneRandom(), SettingZoneRandom(), SettingZoneRandom());
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.CompareTag("Player"))
        {

            if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                if (stageManager != null)
                {
                    
                    if (loadstage != null)
                    {
                        stageManager.SetStage(loadstage);
                    }

                }
            }
        }
    }
}
