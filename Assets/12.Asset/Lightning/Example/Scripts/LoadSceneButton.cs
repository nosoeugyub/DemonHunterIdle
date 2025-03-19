using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    public void LoadPrevious(Object scene)
    {
        if (scene != null)
        {
            SceneManager.LoadScene(scene.name);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
    }

    public void LoadNext(Object scene)
    {
        if (scene != null)
        {
            SceneManager.LoadScene(scene.name);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
