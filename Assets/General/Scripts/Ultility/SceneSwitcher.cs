using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Change current scene to selected scene index/name
/// Tips: Attach to a gameObject, drag SwitchScene() function into an UnityEvent and type in scene index/scene name
/// Notes: You can also call SwitchScene() via script
/// </summary>
public class SceneSwitcher : MonoBehaviour {

    public void SwitchScene(int sceneNumber)
    {
       // Debug.Log("switch to scene " + sceneNumber);
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneNumber);

        //SceneTransitionManager.instance.SwitchScene(sceneNumber);
        // 0 HOME
        // 1 GAME
        // 2 SCORE
    }

    public void SwitchScene(string sceneName)
    {
       // Debug.Log("switch to scene " + sceneName);
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);

        //SceneTransitionManager.instance.SwitchScene(sceneName);
        // 0 HOME
        // 1 GAME
        // 2 SCORE
    }
    public void RestartScene()
    {
      //  Debug.Log("restarting scene");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //SceneTransitionManager.instance.RestartScene();
    }
}
