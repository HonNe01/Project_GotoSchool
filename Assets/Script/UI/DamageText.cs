using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    private Vector3 dir;

    public TextMeshPro damageText;

    private void OnEnable()
    {
        transform.position = Vector3.zero;
        dir = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1f), 0).normalized;
    }

    private void Update()
    {
        transform.Translate(dir * Time.deltaTime);
    }

    public void SetDamage(float damage)
    {
        damageText.text = ((int)damage).ToString();
        StartCoroutine(TextEffect());
    }

    IEnumerator TextEffect()
    {
        float duration = 1.0f;
        float elapsedTime = 0f;
        Color initialColor = damageText.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            damageText.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
