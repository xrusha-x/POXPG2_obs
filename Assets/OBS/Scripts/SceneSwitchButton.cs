using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitchButton : MonoBehaviour
{
    [Header("Scene Switching")]
    [Tooltip("Exact scene name as listed in Build Settings.")]
    public string targetScene;

    [Tooltip("Load scene asynchronously to avoid UI hitches.")]
    public bool useAsyncLoad = true;

    public void Switch()
    {
        if (string.IsNullOrEmpty(targetScene))
        {
            return;
        }
        if (!SceneExists(targetScene))
        {
            return;
        }

        if (useAsyncLoad)
        {
            var op = SceneManager.LoadSceneAsync(targetScene);
            op.allowSceneActivation = true;
        }
        else
        {
            SceneManager.LoadScene(targetScene);
        }
    }

    public void SwitchTo(string sceneName)
    {
        targetScene = sceneName;
        Switch();
    }

    public void SwitchToIndex(int buildIndex)
    {
        if (buildIndex < 0 || buildIndex >= SceneManager.sceneCountInBuildSettings)
        {
            return;
        }

        if (useAsyncLoad)
        {
            var op = SceneManager.LoadSceneAsync(buildIndex);
            op.allowSceneActivation = true;
        }
        else
        {
            SceneManager.LoadScene(buildIndex);
        }
    }

    bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == sceneName) return true;
        }
        return false;
    }
}
