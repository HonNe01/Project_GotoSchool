using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonSelect : MonoBehaviour
{
    public GameObject[] charObjs;        // 캐릭터 프리펩 배열
    public GameObject charStatusPanel;   // 캐릭터 설명창
    public GameObject characterSelectPanel;
    public Transform enterPosition;         // 입장 위치
    public Transform exitPosition;          // 퇴장 위치

    public GameObject bus;
    public Image fade;

    private GameObject curChar;

    [SerializeField] private bool isChange = false;
    [SerializeField] private int curCharIndex = 0;
    

    void Start()
    {
        // 처음에 캐릭터 0 선택 상태로 설정
        curChar = charObjs[curCharIndex];
        curChar.SetActive(true);
        charStatusPanel.SetActive(true);
    }

    public void OnSelectConfirm()
    {
        Debug.Log("Char Selected Completed : Char " + GameManager.instance.playerId);
        AudioManager.instance.PlaySfx(AudioManager.SFX.Select);

        StartCoroutine(BusAnim());
    }

    IEnumerator BusAnim()
    {
        Animator busAnim = bus.GetComponent<Animator>();
        characterSelectPanel.SetActive(false);

        // Bus 정차
        busAnim.SetTrigger("Stop");
        AudioManager.instance.PlaySfx(AudioManager.SFX.Bus);
        yield return new WaitForSeconds(3f);

        curChar.SetActive(false);

        // Bus 출발
        busAnim.SetTrigger("Go");
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float curTime = 0f;
        Color color = fade.color;

        while (curTime < 1f)
        {
            curTime += Time.deltaTime;
            color.a = Mathf.Clamp01(curTime / 1f);
            fade.color = color;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene("Map");
    }

    public void OnSelect(int charIndex)
    {
        AudioManager.instance.PlaySfx(AudioManager.SFX.Select);

        if (curCharIndex == charIndex || isChange)
            return;

        isChange = true;
        GameManager.instance.playerId = charIndex;

        // 설명창 닫기
        charStatusPanel.SetActive(false);

        // 기존 캐릭터 퇴장
        StartCoroutine(ChangeChar(charIndex));
        Debug.Log("Char Selected: " + charIndex);
    }

    IEnumerator ChangeChar(int newCharIndex)
    {
        // 이동 설정
        float elapsedTime = 0f;
        float duration = 1.5f;

        Vector3 curStartPosition = curChar.transform.position;
        Vector3 curTargetPosition = exitPosition.position;
        Vector3 newStartPosition = enterPosition.position;
        Vector3 newTargetPosition = new(-5.33f, -2.5f);

        // 선택 캐릭 활성
        GameObject newChar = charObjs[newCharIndex];
        newChar.transform.position = enterPosition.position;
        newChar.SetActive(true);

        // 선택 캐릭
        Animator newAnim = newChar.GetComponent<Animator>();
        newAnim.SetFloat("Speed", 1f);

        // 기존 캐릭
        Animator curAnim = curChar.GetComponent<Animator>();
        curAnim.SetFloat("Speed", 1f);

        // 캐릭터 이동
        while (elapsedTime < duration)
        {
            curChar.transform.position = Vector3.Lerp(curStartPosition, curTargetPosition, elapsedTime / duration);
            newChar.transform.position = Vector3.Lerp(newStartPosition, newTargetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 캐릭터 정지
        newAnim.SetFloat("Speed", 0f);
        curAnim.SetFloat("Speed", 0f);
        curChar.SetActive(false);

        curChar = newChar;
        curCharIndex = newCharIndex;

        // 해당 캐릭터의 설명창 오픈
        charStatusPanel.SetActive(true);
        isChange = false;
    }
}
