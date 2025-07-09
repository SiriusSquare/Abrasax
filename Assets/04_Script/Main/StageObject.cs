using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public enum StageClearType
{
    NoNeed = 0,
    KillAllEnemy = 1,
    KillEnemyCount = 2,
    AllButtonPressd = 3,
    Stay = 4,
    other = 5,
}
public interface IStageSeterble
{
    public void StageSetting();
}

public class StageObject : MonoBehaviour
{
    [field: SerializeField] public Transform SpawnPoint { get; set; }
    [field: SerializeField] public Transform[] ElevatorPos { get; set; }

    [field: SerializeField] public StageClearType Sct { get; set; }
    [field: SerializeField] public bool StageClear { get; set; }
    [field: SerializeField] public int StageSettingInt { get; set; }

    private bool routineStarted = false;

    [SerializeField] private AudioClip song;
    private void Start()
    {
        if (song != null)
        {
            MusicScriptMain.Instance.PlayMusic(song);
        }
        
    }
    private void Update()
    {
        if (StageClear) return;

        switch (Sct)
        {
            case StageClearType.NoNeed:
                StageClear = true;
                MarkStageClear();
                break;

            case StageClearType.KillAllEnemy:
                if (!routineStarted)
                {
                    routineStarted = true;
                    StartCoroutine(CheckKillAllEnemies());
                }
                break;

            case StageClearType.KillEnemyCount:
                if (!routineStarted)
                {
                    routineStarted = true;
                    StartCoroutine(CheckKillEnemyCount());
                }
                break;

            case StageClearType.Stay:
                if (!routineStarted)
                {
                    routineStarted = true;
                    StartCoroutine(CheckStayTime());
                }
                break;

        }
    }

    private void MarkStageClear()
    {
        StageClear = true;

        if (StageManager.Instance != null)
        {
            StageManager.Instance.StageClear();
        }
    }

    private IEnumerator CheckKillAllEnemies()
    {
        yield return new WaitForSeconds(2f);
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            MarkStageClear();
        }
        routineStarted = false;
    }

    private IEnumerator CheckKillEnemyCount()
    {
        var initialEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

        yield return new WaitForSeconds(2f);

        var currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (initialEnemies - currentEnemies >= StageSettingInt)
        {
            MarkStageClear();
        }

        routineStarted = false;
    }

    private IEnumerator CheckStayTime()
    {
        yield return new WaitForSeconds(StageSettingInt);
        MarkStageClear();
        routineStarted = false;
    }
}
