using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnvironmentSceneLoader : MonoBehaviour
{
    [SerializeField]
    private string environmentSceneName = "OutdoorHighSchoolScene";

    public void LoadEnvironment()
    {
        StartCoroutine(LoadEnvironmentRoutine());
    }

    private IEnumerator LoadEnvironmentRoutine()
    {
        Scene environmentScene =
            SceneManager.GetSceneByName(environmentSceneName);

        if (environmentScene.isLoaded)
        {
            yield break;
        }

        AsyncOperation loadOperation =
            SceneManager.LoadSceneAsync(
                environmentSceneName,
                LoadSceneMode.Additive
            );

        while (!loadOperation.isDone)
        {
            yield return null;
        }

        Debug.Log(
            $"Loaded {environmentSceneName} additively."
        );
    }
}