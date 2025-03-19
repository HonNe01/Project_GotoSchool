using UnityEngine;

public class Scanner : MonoBehaviour
{
    public float scanRange;
    public LayerMask targetLayer;
    public RaycastHit2D[] targets;
    public Transform nearestTarget;

    void FixedUpdate()
    {
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        nearestTarget = GetNearest();
    }

    Transform GetNearest()
    {
        Transform result = null;
        float diff = 100;

        foreach (RaycastHit2D target in targets) {
            Vector3 myPos = transform.position;
            Vector3 targetPos = target.transform.position;
            float curDiff = Vector3.Distance(myPos, targetPos);

            if (curDiff < diff) {
                diff = curDiff;
                result = target.transform;
            }
        }

        return result;
    }

    void OnDrawGizmos()
    {
        // 스캔 범위를 나타내는 원 그리기
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, scanRange);

        // 가장 가까운 타겟을 나타내는 선 그리기
        if (nearestTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, nearestTarget.position);
        }
    }
}
