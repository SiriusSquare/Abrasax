using UnityEngine;
using System.Collections;

public class SimpleWaveSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private float[] probabilities;
    [SerializeField] private float interval = 1f;

    [SerializeField] private int totalSpawns = 10;
    [SerializeField] private GameObject teleportPrefab;

    private void Start()
    {
        if (enemyPrefabs.Length != probabilities.Length)
        {
            Debug.LogError("enemyPrefabs과 probabilities의 크기가 동일해야 합니다.");
            Destroy(gameObject);
            return;
        }

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        

        for (int i = 0; i < totalSpawns; i++)
        {
            SpawnEnemy();
            IPoolable s = PoolManager.Instance.Pop(teleportPrefab.name);
            if (s == null)
            {
                Debug.LogError($"PoolManager에서 {teleportPrefab.name} 이름으로 Pop 실패");
            }

            s.GameObject.transform.position = transform.position;
            s.GameObject.SetActive(true);
            yield return new WaitForSeconds(interval);
        }

        Destroy(gameObject);
    }

    private void SpawnEnemy()
    {
        float totalWeight = 0f;

        foreach (var p in probabilities) totalWeight += p;

        float rand = Random.value * totalWeight;

        for (int j = 0; j < probabilities.Length; j++)
        {
            if (rand < probabilities[j])
            {
                Instantiate(enemyPrefabs[j], transform.position, Quaternion.identity);
                return;
            }
            rand -= probabilities[j];
        }
    }
}
