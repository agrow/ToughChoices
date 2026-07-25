using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnvironmentSceneLoader : MonoBehaviour
{
    [SerializeField]
    private string environmentSceneName = "OutdoorHighSchoolScene";

    private IEnumerator Start()
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