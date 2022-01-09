using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class LoadingSceneManager : MonoBehaviour
{
    static string nextScene;

    [SerializeField]
    Image loadingBar;
    public TextMeshProUGUI loadingText;

    void Start()
    {
        StartCoroutine(LoadSceneProcess());
      
    }
    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("Loading");
    }
    IEnumerator LoadSceneProcess()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0f;
        while (!op.isDone)
        {
            yield return null;

            if (op.progress < 0.6f)
            {
                loadingBar.fillAmount = op.progress;
            }
            else
            {
                timer += (Time.unscaledDeltaTime * 0.6f);
                loadingBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);
                if(loadingBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
            loadingText.text = "Loading..." + ((int)(loadingBar.fillAmount * 100)).ToString() + "%";
        }
    }

   
}
