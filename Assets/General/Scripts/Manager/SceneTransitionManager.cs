using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager instance;
    public Animator animator;

    /// <summary>
    /// Delay before fade in the Transition screen
    /// </summary>
    [Range(0f, 10f)]
    public float fadeInDelay = .5f;

    /// <summary>
    /// /// Delay before fade out the Transition screen
    /// </summary>
    [Range(0f, 10f)]
    public float fadeOutDelay = .5f;

    /// <summary>
    /// Event to execute when the fade in start
    /// </summary>
    public UnityEvent onFadeIn;

    /// <summary>
    /// Event to execute when the fade in end
    /// </summary>
    public UnityEvent onFadeOut;

    private bool eventsAdded = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void OnEnable()
    {
        if (eventsAdded) return;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (this == null) return;

        Invoke("FadeOut", fadeOutDelay);
    }

    public void SwitchScene(int sceneIndex)
    {
        FadeIn();
        StartCoroutine(SwitchSceneIndexCoroutine(sceneIndex));
    }

    public void SwitchScene(string sceneName)
    {
        FadeIn();
        StartCoroutine(SwitchSceneNameCoroutine(sceneName));
    }

    public void RestartScene()
    {
        FadeIn();
        StartCoroutine(RestartSceneCoroutine());
    }

    public void FadeIn()
    {
        animator.gameObject.SetActive(true);
        animator.SetInteger("Fade", 1);
        if (onFadeIn.GetPersistentEventCount() > 0) onFadeIn.Invoke();
    }

    public void FadeOut()
    {
        if (this == null) return;

        animator.SetInteger("Fade", 0);
        if (onFadeOut.GetPersistentEventCount() > 0) onFadeOut.Invoke();
    }

    private IEnumerator SwitchSceneNameCoroutine(string sceneName)
    {
        // Debug.Log(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        yield return new WaitForSeconds(fadeInDelay + animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator SwitchSceneIndexCoroutine(int sceneIndex)
    {
        //     Debug.Log(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        yield return new WaitForSeconds(fadeInDelay + animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        SceneManager.LoadScene(sceneIndex);
    }

    private IEnumerator RestartSceneCoroutine()
    {
        Debug.Log(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        yield return new WaitForSeconds(fadeInDelay + animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}