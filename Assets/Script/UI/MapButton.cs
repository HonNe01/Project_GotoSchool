using System.Collections;
using UnityEngine;

public class MapButton : MonoBehaviour
{
    private MapManager mapManager;

    public int mapIndex;

    
    private void Start()
    {
        mapManager = GetComponentInParent<MapManager>();

        StartCoroutine(Floating());
    }

    private void OnMouseDown()
    {
        if (mapIndex == 2)
            return;

        mapManager.OnMapSelect(mapIndex);
    }

    IEnumerator Floating()
    {
        Vector3 startPos = transform.position;
        float length = 0.1f;
        float speed = 1.5f;

        while (true)
        {
            float newY = startPos.y + Mathf.Sin(Time.fixedTime * speed) * length;
            transform.position = new Vector3(startPos.x, newY, startPos.z);
            yield return null;
        }
    }
}
