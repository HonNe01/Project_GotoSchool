using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    private AsyncOperation operation;


    public Slider progressBar; // 진행도 슬라이더
    public TextMeshProUGUI progressText;  // 진행도 텍스트
    

    void Start()
    {
        // 다음 씬 로드
        StartCoroutine(LoadScene("Stage"));
    }

    IEnumerator LoadScene(string sceneName)
    {
        // 씬 비동기 로드
        operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false; // 자동 씬 전환 해제

        float timer = 0f;

        // 진행도 업데이트
        while (!operation.isDone)
        {
            // 로딩 진행도는 0~0.9 사이 (로딩이 90%에서 완료됨)
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            timer += Time.deltaTime;
            progressBar.value = Mathf.Lerp(progressBar.value, progress, timer);

            
            if (progressText != null)
            {
                progressText.text = Mathf.RoundToInt(progressBar.value * 100) + "%";

                Debug.Log("Loading Progress: " + progressText.text);
            }

            // 씬 전환
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
