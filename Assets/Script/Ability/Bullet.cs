using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum Type { pencil, book, dirty }
    public Type type;

    public float damage;
    public int per;

    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (type == Type.book && type == Type.dirty) {
            transform.Rotate(Vector3.forward * 100 * Time.deltaTime);
        }
    }

    public void Init(float damage, int per, Vector3 dir)
    {
        this.damage = damage;
        this.per = per;

        if (per >= 0) {
            rigid.velocity = dir * 15f;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy") || per == -100)
            return;

        per--;
        AudioManager.instance.PlaySfx(AudioManager.SFX.Melee);

        if (per < 0) {
            rigid.velocity = Vector2.zero;
            gameObject.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area") || per == -100)
            return;

        gameObject.SetActive(false);
    }
}   