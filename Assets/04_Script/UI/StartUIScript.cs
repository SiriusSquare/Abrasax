using UnityEngine;
using UnityEngine.UI;
public class StartUIScript : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    private void Start()
    {
        bgmSlider.value = CoreScript.Instance.BGMSetting;
        sfxSlider.value = CoreScript.Instance.SFXSetting;
    }
    public void BGMChange()
    {
        CoreScript.Instance.BGMSetting = bgmSlider.value;
    }
    public void SFXChange()
    {
        CoreScript.Instance.SFXSetting = sfxSlider.value;
    }
}
