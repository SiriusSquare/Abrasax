using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

[System.Serializable]
public struct SkillData
{
    public string skillKey;
    public Skill skill;
    public GameObject skillslotObject;
    public Sprite skillIconSprite;
    public Image skillSlotImageObject;
    public TextMeshProUGUI skillKeyText;
    public GameObject skillCooldownImage;
}

public class SkillUI : MonoBehaviour
{
    public List<GameObject> skillSlotObjects;

    public List<Skill> skillReferences;

    public List<SkillData> skillDataList = new();

    public AudioSource failBuzzer;
    public AudioClip failBuzz;
    private Dictionary<string, SkillData> skillDictionary = new();
    private Dictionary<string, bool> cooldownFinishedEffectPlayed = new();
    private Dictionary<string, float> previousCooldowns = new();
    private Dictionary<Image, Color> originalColors = new();
    private bool isFlashing = false;
    private void Awake()
    {
        BuildSkillDataList();

        foreach (var data in skillDataList)
        {
            if (!skillDictionary.ContainsKey(data.skillKey))
            {
                skillDictionary[data.skillKey] = data;
                cooldownFinishedEffectPlayed[data.skillKey] = false;
                previousCooldowns[data.skillKey] = float.MaxValue;
            }

            if (data.skillslotObject != null && data.skillIconSprite != null)
            {
                if (data.skillSlotImageObject != null)
                {
                    data.skillSlotImageObject.sprite = data.skillIconSprite;
                }
            }
        }
    }

    private void Update()
    {
        foreach (var kvp in skillDictionary)
        {
            string key = kvp.Key;
            SkillData data = kvp.Value;
            Skill skill = data.skill;

            if (skill == null || data.skillCooldownImage == null)
                continue;

            Image cooldownImage = data.skillCooldownImage.GetComponent<Image>();
            if (cooldownImage == null) continue;

            float maxCooldown = skill.finalSkillCoolDown;
            float currentCooldown = skill.SkillCoolDown;

            float fillAmount = Mathf.Clamp01(currentCooldown / maxCooldown);
            cooldownImage.fillAmount = fillAmount;

            data.skillKeyText.text = key;

            if (previousCooldowns.TryGetValue(key, out float prevCooldown))
            {
                if (prevCooldown > 0f && currentCooldown <= 0f && !cooldownFinishedEffectPlayed[key])
                {
                    cooldownFinishedEffectPlayed[key] = true;
                    StartCoroutine(PlayCooldownFinishEffect(cooldownImage));
                }
                else if (currentCooldown > 0f)
                {
                    cooldownFinishedEffectPlayed[key] = false;
                }

                previousCooldowns[key] = currentCooldown;
            }
            else
            {
                previousCooldowns[key] = currentCooldown;
            }
        }
    }

    private void BuildSkillDataList()
    {
        skillDataList.Clear();

        for (int i = 0; i < skillSlotObjects.Count; i++)
        {
            GameObject slot = skillSlotObjects[i];
            if (slot == null) continue;

            SkillData data = new SkillData();
            data.skillslotObject = slot;
            data.skill = (i < skillReferences.Count) ? skillReferences[i] : null;

            data.skillKey = slot.name;

            Image[] images = slot.GetComponentsInChildren<Image>();
            if (images.Length > 1)
            {
                data.skillSlotImageObject = images[1];
                data.skillIconSprite = images[1].sprite;
            }

            data.skillKeyText = slot.GetComponentInChildren<TextMeshProUGUI>();

            Transform cooldown = slot.transform.Find("CooldawnImage");
            if (cooldown != null)
                data.skillCooldownImage = cooldown.gameObject;

            skillDataList.Add(data);
        }
    }

    public void ForceRefreshSkillDictionary()
    {
        skillDictionary.Clear();
        cooldownFinishedEffectPlayed.Clear();
        previousCooldowns.Clear();

        foreach (var data in skillDataList)
        {
            if (!skillDictionary.ContainsKey(data.skillKey))
            {
                skillDictionary[data.skillKey] = data;
                cooldownFinishedEffectPlayed[data.skillKey] = false;
                previousCooldowns[data.skillKey] = float.MaxValue;
            }
        }
    }

    private IEnumerator PlayCooldownFinishEffect(Image image)
    {
        Color originalColor = image.color;
        image.DOColor(Color.white, 0.15f);
        yield return new WaitForSeconds(0.15f);
        image.DOColor(originalColor, 0.15f);
    }


    public void PlayFailFeedback(string skillKey)
    {
        if (failBuzzer != null) failBuzzer.PlayOneShot(failBuzz);

        if (skillDictionary.TryGetValue(skillKey, out var data) && data.skillSlotImageObject != null)
        {
            StartCoroutine(FlashRed(data.skillSlotImageObject));
        }
    }


    private IEnumerator FlashRed(Image icon)
    {
        if (isFlashing)
            yield break;

        isFlashing = true;

        if (!originalColors.ContainsKey(icon))
        {
            originalColors[icon] = icon.color;
        }

        Color originalColor = originalColors[icon];

        float flashDuration = 0.1f;

            icon.DOColor(Color.red, flashDuration);
            yield return new WaitForSeconds(flashDuration);
            icon.DOColor(originalColor, flashDuration);
            yield return new WaitForSeconds(flashDuration);

        isFlashing = false;
    }

}
