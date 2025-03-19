using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public Transform[] mapButton;

    private GameObject curMap;
    public GameObject startButton;
    public Animator cameraAnim;
    public Camera mainCamera;
    public Image fade;

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float curTime = 1f;
        Color color = fade.color;

        while (curTime > 0f)
        {
            curTime -= Time.deltaTime;
            color.a = Mathf.Clamp01(curTime / 1f);
            fade.color = color;
            yield return null;
        }
    }

    public void Update()
    {
        // ESC 키 누르면 맵 선택 취소
        if (Input.GetKeyDown(KeyCode.Escape) && GameManager.instance.curMap != -1)
        {
            OnCancel();
        }
    }

    public void OnMapSelect(int mapIndex)
    {
        StartCoroutine(OnMapSelectAnim(mapIndex));
    }

    IEnumerator OnMapSelectAnim(int mapIndex)
    {
        AudioManager.instance.PlaySfx(AudioManager.SFX.Select);
        GameManager.instance.curMap = mapIndex;

        // 맵 선택 연출
        cameraAnim.SetTrigger("MapSelect" + mapIndex);
        mainCamera.orthographicSize = 2.5f;

        Debug.Log("Map Selected: " + mapIndex);

        yield return new WaitForSeconds(0.5f);
        startButton.SetActive(true);
    }

    public void OnMapConfirm()
    {
        AudioManager.instance.PlaySfx(AudioManager.SFX.Select);

        Debug.Log("Map Selected Completed : Map " + GameManager.instance.curMap);

        SceneManager.LoadScene("Loading");
    }

    void OnCancel()
    {
        Debug.Log("Map Selected Cancel");
        startButton.SetActive(false);
        AudioManager.instance.PlaySfx(AudioManager.SFX.Select);

        GameManager.instance.curMap = -1;
        cameraAnim.SetTrigger("MoveInit");
        CameraPosSetInit();
    }

    void CameraPosSetInit()
    {
        mainCamera.transform.position = new Vector3(0f, 0f, -10f);
        mainCamera.orthographicSize = 6f;
    }
}
