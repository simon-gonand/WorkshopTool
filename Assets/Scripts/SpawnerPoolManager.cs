using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerPoolManager : MonoBehaviour
{
    public EthanBehaviour[] enemies;
    public GameObject enemyPrefab;
    public int poolSize;
    public float enemySpawnRate;

    bool canEnemySpawn = true;

    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        EthanBehaviour ethan = GetFreeElement();
        if (ethan)
            ethan.StartLiving();
    }

    public EthanBehaviour GetFreeElement()
    {
        for (int i = 0; i < enemies.Length; ++i)
        {
            if (!enemies[i].isEnable)
                return enemies[i];
        }

        return null;
    }

    IEnumerator Timer()
    {
        canEnemySpawn = false;
        yield return new WaitForSeconds(enemySpawnRate);
        canEnemySpawn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (canEnemySpawn)
        {
            SpawnEnemy();
            StartCoroutine(Timer());
        }
    }
}
