using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] Button playButton;
    [SerializeField] Button creditsButton;
    [SerializeField] Button exitButton;
    [SerializeField] Button creditsExit;
    [SerializeField] CanvasGroup credits;
    [SerializeField] Image fadeImage;

    void OnEnable()
    {
        playButton.onClick.AddListener(PlayGame);
        creditsButton.onClick.AddListener(() =>
        {
            ShowCredits(credits.alpha < 1 ? true : false);
        });
        exitButton.onClick.AddListener(ExitGame);
        creditsExit.onClick.AddListener(() =>
        {
            ShowCredits(false);
        });
    }

    // Start is called before the first frame update
    void Start()
    {
        fadeImage.color = Color.black;
        LeanTween.alpha(fadeImage.rectTransform, 0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void PlayGame()
    {
        LeanTween.alpha(fadeImage.rectTransform, 1f, 1f).setOnComplete(() =>
            {
                SceneManager.LoadScene("GameScene");
            });
    }

    void ShowCredits(bool show)
    {
        if (show)
        {
            LeanTween.alphaCanvas(credits, 1f, 0.5f);
        }
        else
        {
            LeanTween.alphaCanvas(credits, 0f, 0.5f);
        }
    }

    void ExitGame()
    {
        Application.Quit();
    }
}
