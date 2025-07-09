using DG.Tweening;
using System;

using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    [field : SerializeField] public int Floor { get; set; } = 0;

    [field: SerializeField] public GameObject currentStage { get; set; }
    [SerializeField] private GameObject elevatorPrefab;
    [SerializeField] private Transform ctransform;
    [SerializeField] private GameObject endingStage;
    public static StageManager Instance { get; private set; }

    private void Awake()
    {
        ctransform.transform.position = new Vector3(0, 0, 0);
        if (Instance == null)
        {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        currentStage = GameObject.FindAnyObjectByType<StageObject>()?.gameObject;
        ResetPos(GameObject.FindGameObjectWithTag("Player"));
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Floor = 0;

        currentStage = GameObject.FindAnyObjectByType<StageObject>()?.gameObject;

        if (GameObject.FindGameObjectWithTag("Player") is GameObject player)
        {
            ResetPos(player);
        }
    }

    public void SetStage(GameObject stage)
    {
        if (Floor >= 99)
        {
           stage = endingStage;

        }
        if (currentStage != null)
        {
            Destroy(currentStage);
        }

        EnemyDestroy(GameObject.FindGameObjectsWithTag("Enemy"));
        GameObject[] elevators = GameObject.FindGameObjectsWithTag("Elevator");
        foreach (var elevator in elevators)
        {
            Destroy(elevator);
        }

        currentStage = Instantiate(stage, transform.position, Quaternion.identity);
        Floor++;
        
        ResetPos(GameObject.FindGameObjectWithTag("Player"));
        Entity en = GameObject.FindGameObjectWithTag("Player").GetComponent<Entity>();
        en.statObject.ArrowPoint += 100;
        
        if (Floor < 33)
        {
            en.Heal(en.statObject.MaxHealth / 4f);
        }
        else if (Floor < 66)
        {
            en.Heal(en.statObject.MaxHealth / 5f);
        }
        else
        {
            en.Heal(en.statObject.MaxHealth / 6f);
        }
        ctransform.transform.position = new Vector3(0, 0, 0);

    }

    private void EnemyDestroy(GameObject[] gameObjects)
    {
        foreach (var go in gameObjects)
        {
            if (go != null)
            {
                Destroy(go);
            }
        }
    }

    public void ResetPos(GameObject go)
    {
        go.transform.position = currentStage.GetComponent<StageObject>().SpawnPoint.position;
    }

    public void StageClear()
    {
        if (currentStage == null) return;

        StageObject stageObj = currentStage.GetComponent<StageObject>();
        if (stageObj == null) return;

        if (stageObj.ElevatorPos != null)
        {
            foreach (var pos in stageObj.ElevatorPos)
            {
                if (pos != null)
                {
                    GameObject elevator = Instantiate(elevatorPrefab, pos.position, pos.rotation);
                }
            }
        }
    }

}
