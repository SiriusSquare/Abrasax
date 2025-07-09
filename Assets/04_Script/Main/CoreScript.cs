using UnityEngine;

public class CoreScript : MonoBehaviour
{
    public static CoreScript Instance { get; private set; }

    public int Difficulty { get; set; } = 2;
    public float[] Diffint { get; set; } = new float[6] { 0.4f, 0.75f, 1f, 1.5f, 2f, 3f };
    public int[] bestScore { get; set; } = new int[6] { 0, 0, 0, 0, 0, 0 };
    public float BGMSetting { get; set; } = 0.5f;
    public float SFXSetting { get; set; } = 0.5f;
    public bool CalamityMode { get; set; } = false;

    private void Awake()
    {
        BGMSetting = PlayerPrefs.GetFloat("BGMSetting", 0.5f);
        SFXSetting = PlayerPrefs.GetFloat("SFXSetting", 0.5f);
        CalamityMode = (PlayerPrefs.GetInt("CalamityMode", 0) == 1 ? true : false);
        Difficulty = PlayerPrefs.GetInt("Difficulty", 2);
        for (int i = 0; i < bestScore.Length; i++)
        {
            if (PlayerPrefs.GetInt("BestScore" + i, 0) > 100)
            {
                PlayerPrefs.SetInt("BestScore" + i, 100);
                PlayerPrefs.Save();
            }
            bestScore[i] = Mathf.Max(0, PlayerPrefs.GetInt("BestScore" + i, 0));
        }

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Instance != null)
        {
            PlayerPrefs.SetFloat("BGMSetting", BGMSetting);
            PlayerPrefs.SetFloat("SFXSetting", SFXSetting);
            PlayerPrefs.SetInt("CalamityMode", CalamityMode ? 1 : 0);
            PlayerPrefs.SetInt("Difficulty", Difficulty);
            PlayerPrefs.SetInt("BestScore" + Difficulty, Mathf.Max(PlayerPrefs.GetInt("BestScore" + Difficulty, 0), StageManager.Instance.Floor));
            PlayerPrefs.Save();
        }
    }
#endif

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("BGMSetting", BGMSetting);
        PlayerPrefs.SetFloat("SFXSetting", SFXSetting);
        PlayerPrefs.SetInt("CalamityMode", CalamityMode ? 1 : 0);
        PlayerPrefs.SetInt("Difficulty", Difficulty);
        PlayerPrefs.SetInt("BestScore" + Difficulty, Mathf.Max(PlayerPrefs.GetInt("BestScore" + Difficulty,0),StageManager.Instance.Floor));
        PlayerPrefs.Save();
    }
}
