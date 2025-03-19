using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    private AsyncOperation operation;


    public Slider progressBar; // ���൵ �����̴�
    public TextMeshProUGUI progressText;  // ���൵ �ؽ�Ʈ
    

    void Start()
    {
        // ���� �� �ε�
        StartCoroutine(LoadScene("Stage"));
    }

    IEnumerator LoadScene(string sceneName)
    {
        // �� �񵿱� �ε�
        operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false; // �ڵ� �� ��ȯ ����

        float timer = 0f;

        // ���൵ ������Ʈ
        while (!operation.isDone)
        {
            // �ε� ���൵�� 0~0.9 ���� (�ε��� 90%���� �Ϸ��)
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            timer += Time.deltaTime;
            progressBar.value = Mathf.Lerp(progressBar.value, progress, timer);

            
            if (progressText != null)
            {
                progressText.text = Mathf.RoundToInt(progressBar.value * 100) + "%";

                Debug.Log("Loading Progress: " + progressText.text);
            }

            // �� ��ȯ
            if (operation.progress >= 0.9f && progressBar.value >= 0.99f)
            {
                Debug.Log("Loading Complete!");
                yield return new WaitForSeconds(1f);
                operation.allowSceneActivation = true;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

}
