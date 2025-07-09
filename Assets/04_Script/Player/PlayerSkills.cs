using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkills : SkillControllerBase
{
    [SerializeField] SkillUI skillUI;

    protected override void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame) TryUseSkill(5);
        if (Keyboard.current.fKey.wasPressedThisFrame) TryUseSkill(9);
        if (Keyboard.current.cKey.wasPressedThisFrame) TryUseSkill(8);
        if (Keyboard.current.vKey.wasPressedThisFrame) TryUseSkill(7);
        if (Keyboard.current.xKey.wasPressedThisFrame) TryUseSkill(6);
        if (Keyboard.current.rKey.wasPressedThisFrame) TryUseSkill(4);
        if (Keyboard.current.wKey.wasPressedThisFrame) TryUseSkill(3);
        if (Keyboard.current.dKey.wasPressedThisFrame) TryUseSkill(2);
        if (Keyboard.current.sKey.wasPressedThisFrame) TryUseSkill(1);
        if (Keyboard.current.aKey.wasPressedThisFrame) TryUseSkill(0);
    }

    private void TryUseSkill(int index)
    {
        if (skillDatas.Count <= index) return;

        var skillData = skillDatas?[index].skill;

        
        bool canUse = !entity.isAttacking && !entity.isHit && !entity.isflyHit
                      && skillData != null
                      && skillData.gameObject.activeInHierarchy
                      && entity.Stamina > skillData.useStamina
                      && entity.Health > skillData.useHealth
                      && entity.Mana > skillData.useMana
                      && (!skillData.SkillDissabled || skillData.SkillUseCount != 0);

        if (canUse)
        {
            StartCoroutine(skillData.TriggerSkill(entity));
        }
        else if ( entity.Stamina < skillData.useStamina
                      || entity.Health < skillData.useHealth
                      || entity.Mana < skillData.useMana)
        {
            PlayFailFeedback(index);
        }
    }

    private void PlayFailFeedback(int index)
    {

        
        if (skillUI.skillDataList.Count > index && skillUI.skillDataList[index].skillSlotImageObject != null)
        {
            var icon = skillUI.skillDataList[index].skillSlotImageObject;
            skillUI.PlayFailFeedback(skillUI.skillDataList[index].skillKey);
        }
    }


    private IEnumerator TriggerSkillWithTracking(int index)
    {
        yield return StartCoroutine(skillDatas[index].skill.TriggerSkill(entity));
        skillCoroutines[index] = null;
    }

    protected override void OnSkillsInitialized()
    {
        if (skillUI != null)
        {
            for (int i = 0; i < skillUI.skillDataList.Count && i < skillDatas.Count; i++)
            {
                var uiData = skillUI.skillDataList[i];
                var playerData = skillDatas[i];

                uiData.skill = playerData.skill;
                uiData.skillSlotImageObject.sprite = playerData.skillIconSprite;

                skillUI.skillDataList[i] = uiData;
                uiData.skill.Skillcolor = entity.color;
            }

            skillUI.ForceRefreshSkillDictionary();
        }
    }
}
