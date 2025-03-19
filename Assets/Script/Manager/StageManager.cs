using Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager instance;

    [Header(" # Camera Control")]
    private CinemachineConfiner2D confiner;
    public CinemachineVirtualCamera virtucalCamera;
    public Collider2D[] mapCollider;


    [Header(" # Stage Control")]
    public float maxGameTime = 2 * 10f;
    public float gameTime;
    public bool isGameRunning = false;
    public bool isBoss = false;

    public TextMeshProUGUI roundText;


    [Header(" # Game Objects")]
    public GameObject[] mapObjects;
    public GameObject[] navMeshObjects;
    public GameObject[] enemySpawner;

    public GameObject enemyCleaner;
    public GameObject bossPrefab;
    public PoolManager poolManager;
    public LevelUp uiLevelUp;
    public Result uiResult;

    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        confiner = virtucalCamera.GetComponent<CinemachineConfiner2D>();
    }

    private void Start()
    {
        GameSet();
    }

    void Update()
    {
        if (!isGameRunning || isBoss)
            return;

        // Stage Play Time
        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
            
            StartCoroutine(BossAppears());
        }
    }

    public IEnumerator ShowRound(int round)
    {
        Debug.Log("- Round " + (round + 1) + " Start -");
        roundText.text = "- Round " + (round + 1) + " -";

        float fadeTime = 1f;
        float timer = 0f;

        // Fade in
        while (timer <= fadeTime)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, timer / fadeTime);
            roundText.alpha = alpha;
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        // Fade Out
        timer = 0f;
        while (timer <= fadeTime)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, timer / fadeTime);
            roundText.alpha = alpha;
            yield return null;
        }
    }

    void GameSet()
    {
        isGameRunning = true;
        isBoss = false;

        // 플레이어 초기화
        PlayerManager.instance.ResetPlayer();
        PlayerManager.instance.player.gameObject.SetActive(true);


        // 맵, 스포너, 네비 설정 초기화
        foreach (var map in mapObjects) map.SetActive(false);
        foreach (var spawner in enemySpawner) spawner.SetActive(false);
        foreach (var navMesh in navMeshObjects) navMesh.SetActive(false);

        // 현재 맵, 스포너, 네비 활성화
        mapObjects[GameManager.instance.curMap].SetActive(true);
        SetMapCamera(GameManager.instance.curMap);
        enemySpawner[GameManager.instance.curMap].SetActive(true);
        navMeshObjects[GameManager.instance.curMap].SetActive(true);


        // 게임 시작
        Debug.Log("Stage Start");
        GameResume();

        // 임시 무기 지급
        uiLevelUp.Select(GameManager.instance.playerId % 2);

        // Sound
        AudioManager.instance.StopBgm();
        AudioManager.instance.PlayBgm(SceneManager.GetActiveScene().name);
    }

    public void SetMapCamera(int mapIndex)
    {
        confiner.m_BoundingShape2D = mapCollider[mapIndex];
    }

    public IEnumerator BossAppears()   // 보스 등장
    {
        
        isBoss = true;
        yield return new WaitForSeconds(0.5f);
        enemyCleaner.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        enemyCleaner.SetActive(false);
        Debug.Log("WARNING! 'Boss Appears' WARNING!");

        AudioManager.instance.PlayBgm("Boss");
        foreach (var spawner in enemySpawner) spawner.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        Instantiate(bossPrefab, transform.position, Quaternion.identity);
    }

    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }
    IEnumerator GameOverRoutine()
    {
        isGameRunning = false;
        Debug.Log("Game Over");

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
        GameStop();

        // Sound
        AudioManager.instance.StopBgm();
        AudioManager.instance.PlaySfx(AudioManager.SFX.Lose);
    }
    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());
    }
    IEnumerator GameVictoryRoutine()
    {
        isGameRunning = false;
        Debug.Log("Game Victory");
        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Win();
        GameStop();

        // Sound
        AudioManager.instance.StopBgm();
        AudioManager.instance.PlaySfx(AudioManager.SFX.Win);
    }

    public void GameStop()
    {
        isGameRunning = false;
        Time.timeScale = 0;

        Debug.Log("Game Stop");
    }
    public void GameResume()
    {
        isGameRunning = true;
        Time.timeScale = 1;

        Debug.Log("Game Resume");
    }
    public void GameExit()
    {
        ResetGame();

        SceneManager.LoadScene("Start");
    }

    void ResetGame()
    {
        gameTime = 0;
        isGameRunning = false;
        Time.timeScale = 1;
        

        AudioManager.instance.ResetAudio();
    }
}
