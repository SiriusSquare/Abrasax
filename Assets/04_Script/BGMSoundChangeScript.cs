using UnityEngine;

public class BGMChangeScriptTest : MonoBehaviour
{
    [SerializeField] private AudioClip bgmClip;
    private void Awake()
    {
        if (bgmClip == null)
        {
            bgmClip = GetComponent<AudioClip>();
            return;
        }
    }
    private void Start()
    {
        if (bgmClip != null)
        {
            MusicScriptMain.Instance.PlayMusic(bgmClip);
        }
        else
        {
        }
    }
}
