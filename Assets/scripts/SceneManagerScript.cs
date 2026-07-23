using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneManagerScript : MonoBehaviour
{
    public static SceneManagerScript instance;
    
    void Awake() => instance = this;

    public Image fadeImage;

    public GameObject howtoplay;

    void Start()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float t=0;
        Color c = fadeImage.color;
        while(t<1)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1, 0, t);
            fadeImage.color = c;
            yield return null;
        }
        fadeImage.gameObject.SetActive(false);

    }

    public void StartGame()
    {
        StartCoroutine(StartC());
    }

    IEnumerator StartC()
    {
        fadeImage.gameObject.SetActive(true);
        float t=0;
        Color c = fadeImage.color;
        while(t<1)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0, 1, t);
            fadeImage.color = c;
            yield return null;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void WinGame()
    {
        StartCoroutine(WinCoroutine());
    }

    IEnumerator WinCoroutine()
    {
        fadeImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        float t=0;
        Color c = fadeImage.color;
        while(t<1)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0, 1, t);
            fadeImage.color = c;
            yield return null;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Menu()
    {
        SceneManager.LoadScene(0);
    }

    public void HowToPlay()
    {
        howtoplay.SetActive(howtoplay.activeSelf ^ true);
    }
}
