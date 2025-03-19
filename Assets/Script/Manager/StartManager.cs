using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public GameObject characterSelectPanel;
    public GameObject startPanel;
    public Animator cameraAnim;
    public Camera mainCamera;


    private void Start()
    {
        characterSelectPanel.SetActive(false);
        startPanel.SetActive(true);

        CameraPosSetInit();
        AudioManager.instance.StopBgm();
        AudioManager.instance.PlayBgm(SceneManager.GetActiveScene().name);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopAllCoroutines();

            StartCoroutine(OnCancel());
        }
    }

    void CameraPosSetInit()
    {
        mainCamera.transform.position = new Vector3(0f, 0f, -10f);
        mainCamera.orthographicSize = 6f;
    }

    void CameraPosSetSelect()
    {
        mainCamera.transform.position = new Vector3(-5.3f, -2.7f, -10f);
        mainCamera.orthographicSize = 2.5f;
    }

    public void OnSelect()
    {
        StartCoroutine(OnSelectAnim());
    }

    IEnumerator OnSelectAnim()
    {
        Debug.Log("Char Select On : Default Select 0");
        AudioManager.instance.PlaySfx(AudioManager.SFX.Select);

        startPanel.SetActive(false);
        cameraAnim.SetTrigger("MoveSelect");
        CameraPosSetSelect();

        yield return new WaitForSeconds(0.5f);
        characterSelectPanel.SetActive(true);
    }

    IEnumerator OnCancel()
    {
        Debug.Log("Char Selected Cancel");
        AudioManager.instance.PlaySfx(AudioManager.SFX.Select);

        characterSelectPanel.SetActive(false);
        cameraAnim.SetTrigger("MoveInit");
        CameraPosSetInit();

        yield return new WaitForSeconds(0.5f);
        startPanel.SetActive(true);

        // 버그 돌려막기
        GameObject startButton = startPanel.transform.Find("Button_Start").gameObject;
        startButton.SetActive(true);
    }
}
