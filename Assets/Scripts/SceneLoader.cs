using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static void LoadScene(string sceneName)
    {
        //Reset Pause Values
        AudioListener.pause = false;
        Time.timeScale = 1f;

        //load scene
        SceneManager.LoadScene(sceneName);
    }

    public static void LoadSceneByIndex(int buildIndex)
    {
        //Reset Pause Values
        AudioListener.pause = false;
        Time.timeScale = 1f;

        //load scene
        SceneManager.LoadScene(SceneManager.GetSceneByBuildIndex(buildIndex).name);
    }

    public static void ReloadActiveScene()
    {
        //Reset Pause Values
        AudioListener.pause = false;
        Time.timeScale = 1f;

        //reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}