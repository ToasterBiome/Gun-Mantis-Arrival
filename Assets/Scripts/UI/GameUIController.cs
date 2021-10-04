using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ammoCountText;
    [SerializeField] TextMeshProUGUI weaponNameText;
    [SerializeField] TextMeshProUGUI reloadText;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] Image fadeImage;
    [SerializeField] TextMeshProUGUI winText;
    [SerializeField] Button backButton;
    [SerializeField] Button restartButton;

    [SerializeField] Color healthAlertColor;
    [SerializeField] Color healthColor;
    [SerializeField] Color waveAlertColor;
    [SerializeField] Color waveColor;

    void OnEnable()
    {
        GunController.OnAmmoCountChange += OnAmmoCountChange;
        GunController.OnWeaponChange += OnWeaponChange;
        PlayerManager.OnHealthChanged += OnHealthChanged;
        WorldManager.OnWaveChanged += OnWaveChanged;
        EndPortal.OnEndGame += OnGameEnd;
        backButton.onClick.AddListener(BackToMenu);
        restartButton.onClick.AddListener(RestartGame);
    }

    void OnDisable()
    {
        GunController.OnAmmoCountChange -= OnAmmoCountChange;
        GunController.OnWeaponChange -= OnWeaponChange;
        PlayerManager.OnHealthChanged -= OnHealthChanged;
        WorldManager.OnWaveChanged -= OnWaveChanged;
        EndPortal.OnEndGame -= OnGameEnd;
    }


    // Start is called before the first frame update
    void Start()
    {
        fadeImage.color = Color.black;
        LeanTween.alpha(fadeImage.rectTransform, 0f, 1f);
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnAmmoCountChange(int currentAmmo, int maxAmmo)
    {
        ammoCountText.SetText(currentAmmo.ToString("D2") + "/" + maxAmmo.ToString("D2"));
        if (currentAmmo <= 0)
        {
            reloadText.SetText("Press R to reload!");
        }
    }

    void OnWeaponChange(Weapon weapon)
    {
        OnAmmoCountChange(weapon.maxAmmo, weapon.maxAmmo);
        weaponNameText.SetText(weapon.name);
        reloadText.SetText("");
    }

    void OnHealthChanged(float health)
    {
        healthText.SetText("Health: " + health.ToString("F1") + "%");
        healthText.color = healthAlertColor;
        LeanTween.textColor(healthText.rectTransform, healthColor, 1f);
    }

    void OnWaveChanged(int waveNumber)
    {
        waveText.SetText("Wave: " + waveNumber.ToString("D2"));
        waveText.color = waveAlertColor;
        LeanTween.textColor(waveText.rectTransform, waveColor, 1f);
    }

    void OnGameEnd(bool win)
    {
        LeanTween.alpha(fadeImage.rectTransform, 1f, 1f).setOnComplete(() =>
            {
                Cursor.lockState = CursorLockMode.None;
                winText.gameObject.SetActive(true);
                backButton.gameObject.SetActive(true);
                restartButton.gameObject.SetActive(true);
                if (win)
                {
                    winText.SetText("You prevailed against the other aliens");
                }
                else
                {
                    winText.SetText("You seize up and fall limp, your eyes dead and lifeless...");
                }
            });
    }

    void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void RestartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
