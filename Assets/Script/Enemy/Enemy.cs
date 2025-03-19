using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public RuntimeAnimatorController[] animController;
    public Rigidbody2D target;

    [Header(" # Basic State")]
    public float health;
    public float maxHealth;
    public float attackRange;
    public float attackCooldown;

    public Collider2D attackCollider;

    bool isLive;
    bool isAttack;

    Animator anim;
    Collider2D coll;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;

    WaitForFixedUpdate wait;

    NavMeshAgent agent;

    [Header(" # Damage Effect")]
    public Transform textSpawnPoint;
    public bool isDirtyHit = false;
    public bool isPencilHit = false;
    public float dirtyTimer;
    public float dirtyInterval = 0.5f;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        wait = new WaitForFixedUpdate();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLive || isAttack)
            return;

        // NavMeshAgent
        if (target != null)
        {
            agent.SetDestination(target.position);
        }

        // Attack Range Check
        float distanceToPlayer = Vector2.Distance(transform.position, target.position);
        if (distanceToPlayer <= attackRange) 
        {
            StartCoroutine(Attack());
        }
    }

    /*  기존 이동 방식 NavMeshAgent 사용으로 불필요
    private void FixedUpdate()
    {
        // Death Check
        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit") || isAttack)
            return;

        // Move
        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;

        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }
    */

    private void LateUpdate()
    {
        if (!isLive) 
            return;

        // Sprite Flip
        spriteRenderer.flipX = target.position.x < rigid.position.x;
    }

    private void OnEnable()
    {
        target = PlayerManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;

        agent.enabled = true;
        agent.isStopped = false;

        spriteRenderer.sortingOrder = 2;
        anim.SetBool("Dead", false);
        anim.Play("Run");

        health = maxHealth;
        dirtyTimer = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isLive || collision.GetComponent<Bullet>() == null)
            return;

        // Bullet 태그 처리
        if (collision.CompareTag("Bullet"))
        {
            float damage = collision.GetComponent<Bullet>().damage;
            health -= damage;

            // Damage Effect
            DamageText(damage);
            StartCoroutine(KnockBack());
            DamagedEffect();
        }
        // Pencil 태그 처리
        else if (collision.CompareTag("Pencil"))
        {
            if (isPencilHit) // Pencil 데미지를 받을 수 있는 상태인지 확인
                return;


            StartCoroutine(PencilHit()); // Pencil 데미지 제한

            float damage = collision.GetComponent<Bullet>().damage;
            health -= damage;

            // Damage Effect
            DamageText(damage);
            StartCoroutine(KnockBack());
            DamagedEffect();
        }
    }

    IEnumerator PencilHit()
    {
        isPencilHit = true;

        yield return new WaitForSeconds(0.5f);

        isPencilHit = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Dirty") || !isLive)
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
            health -= damage;

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

    IEnumerator Attack()
    {
        isAttack = true;
        agent.isStopped = true;
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f); // Attack animation delay

        attackCollider.enabled = true;
        yield return new WaitForSeconds(0.2f); // Attack collider active time
        attackCollider.enabled = false;
        agent.isStopped = false;

        yield return new WaitForSeconds(attackCooldown);
        isAttack = false;
    }

    IEnumerator KnockBack()
    {
        yield return wait;

        Vector3 playerPos = PlayerManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;

        //rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);

        // NavMeshAgent KnockBack Logic
        agent.isStopped = true;
        transform.position += dirVec.normalized * 0.2f;
        yield return new WaitForSeconds(0.2f);
        agent.isStopped = false;
    }

    void DamageText(float damage)
    {
        if (StageManager.instance.isBoss)
            return;

        // Damage Text Effect
        GameObject damageText = StageManager.instance.poolManager.Get(2);
        Vector2 camaraPoint = RectTransformUtility.WorldToScreenPoint(Camera.main,
                                                                      textSpawnPoint.position + new Vector3(0, 0.6f, 0));
        RectTransformUtility.ScreenPointToLocalPointInRectangle(damageText.GetComponent<RectTransform>(),
                                                                camaraPoint,
                                                                Camera.main,
                                                                out Vector2 localPoint);
        damageText.GetComponent<RectTransform>().localPosition = localPoint;
        damageText.GetComponent<DamageText>().SetDamage(damage);
    }

    void DamagedEffect()
    {
        if (health > 0)
        {
            anim.ResetTrigger("Hit");
            anim.SetTrigger("Hit");

            AudioManager.instance.PlaySfx(AudioManager.SFX.Hit);
        }
        else
        {
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            agent.isStopped = true;
            agent.enabled = false;
            spriteRenderer.sortingOrder = 1;

            StopAllCoroutines();

            anim.SetTrigger("Hit");
            anim.SetBool("Dead", true);
            if (StageManager.instance.isGameRunning || !StageManager.instance.isBoss)
            {
                PlayerManager.instance.kill++;
                PlayerManager.instance.GetExp();
            }

            if (StageManager.instance.isGameRunning)
                AudioManager.instance.PlaySfx(AudioManager.SFX.Dead);
        }
    }

    void Dead()
    {
        gameObject.SetActive(false);
    }

    public void Init(SpawnData data, string enemyName)          // Normal
    {
        anim.runtimeAnimatorController = animController[data.spriteType];
        agent.speed = data.speed;
        maxHealth = data.health;
        health = data.health;
        transform.name = enemyName; // Enemy ID
    }

    public void Init(SpawnEliteData data, string enemyName)     // Elite
    {
        anim.runtimeAnimatorController = animController[data.spriteType];
        agent.speed = data.speed;
        maxHealth = data.health;
        health = data.health;
        transform.name = enemyName; // EliteEnemy ID
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
