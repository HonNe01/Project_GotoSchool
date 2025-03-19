using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonSelect : MonoBehaviour
{
    public GameObject[] charObjs;        // ĳ���� ������ �迭
    public GameObject charStatusPanel;   // ĳ���� ����â
    public GameObject characterSelectPanel;
    public Transform enterPosition;         // ���� ��ġ
    public Transform exitPosition;          // ���� ��ġ

    public GameObject bus;
    public Image fade;

    private GameObject curChar;

    [SerializeField] private bool isChange = false;
    [SerializeField] private int curCharIndex = 0;
    

    void Start()
    {
        // ó���� ĳ���� 0 ���� ���·� ����
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

        // Bus ����
        busAnim.SetTrigger("Stop");
        AudioManager.instance.PlaySfx(AudioManager.SFX.Bus);
        yield return new WaitForSeconds(3f);

        curChar.SetActive(false);

        // Bus ���
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

        // ����â �ݱ�
        charStatusPanel.SetActive(false);

        // ���� ĳ���� ����
        StartCoroutine(ChangeChar(charIndex));
        Debug.Log("Char Selected: " + charIndex);
    }

    IEnumerator ChangeChar(int newCharIndex)
    {
        // �̵� ����
        float elapsedTime = 0f;
        float duration = 1.5f;

        Vector3 curStartPosition = curChar.transform.position;
        Vector3 curTargetPosition = exitPosition.position;
        Vector3 newStartPosition = enterPosition.position;
        Vector3 newTargetPosition = new(-5.33f, -2.5f);

        // ���� ĳ�� Ȱ��
        GameObject newChar = charObjs[newCharIndex];
        newChar.transform.position = enterPosition.position;
        newChar.SetActive(true);

        // ���� ĳ��
        Animator newAnim = newChar.GetComponent<Animator>();
        newAnim.SetFloat("Speed", 1f);

        // ���� ĳ��
        Animator curAnim = curChar.GetComponent<Animator>();
        curAnim.SetFloat("Speed", 1f);

        // ĳ���� �̵�
        while (elapsedTime < duration)
        {
            curChar.transform.position = Vector3.Lerp(curStartPosition, curTargetPosition, elapsedTime / duration);
            newChar.transform.position = Vector3.Lerp(newStartPosition, newTargetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ĳ���� ����
        newAnim.SetFloat("Speed", 0f);
        curAnim.SetFloat("Speed", 0f);
        curChar.SetActive(false);

        curChar = newChar;
        curCharIndex = newCharIndex;

        // �ش� ĳ������ ����â ����
        charStatusPanel.SetActive(true);
        isChange = false;
    }
}
