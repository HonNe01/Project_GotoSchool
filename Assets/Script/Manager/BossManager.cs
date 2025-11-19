using System.Collections;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public static BossManager instance;

    [Header("Enemy Setting")]
    public GameObject[] enemySpawner;
    public GameObject enemyCleaner;


    [Header("Boss Setings")]
    public GameObject bossPrefab;
    public GameObject bossHealth; 
    public bool isBoss;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        isBoss = false;
        bossHealth.SetActive(false);
    }
    
    public void SpawnBoss()
    {
        if (isBoss) return;
        
        isBoss = true;
        StartCoroutine(Co_SpawnBoss());
    }

    private IEnumerator Co_SpawnBoss()
    {
        // Map Clear
        yield return new WaitForSeconds(0.5f);
        enemyCleaner.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        enemyCleaner.SetActive(false);
        foreach (var spawner in enemySpawner) spawner.SetActive(false);

        // Boss Spawn
        AudioManager.instance.PlayBgm("Boss");
        Debug.Log("WARNING! 'Boss Appears' WARNING!");
        yield return new WaitForSeconds(0.3f);
        Instantiate(bossPrefab, Vector3.zero, Quaternion.identity);
        bossHealth.SetActive(true);
    }
}
