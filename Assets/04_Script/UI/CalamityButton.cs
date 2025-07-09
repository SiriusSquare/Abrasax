using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

using DG.Tweening;

public class CalamityButton : MonoBehaviour
{
    [SerializeField] private AudioClip DefClip;
    [SerializeField] private AudioClip calamityClip;
    [SerializeField] private GameObject calamityButton;
    [SerializeField] private Button calamityButtonComponent;
    [SerializeField] private Light2D[] calamityLight;
    [SerializeField] private ParticleSystem[] calamityParticle;
    private Color[] calamityLightdefcolor;
    private Color[] calamityParticledefcolor;
    [SerializeField] private Color[] calamitycolor;
    void Start()
    {
        calamityLightdefcolor = new Color[calamityLight.Length];
        calamityParticledefcolor = new Color[calamityParticle.Length];
        for (int i = 0; i < calamityLight.Length; i++)
        {
            if (calamityLight[i] != null)
            {
                calamityLightdefcolor[i] = calamityLight[i].color;
            }
        }

        for (int i = 0; i < calamityParticle.Length; i++)
        {
            if (calamityParticle[i] != null)
            {
                var main = calamityParticle[i].main;
                calamityParticledefcolor[i] = main.startColor.color;
            }
        }
        if (CoreScript.Instance.CalamityMode == true)
        {
            CalamityOn();
        }
        else
        {
            CalamityOff();
        }

    }




    public void CalamityChange()
    {
        if (CoreScript.Instance.CalamityMode == true)
        {
            CoreScript.Instance.CalamityMode = false;
            CalamityOff();

        }
        else
        {
            CoreScript.Instance.CalamityMode = true;
            CalamityOn();
        }
    }

    private void CalamityOff()
    {
        MusicScriptMain.Instance.PlayMusic(DefClip);
        ColorBlock colors = calamityButtonComponent.colors;
        colors.normalColor = Color.white;
        calamityButton.SetActive(false);
        for (int i = 0; i < calamityLight.Length; i++)
        {
            int index = i;
            Light2D light = calamityLight[index];
            Color targetColor = calamityLightdefcolor[index];

            Color startColor = light.color;

            DOTween.To(() => startColor, x => {
                startColor = x;
                light.color = x;
            }, targetColor, 0.5f)
            .SetEase(Ease.InOutSine);
        }

        for (int i = 0; i < calamityParticle.Length; i++)
        {
            int index = i;
            ParticleSystem particle = calamityParticle[index];
            var main = particle.main;
            Color targetColor = calamityParticledefcolor[index];
            Color startColor = main.startColor.color;
            DOTween.To(() => startColor, x =>
            {
                startColor = x;
                main.startColor = x;
            }, targetColor, 0.5f)
            .SetEase(Ease.InOutSine);
        }
    }

    private void CalamityOn()
    {
        MusicScriptMain.Instance.PlayMusic(calamityClip);
        ColorBlock colors = calamityButtonComponent.colors;
        colors.normalColor = Color.red;
        calamityButton.SetActive(true);
        for (int i = 0; i < calamityLight.Length; i++)
        {
            int index = i;
            Light2D light = calamityLight[index];
            Color targetColor = calamitycolor[0];

            Color startColor = light.color;

            DOTween.To(() => startColor, x => {
                startColor = x;
                light.color = x;
            }, targetColor, 0.5f)
            .SetEase(Ease.InOutSine);
        }

        for (int i = 0; i < calamityParticle.Length; i++)
        {
            int index = i;
            ParticleSystem particle = calamityParticle[index];
            var main = particle.main;
            Color targetColor = calamitycolor[1];
            Color startColor = main.startColor.color;
            
            DOTween.To(() => startColor, x =>
            {
                startColor = x;
                main.startColor = x;
            }, targetColor, 0.5f)
            .SetEase(Ease.InOutSine);
        }
    }
}
