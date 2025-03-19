using System.Collections;
using UnityEngine;

public class Floor : MonoBehaviour
{
    private readonly float[] scaleValues = { 0f, 0.5f, 0.63f };
    private SpriteRenderer imageRenderer;
    private SpriteRenderer gageRenderer;
    

    private int curStage = 0;


    public Collider2D[] colliders;
    public GameObject imageObj;
    public GameObject gageObj;
    public Sprite[] imageSprites;
    public Sprite[] gageSprites;
    public float scaleSpd = 1f;

    public bool IsFinished { get; private set; } = false;

    private void Awake()
    {
        imageRenderer = imageObj.GetComponent<SpriteRenderer>();
        gageRenderer = gageObj.GetComponent<SpriteRenderer>();

        colliders = GetComponentsInChildren<Collider2D>();
    }

    private void OnEnable()
    {
        foreach (var col in colliders)
        {
            col.enabled = false;
        }
        IsFinished = false;
        curStage = 0;

        imageRenderer.sprite = imageSprites[0];
        gageRenderer.sprite = gageSprites[0];

        gageObj.transform.localScale = Vector3.zero;

        StartCoroutine(FloorLogic());
    }

    IEnumerator FloorLogic()
    {
        for (int i = 0; i < imageSprites.Length; i++)
        {
            // 게이지 스케일 초기화
            float startScale = scaleValues[i];
            float progress = 0f;

            while (progress < 1f)
            {
                progress += Time.deltaTime * scaleSpd;
                float newScale = Mathf.Lerp(startScale, 1f, progress);
                gageObj.transform.localScale = Vector3.one * newScale;
                yield return null;
            }

            colliders[curStage].enabled = true;
            AudioManager.instance.PlaySfx(AudioManager.SFX.Boom);
            yield return new WaitForSeconds(0.1f);
            colliders[curStage].enabled = false;

            curStage++;
            if (curStage < imageSprites.Length)
            {
                imageRenderer.sprite = imageSprites[curStage];
                gageRenderer.sprite = gageSprites[curStage] ;

                gageObj.transform.localScale = Vector3.one * 0.3f * (curStage + 1);
            }
        }

        IsFinished = true;
        gameObject.SetActive(false);
    }
}
