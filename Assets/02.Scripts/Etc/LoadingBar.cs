using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingBar : MonoSingleton<LoadingBar>
{
    public static string nextScene;
    [SerializeField] Image progressBar;

  
    public IEnumerator LoadScene(string _scenename)
    {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(_scenename);
        op.allowSceneActivation = false;
        float timer = 0.0f;
        bool isSucceced = false;
        while (!op.isDone)
        {
            
            yield return null;
            timer += Time.deltaTime / 10;
            if (op.progress < 0.9f)
            {
               

                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                if (progressBar.fillAmount >= op.progress)
                {
                    timer = 0f;

                    
                }
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                if (progressBar.fillAmount >= 1.0f )
                {
                        op.allowSceneActivation = true;
                       // SceneManager.LoadScene(_scenename);
                        yield break;
                }
            }
        }
    }
  
}
