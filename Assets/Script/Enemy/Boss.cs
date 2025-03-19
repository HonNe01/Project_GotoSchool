using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    [Header("Basic Settings")]
    private new Collider2D collider;
    private Rigidbody2D rigid;
    public float easyHealth;
    public float normalHealth;
    public float maxHealth;
    public float curHealth;
    

    [Header("Attack Settings")]
    [SerializeField] private float attackTimer;
    public float attackInterval;
    public float easyPatternDelay;
    public float normalPatternDelay;
    [SerializeField] private bool isAttack = false;

    [Header("Movement Settings")]
    [SerializeField] private bool isMove = false;
    [SerializeField] private bool isRest = false;
    private NavMeshAgent navMeshAgent;

    [Header("References")]
    public Transform player;
    public Transform textSpawnPoint;
    public GameObject floorPrefab;
    public GameObject boomPrefab;       // 과제 폭탄
    public GameObject cBulletPrefab;    // C
    private Animator anim;

    [Header("Damage Settings")]
    public bool isDirtyHit = false;
    public bool isPencilHit = false;
    public float dirtyTimer;
    public float dirtyInterval = 0.5f;


    private void Awake()
    {
        collider = GetComponent<Collider2D>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        textSpawnPoint = transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.instance.curMap == 0)
        {
            maxHealth = easyHealth;
        }
        else if (GameManager.instance.curMap == 1)
        {
            maxHealth = normalHealth;
        }
        curHealth = maxHealth;


        player = PlayerManager.instance.player.transform;

        
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        navMeshAgent.isStopped = false;
        navMeshAgent.enabled = true;

        StartCoroutine(BossThinking(Random.Range(2f, 4f)));
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAttack && !isMove)
        {
            StartCoroutine(Move());
        }
    }

    private void LateUpdate()
    {
        float curSpeed = navMeshAgent.velocity.magnitude;
        anim.SetFloat("Speed", curSpeed);
    } 

    IEnumerator Move()
    {
        isMove = true;
        Vector3 randomPos = player.position + (Random.insideUnitSphere * 10f);
        randomPos.z = 0;
        

        navMeshAgent.SetDestination(randomPos);
        while (navMeshAgent.pathPending || navMeshAgent.remainingDistance > 0.1f)
        {
            if (isAttack)
            {
                navMeshAgent.ResetPath();
                isMove = false;
                yield break;
            }

            yield return null;
        }
        
        isMove = false;
    }

    IEnumerator BossThinking(float randomTime)
    {
        foreach (GameObject spawner in StageManager.instance.enemySpawner)
        {
            spawner.SetActive(false);
        }
        navMeshAgent.isStopped = false;
        isAttack = false;


        Debug.Log("Boss is Thinking . . .");
        yield return new WaitForSeconds(randomTime);

        int randomValue = Random.Range(0, 100);

        if (randomValue < 10 && !isRest)       // 휴식
        {
            Debug.Log("Boss is resting");
            isRest = true;
            StartCoroutine(BossRest());
        }

        else if (randomValue < 30)  // 패턴 A : 출석
        {
            Debug.Log("Boss uses Attendance pattern");

            isRest = false;
            StartCoroutine(BossPatternA());
        }

        else if (randomValue < 50)  // 패턴 B : 과제 폭탄
        {
            Debug.Log("Boss uses Boom pattern");

            isRest = false;
            StartCoroutine(BossPatternB());
        }

        else if (randomValue < 70)  // 패턴 C : 학점 C
        {
            Debug.Log("Boss give C");

            isRest = false;
            StartCoroutine(BossPatternC());
        }
        else
        {
            Debug.Log("Boss is moving around");
            isAttack = false;
            isRest = true;

            StartCoroutine(BossThinking(2f));
        }
    }

    IEnumerator BossRest()      // 휴식
    {
        navMeshAgent.isStopped = true;

        yield return new WaitForSeconds(2f);

        navMeshAgent.isStopped = false;

        StartCoroutine(BossThinking(1f));
    }   

    IEnumerator BossPatternA()  // 출석
    {
        Debug.Log("Boss Pattern : 자~ 출석 부를게요");
        navMeshAgent.isStopped = true;
        isAttack = true;

        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.1f);

        // Floor 활성화
        Floor floor = floorPrefab.GetComponent<Floor>();
        floor.gameObject.SetActive(true);

        // 대기
        yield return new WaitUntil(() => floor.IsFinished);

        navMeshAgent.isStopped = false;
        isAttack = false;
        StartCoroutine(BossThinking(Random.Range(1f, 3f)));
    }

    IEnumerator BossPatternB()  // 과제
    {
        Debug.Log("Boss Pattern : 과제 나갑니다~");
        navMeshAgent.isStopped = true;
        isAttack = true;

        
        for (int i = 0; i < 3;  i++)
        {
            anim.SetTrigger("Attack");
            yield return new WaitForSeconds(0.1f);

            // 폭탄 생성
            GameObject bomb = Instantiate(boomPrefab, transform.position, Quaternion.identity);
            AudioManager.instance.PlaySfx(AudioManager.SFX.Range);

            // 폭탄 발사
            Vector3 targetPos = player.position;
            bomb.GetComponent<Boom>().Launch(transform.position, targetPos);

            yield return new WaitForSeconds(1.5f);
        }
        
        navMeshAgent.isStopped = false;
        isAttack = false;

        StartCoroutine(BossThinking(Random.Range(1f, 3f)));
    }

    IEnumerator BossPatternC()  // C 학점
    {
        Debug.Log("Boss Pattern : 자네는 C일세!");
        navMeshAgent.isStopped = true;
        isAttack = true;

        float spreadAngle = 30f;
        int bulletCount = 3;
        Vector3 dir = (player.position - transform.position).normalized;


        anim.SetTrigger("Attack");
        AudioManager.instance.PlaySfx(AudioManager.SFX.Range);
        for (int i = 0; i < bulletCount; i++)
        {
            // 각도 계산
            float angle = -spreadAngle + (spreadAngle * 2 * i / (bulletCount - 1));
            Vector3 rotateDir = Quaternion.Euler(0, 0, angle) * dir;

            // C 생성
            GameObject cBullet = Instantiate(cBulletPrefab, transform.position, Quaternion.identity);
            cBullet.GetComponent<cBullet>().Init(rotateDir);
        }


        yield return new WaitForSeconds(1f);
        navMeshAgent.isStopped = false;
        isAttack = false;
        StartCoroutine(BossThinking(Random.Range(2f, 4f)));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Bullet>() == null)
            return;

        // Bullet 태그 처리
        if (collision.CompareTag("Bullet"))
        {
            float damage = collision.GetComponent<Bullet>().damage;
            curHealth -= damage;

            // Damage Effect
            DamageText(damage);
            DamagedEffect();
        }
        // Pencil 태그 처리
        else if (collision.CompareTag("Pencil"))
        {
            if (isPencilHit) // Pencil 데미지를 받을 수 있는 상태인지 확인
                return;


            StartCoroutine(PencilHit()); // Pencil 데미지 제한

            float damage = collision.GetComponent<Bullet>().damage;
            curHealth -= damage;

            // Damage Effect
            DamageText(damage);
            DamagedEffect();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Dirty"))
            return;

        if (!isDirtyHit)
        {
            isDirtyHit = true;
        }

        dirtyTimer += Time.deltaTime;
        if (dirtyTimer >= dirtyInterval)
        {
            dirtyTimer = 0f;
            float damage = collision.GetComponent<Bullet>().damage;
            curHealth -= damage;

            // Damage Effect
            DamageText(damage);
            DamagedEffect();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Dirty"))
        {
            isDirtyHit = false;
        }
    }

    IEnumerator PencilHit()
    {
        isPencilHit = true;

        yield return new WaitForSeconds(0.5f);

        isPencilHit = false;
    }

    void DamageText(float damage)
    {
        // Damage Text Effect
        GameObject damageText = StageManager.instance.poolManager.Get(2);
        Vector2 camaraPoint = RectTransformUtility.WorldToScreenPoint(Camera.main,
                                                                      textSpawnPoint.position + new Vector3(0, 0.5f, 0));
        RectTransformUtility.ScreenPointToLocalPointInRectangle(damageText.GetComponent<RectTransform>(),
                                                                camaraPoint,
                                                                Camera.main,
                                                                out Vector2 localPoint);
        damageText.GetComponent<RectTransform>().localPosition = localPoint;
        damageText.GetComponent<DamageText>().SetDamage(damage);
    }

    void DamagedEffect()
    {
        if (curHealth > 0)
        {
            if (!isAttack)
            {
                anim.ResetTrigger("Hit");
                anim.SetTrigger("Hit");
            }
            

            AudioManager.instance.PlaySfx(AudioManager.SFX.Hit);
        }
        else
        {
            rigid.simulated = false;
            collider.enabled = false;
            navMeshAgent.isStopped = true;
            navMeshAgent.enabled = false;
            

            StopAllCoroutines();

            anim.SetTrigger("Hit");
            anim.SetBool("Dead", true);

            AudioManager.instance.PlaySfx(AudioManager.SFX.Dead);
            StartCoroutine(Dead());
        }
    }

    IEnumerator Dead()
    {
        yield return new WaitForSeconds(1f);

        StageManager.instance.GameVictory();
    }
}
