using System.Collections;
using UnityEngine;

public class Boom : MonoBehaviour
{
    public GameObject boomEffectPrefab; // 폭발 효과
    public GameObject boomRange;
    public GameObject paperPrefab;      // 과제 폭탄 파편

    SpriteRenderer sprite;
    Rigidbody2D rigid;
    Vector3 targetPos;
    bool isFly = true;

    Vector3[] pathPoints; // 포물선 경로 점들
    int currentPointIndex;


    private void Update()
    {
        if (!isFly)
            return;

        if (Vector2.Distance(transform.position, targetPos) < 0.1f)
        {
            rigid.velocity = Vector2.zero;
            rigid.gravityScale = 0f;
            isFly = false;

            StartCoroutine(PrepareBoom());
        }
    }

    public void Launch(Vector3 startPos, Vector3 targetPos) // 발사
    {
        sprite = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();

        transform.position = startPos;
        this.targetPos = targetPos;

        boomRange.SetActive(false);

        // 목표 위치로 향하는 방향 벡터 계산
        Vector3 direction = (targetPos - startPos).normalized;

        // 목표 위치까지의 거리 계산
        float distance = Vector3.Distance(startPos, targetPos);

        // 속도를 거리 비례로 조정 (부드럽게 이동)
        float moveSpeed = 5f; // 기본 이동 속도
        float scaledSpeed = Mathf.Clamp(moveSpeed * (distance / 5f), 2f, moveSpeed);

        // Rigidbody2D의 velocity로 이동
        rigid.velocity = direction * scaledSpeed;

        // 강제 폭발 준비
        StartCoroutine(ForceBoom(distance / scaledSpeed)); // 예상 이동 시간 기반 타이머 설정
    }


    IEnumerator ForceBoom(float timeLimit)                  // 발화
    {
        yield return new WaitForSeconds(timeLimit + 0.5f);

        if (isFly) // 여전히 비행 중이라면 강제 폭발 준비
        {
            rigid.velocity = Vector2.zero;
            isFly = false;
            StartCoroutine(PrepareBoom());
        }
    }


    IEnumerator PrepareBoom()                               // 폭발
    {
        boomRange.SetActive(true);
        float blinkTime = 1f;
        switch (GameManager.instance.curMap)
        {
            case 0:
                blinkTime = 1.2f;

                break;
            case 1:
                blinkTime = 1f;

                break;
        }
        
        float curTime = 0f;
        

        while (curTime < blinkTime)
        {
            curTime += Time.deltaTime;
            

            Color color = sprite.color;
            color.a = Mathf.PingPong(curTime * 4f, 0.6f) + 0.4f;
            sprite.color = color;

            if (color.a >= 0.9f)
            {
                AudioManager.instance.PlaySfx(AudioManager.SFX.Tick);
            }
            
            yield return null;
        }

        // 파편 발사
        switch(GameManager.instance.curMap)
        {
            case 0:
                break;
            case 1:
                LaunchPapers();
                break;
        }

        // 폭발 효과
        AudioManager.instance.PlaySfx(AudioManager.SFX.Boom);
        Instantiate(boomEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    void LaunchPapers()                                     // 파편
    {
        Vector2[] directions = new Vector2[]
        {
        Vector2.up,    // 상
        Vector2.down,  // 하
        Vector2.left,  // 좌
        Vector2.right  // 우
        };

        float paperSpeed = 5f;

        foreach (Vector2 direction in directions)
        {
            // 생성
            GameObject paper = Instantiate(paperPrefab, transform.position, Quaternion.identity);

            // 각도
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            paper.transform.rotation = Quaternion.Euler(0, 0, angle);

            // 발사
            Rigidbody2D paperRigid = paper.GetComponent<Rigidbody2D>();
            if (paperRigid != null)
            {
                paperRigid.velocity = direction * paperSpeed;
            }
        }
    }                               
}
