using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float fadeInSpeed = 0.25f;
    [SerializeField] float fadeOutSpeed = 0.25f;

    [Header("References")]
    [SerializeField] Image background;


    public void Start()
    {
        background.DOFade(1, 0);
        FadeIn();
    }

    public void LoadMainMenu()
    {
        StartCoroutine(LoadSceneRoutine("MainMenu"));
    }

    public void LoadGameScene()
    {
        StartCoroutine(LoadSceneRoutine("MainScene"));
    }

    IEnumerator LoadSceneRoutine(string sceneToLoad)
    {
        FadeOut();
        yield return new WaitForSecondsRealtime(fadeOutSpeed);
        SceneManager.LoadScene(sceneToLoad);
    }

    public void FadeIn()
    {
        background.DOFade(0, fadeInSpeed);
    }

    public void FadeOut()
    {
        background.DOFade(1, fadeOutSpeed);
    }
}
