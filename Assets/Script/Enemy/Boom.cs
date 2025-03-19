using System.Collections;
using UnityEngine;

public class Boom : MonoBehaviour
{
    public GameObject boomEffectPrefab; // ���� ȿ��
    public GameObject boomRange;
    public GameObject paperPrefab;      // ���� ��ź ����

    SpriteRenderer sprite;
    Rigidbody2D rigid;
    Vector3 targetPos;
    bool isFly = true;

    Vector3[] pathPoints; // ������ ��� ����
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

    public void Launch(Vector3 startPos, Vector3 targetPos) // �߻�
    {
        sprite = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();

        transform.position = startPos;
        this.targetPos = targetPos;

        boomRange.SetActive(false);

        // ��ǥ ��ġ�� ���ϴ� ���� ���� ���
        Vector3 direction = (targetPos - startPos).normalized;

        // ��ǥ ��ġ������ �Ÿ� ���
        float distance = Vector3.Distance(startPos, targetPos);

        // �ӵ��� �Ÿ� ��ʷ� ���� (�ε巴�� �̵�)
        float moveSpeed = 5f; // �⺻ �̵� �ӵ�
        float scaledSpeed = Mathf.Clamp(moveSpeed * (distance / 5f), 2f, moveSpeed);

        // Rigidbody2D�� velocity�� �̵�
        rigid.velocity = direction * scaledSpeed;

        // ���� ���� �غ�
        StartCoroutine(ForceBoom(distance / scaledSpeed)); // ���� �̵� �ð� ��� Ÿ�̸� ����
    }


    IEnumerator ForceBoom(float timeLimit)                  // ��ȭ
    {
        yield return new WaitForSeconds(timeLimit + 0.5f);

        if (isFly) // ������ ���� ���̶�� ���� ���� �غ�
        {
            rigid.velocity = Vector2.zero;
            isFly = false;
            StartCoroutine(PrepareBoom());
        }
    }


    IEnumerator PrepareBoom()                               // ����
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

        // ���� �߻�
        switch(GameManager.instance.curMap)
        {
            case 0:
                break;
            case 1:
                LaunchPapers();
                break;
        }

        // ���� ȿ��
        AudioManager.instance.PlaySfx(AudioManager.SFX.Boom);
        Instantiate(boomEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    void LaunchPapers()                                     // ����
    {
        Vector2[] directions = new Vector2[]
        {
        Vector2.up,    // ��
        Vector2.down,  // ��
        Vector2.left,  // ��
        Vector2.right  // ��
        };

        float paperSpeed = 5f;

        foreach (Vector2 direction in directions)
        {
            // ����
            GameObject paper = Instantiate(paperPrefab, transform.position, Quaternion.identity);

            // ����
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            paper.transform.rotation = Quaternion.Euler(0, 0, angle);

            // �߻�
            Rigidbody2D paperRigid = paper.GetComponent<Rigidbody2D>();
            if (paperRigid != null)
            {
                paperRigid.velocity = direction * paperSpeed;
            }
        }
    }                               
}
