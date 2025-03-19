using UnityEngine;

public class cBullet : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(Vector3.forward * 200 * Time.deltaTime);
    }


    public void Init(Vector3 dir)
    {
        GetComponent<Rigidbody2D>().velocity = dir * 5f;

        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
