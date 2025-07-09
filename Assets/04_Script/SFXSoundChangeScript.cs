using UnityEngine;

public class SFXSoundChangeScript : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private float updateTimer = 0f;
    private float updateInterval = 0.1f;
    private float originalVolume;

    void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            
        }
        originalVolume = audioSource.volume;

    }

    void OnEnable()
    {
        
        updateTimer = 0f;
    }

    void Update()
    {
        updateTimer += Time.deltaTime;

        if (updateTimer >= updateInterval)
        {
            updateTimer = 0f;

            float sfxSetting = CoreScript.Instance.SFXSetting;
            audioSource.volume = originalVolume * sfxSetting;
        }
    }
}
