using UnityEngine;

public class BGScroll : MonoBehaviour
{
    public Transform[] sprites;
    public int startIndex;
    public int endIndex;

    public float speed = 1f;

    float viewWidth;

    private void Awake()
    {
        viewWidth = (Camera.main.orthographicSize * 2) * (16f/9f);
    }

    void Update()
    {
        // Sprite Move
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.left * speed * Time.deltaTime;

        transform.position = curPos + nextPos;

        if (sprites[endIndex].position.x < -viewWidth)
        {
            // Sprite Reset
            Vector3 leftSpritePos = sprites[startIndex].localPosition;
            Vector3 rightSpritePos = sprites[endIndex].localPosition;
            sprites[endIndex].transform.localPosition = leftSpritePos + Vector3.right * viewWidth;

            // Index Reset
            int index = startIndex;
            startIndex = endIndex;
            endIndex = index - 1 == -1 ? sprites.Length - 1 : index - 1;
        }
    }
}
