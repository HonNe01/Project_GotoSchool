using UnityEngine;

public class CloudMove : MonoBehaviour
{
    public float speed = 1f;
    public float end;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (transform.position.x < end)
        {
            transform.position = new Vector3(-end, transform.position.y, transform.position.z);
        }
    }
}
