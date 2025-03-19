using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    public SpawnEliteData[] spawnEliteData;

    public int enemyCount = 0;

    public float roundTime;

    int round;
    int eliteSpawnCount = 0;

    float timer;
    float[] eliteSpawnTimes = new float[3];

    void Start()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
        roundTime = StageManager.instance.maxGameTime / spawnData.Length;

        SetEliteSpawnTimes();
        StartCoroutine(StageManager.instance.ShowRound(round));
    }

    void Update()
    {
        if (!StageManager.instance.isGameRunning || round > spawnData.Length - 1)
            return;

        timer += Time.deltaTime;
        int curRound = Mathf.Clamp(Mathf.FloorToInt(StageManager.instance.gameTime / roundTime), 0, spawnData.Length - 1);

        // Round Check
        if (curRound != round)
        {
            round = curRound;
            OnRoundChanged();
        }


        // Normal Enemy Spawn
        if (timer > spawnData[round].spawnTime) {
            timer = 0;
            Spawn();
        }

        // Elite Enemy Spawn
        float stageTime = StageManager.instance.gameTime - (round * roundTime);
        
        if (eliteSpawnCount < eliteSpawnTimes.Length && stageTime > eliteSpawnTimes[eliteSpawnCount])
        {
            SpawnElite();
            eliteSpawnCount++;
        }
    }

    void Spawn()
    {
        GameObject enemy = StageManager.instance.poolManager.Get(0);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        enemy.GetComponent<Enemy>().Init(spawnData[round], "Enemy_" + enemyCount++);
    }

    void SpawnElite()
    {
        GameObject eliteEnemy = StageManager.instance.poolManager.Get(1);
        eliteEnemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        eliteEnemy.GetComponent<Enemy>().Init(spawnEliteData[round], "EliteEnemy_" + round);

        Debug.Log("Elite Enemy Spawned at Stage " + (round + 1));
    }

    void SetEliteSpawnTimes()
    {
        // Elite Spawn Times Set
        eliteSpawnTimes[0] = Random.Range(roundTime * 0.2f, roundTime * 0.3f);
        eliteSpawnTimes[1] = Random.Range(roundTime * 0.4f, roundTime * 0.5f);
        eliteSpawnTimes[2] = Random.Range(roundTime * 0.6f, roundTime * 0.7f);

        Debug.Log("Elite Enemy Spawn Times: " + eliteSpawnTimes[0] + ", " + eliteSpawnTimes[1] + ", " + eliteSpawnTimes[2]);
    }

    void OnRoundChanged()
    {
        // Elite Spawn Time Init
        eliteSpawnCount = 0;
        SetEliteSpawnTimes();

        // Round Fade In/Out
        StartCoroutine(StageManager.instance.ShowRound(round));
    }
}

[System.Serializable]
public class SpawnData
{
    public float spawnTime;
    public int spriteType;
    public int health;
    public float speed;
}

[System.Serializable]
public class SpawnEliteData
{
    public int spriteType;
    public int health;
    public float speed;
}
