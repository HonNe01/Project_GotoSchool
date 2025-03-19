using UnityEngine;

public class Hand : MonoBehaviour
{
    public enum HandType { Left, Right, Back }
    public HandType handType;

    public SpriteRenderer spriter;

    SpriteRenderer player;

    Vector3 rightPos = new Vector3(0.35f, -0.15f, 0);
    Vector3 rightPosReverse = new Vector3(-0.15f, -0.15f, 0);
    Quaternion leftRot = Quaternion.Euler(0, 0, -35);
    Quaternion leftRotReverse = Quaternion.Euler(0, 0, -135);

    Vector3 backPos = new Vector3(-0.3f, -0.2f, 0);
    Vector3 backPosReverse = new Vector3(0.3f, -0.2f, 0);

    void Awake()
    {
        player = GetComponentsInParent<SpriteRenderer>()[1];
    }

    void LateUpdate()
    {
        bool isReverse = player.flipX;


        switch (handType)
        {
            case HandType.Left:
                transform.localRotation = isReverse ? leftRotReverse : leftRot;
                spriter.flipY = isReverse;
                spriter.sortingOrder = isReverse ? 4 : 6;

                break;

            case HandType.Right:
                transform.localPosition = isReverse ? rightPosReverse : rightPos;
                spriter.flipX = isReverse;
                spriter.sortingOrder = isReverse ? 6 : 4;

                break;
            case HandType.Back:
                transform.localPosition = isReverse ? backPosReverse : backPos;
                spriter.flipX = isReverse;

                break;

        }
    }
}
