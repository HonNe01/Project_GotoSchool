using UnityEngine;

public class BoomEffect : MonoBehaviour
{
    private Collider2D coll;


    private void Awake()
    {
        coll = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        Invoke(nameof(ColliderOff), 0.2f);
    }

    void ColliderOff()
    {
        if (coll != null)
        {
            coll.enabled = false;
        }
    }

    void doDestroy()
    {
        Destroy(gameObject);
    }
}
