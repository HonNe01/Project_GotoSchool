using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Dictionary<Enemy, float> hitCooldowns = new Dictionary<Enemy, float>();     // 피격 딜레이 관리 딕셔너리

    public Vector2 inputVec;
    public float hitDelay = 1.0f;
    public float baseSpeed;
    public float bonusSpeed = 1f;

    [SerializeField] public float speed => baseSpeed * (1 + bonusSpeed);

    public RuntimeAnimatorController[] animCon;
    public Scanner scanner;
    public Hand[] hands;

    bool isHitEffect = false;

    Animator anim;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        rigid = GetComponent<Rigidbody2D>();
        hands = GetComponentsInChildren<Hand>(true);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        bonusSpeed = Character.Speed - 1f;
        anim.runtimeAnimatorController = animCon[GameManager.instance.playerId];
    }

    void Update()                   // Logic Update
    {
        // 피격 딜레이 관리
        List<Enemy> enemiesToReset = new List<Enemy>(hitCooldowns.Keys);    // 임시 리스트 생성
        foreach (var enemy in enemiesToReset)
        {
            hitCooldowns[enemy] -= Time.deltaTime; // 딜레이 계산
            if (hitCooldowns[enemy] <= 0)
            {
                hitCooldowns.Remove(enemy); // 딕셔너리 수정
            }
        }
    }

    private void FixedUpdate()      // Physics Update
    {
        if (!StageManager.instance.isGameRunning)
            return;

        // Move
        Vector2 nextVec = inputVec * speed * Time.deltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    private void LateUpdate()
    {
        // Sprite Flip
        if (inputVec.x != 0)
        {
            spriteRenderer.flipX = inputVec.x < 0;
        }

        // Animation
        anim.SetFloat("Speed", inputVec.magnitude);
    }   // Visual Update

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!StageManager.instance.isGameRunning)
            return;

        // Hit
        if (collision.CompareTag("EnemyAttack") || collision.CompareTag("EnemyBullet"))
        {
            // EnemyAttack인 경우 추가 처리
            if (collision.CompareTag("EnemyAttack"))
            {
                Enemy enemy = collision.transform.GetComponentInParent<Enemy>();
                if (enemy != null && (!hitCooldowns.ContainsKey(enemy) || hitCooldowns[enemy] <= 0))
                {
                    hitCooldowns[enemy] = hitDelay; // 피격 딜레이 설정
                }
                else
                {
                    return; // 피격 불가능한 상태면 종료
                }
            }

            // 플레이어 데미지 처리
            PlayerManager.instance.TakeDamage(2);
            AudioManager.instance.PlaySfx(AudioManager.SFX.Damaged);
            Debug.Log($"Hit! : {collision.name} Cur Health : {PlayerManager.instance.health}");

            // 피격 효과 실행
            if (!isHitEffect)
                StartCoroutine(HitEffect());
        }

        // Dead
        if (PlayerManager.instance.health < 0)
        {
            for (int i = 2; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }

            anim.SetTrigger("Dead");
        }
    }

    IEnumerator HitEffect()
    {
        isHitEffect = true;
        int countTime = 0;

        while (countTime < 10)
        {
            if (countTime % 2 == 0)
                spriteRenderer.color = new Color32(255, 255, 255, 90);
            else
                spriteRenderer.color = new Color32(255, 255, 255, 180);
            yield return new WaitForSeconds(0.2f);

            countTime++;
        }

        spriteRenderer.color = Color.white;
        isHitEffect = false;
    }   // 피격 효과
}
