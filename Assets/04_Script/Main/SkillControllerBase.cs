using System.Collections.Generic;
using UnityEngine;

public abstract class SkillControllerBase : MonoBehaviour
{
    [SerializeField] protected Entity entity;
    [SerializeField] protected GameObject[] skillPrefabs;
    public List<SkillData> skillDatas = new();
    public List<Coroutine> skillCoroutines = new();
    protected virtual void Start()
    {
        int count = Mathf.Min(skillDatas.Count, skillPrefabs.Length);

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(skillPrefabs[i], transform);
            obj.SetActive(true);

            Skill skillComponent = obj.GetComponent<Skill>();
            if (skillComponent == null)
                continue;

            SkillData data = skillDatas[i];
            data.skill = skillComponent;
            skillDatas[i] = data;
        }
        for (int i = 0; i < skillDatas.Count; i++)
        {
            skillCoroutines.Add(null);
        }

        OnSkillsInitialized(); // 각 클래스가 오버라이드할 수 있음
    }

    protected virtual void OnDisable()
    {
        foreach (var data in skillDatas)
        {
            if (data.skill != null)
                data.skill.gameObject.SetActive(false);
        }
    }

    protected abstract void OnSkillsInitialized(); // UI 동기화 등 용도
    protected abstract void Update();
}
