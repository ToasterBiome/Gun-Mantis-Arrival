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

    void OnEnable()
    {
        GunController.OnAmmoCountChange += OnAmmoCountChange;
        GunController.OnWeaponChange += OnWeaponChange;
        PlayerManager.OnHealthChanged += OnHealthChanged;
        WorldManager.OnWaveChanged += OnWaveChanged;
        EndPortal.OnEndGame += OnGameEnd;
        backButton.onClick.AddListener(BackToMenu);
    }

    void OnDisable()
    {
        GunController.OnAmmoCountChange -= OnAmmoCountChange;
        GunController.OnWeaponChange -= OnWeaponChange;
        PlayerManager.OnHealthChanged -= OnHealthChanged;
        WorldManager.OnWaveChanged -= OnWaveChanged;
        EndPortal.OnEndGame -= OnGameEnd;
        backButton.onClick.RemoveAllListeners();
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
    }

    void OnWaveChanged(int waveNumber)
    {
        waveText.SetText("Wave: " + waveNumber.ToString("D2"));
    }

    void OnGameEnd()
    {
        LeanTween.alpha(fadeImage.rectTransform, 1f, 1f).setOnComplete(() =>
            {
                winText.gameObject.SetActive(true);
                backButton.gameObject.SetActive(true);
                winText.SetText("You win!");
            });
    }

    void BackToMenu()
    {
        //make MainMenu do something
        //SceneManager.LoadScene("MainMenu");
    }
}
