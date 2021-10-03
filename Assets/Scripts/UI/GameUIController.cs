using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ammoCountText;
    [SerializeField] TextMeshProUGUI weaponNameText;
    [SerializeField] TextMeshProUGUI reloadText;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI waveText;


    void OnEnable()
    {
        GunController.OnAmmoCountChange += OnAmmoCountChange;
        GunController.OnWeaponChange += OnWeaponChange;
        PlayerManager.OnHealthChanged += OnHealthChanged;
        WorldManager.OnWaveChanged += OnWaveChanged;
    }

    void OnDisable()
    {
        GunController.OnAmmoCountChange -= OnAmmoCountChange;
        GunController.OnWeaponChange -= OnWeaponChange;
        PlayerManager.OnHealthChanged -= OnHealthChanged;
        WorldManager.OnWaveChanged -= OnWaveChanged;
    }


    // Start is called before the first frame update
    void Start()
    {

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
}
