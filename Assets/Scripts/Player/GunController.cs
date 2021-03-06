using Microsoft.CSharp.RuntimeBinder;
using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public static Action<int, int> OnAmmoCountChange;
    public static Action<Weapon> OnWeaponChange;
    [SerializeField] Animator gunAnimator;
    [SerializeField] float shootCooldown;
    [SerializeField] GameObject spawnCube;
    [SerializeField] GameObject gunModel;
    [SerializeField] Transform handTransform;
    [SerializeField] Weapon currentWeapon;
    [SerializeField] Weapon debugWeapon;
    [SerializeField] ParticleSystem muzzleFlashParticles;
    [SerializeField] int currentAmmo;
    [SerializeField] bool isReloading;
    [SerializeField] bool isSoftReloading;
    [SerializeField] List<Weapon> possibleWeapons;
    [SerializeField] GameObject impactEffect;
    [SerializeField] GameObject rocketPrefab;
    [SerializeField] Weapon weaponOverride;
    [SerializeField] AudioSource fireSound;
    [SerializeField] AudioSource reloadSound;
    [SerializeField] bool superActivated = false;
    [SerializeField] Material superMaterial;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Reload());
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWeapon == null) return;

        shootCooldown -= Time.deltaTime;
        //bonus if you're super
        if (superActivated)
        {
            shootCooldown -= Time.deltaTime;
        }
        if (Input.GetButton("Fire1") && shootCooldown <= 0 && currentAmmo > 0 && !isSoftReloading)
        {
            currentAmmo--;
            OnAmmoCountChange?.Invoke(currentAmmo, currentWeapon.maxAmmo);
            shootCooldown = currentWeapon.shotCooldown;
            FireGun();
        }

        if (currentAmmo <= 0 && Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            isReloading = true;
            isSoftReloading = true;
            StartCoroutine(Reload());
        }
        else if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            gunAnimator.SetTrigger("Flip");
        }
        else if (Input.GetKeyDown(KeyCode.F) && !isReloading)
        {
            gunAnimator.SetTrigger("Inspect");
        }
    }

    void FireGun()
    {
        gunAnimator.SetTrigger("Shoot");
        muzzleFlashParticles.Play();
        fireSound.Play();
        if (currentWeapon.name == "Rocket Launcher")
        {
            RocketFire();
            return;
        }
        for (int shots = 0; shots < currentWeapon.shotAmount; shots++)
        {
            RaycastHit hit;
            Vector3 shotAngle = Camera.main.transform.forward + new Vector3(UnityEngine.Random.Range(-currentWeapon.shotSpread, currentWeapon.shotSpread), UnityEngine.Random.Range(-currentWeapon.shotSpread, currentWeapon.shotSpread), UnityEngine.Random.Range(-currentWeapon.shotSpread, currentWeapon.shotSpread));
            if (Physics.Raycast(Camera.main.transform.position, shotAngle, out hit))
            {
                Debug.Log(hit.transform.name);
                IDamageable damageable = hit.transform.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    if (superActivated)
                    {
                        damageable.Damage(currentWeapon.damage * 2f);
                    }
                    else
                    {
                        damageable.Damage(currentWeapon.damage);
                    }

                }
                GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 1f);
                //Instantiate(spawnCube, hit.point, Quaternion.identity);
            }
        }
    }

    void RocketFire()
    {
        GameObject rocket = Instantiate(rocketPrefab, transform.position + Camera.main.transform.forward, Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up));
        Projectile projectile = rocket.GetComponent<Projectile>();
        projectile.SetProjectileValues(currentWeapon.damage, currentWeapon.shotSpread, Camera.main.transform.forward.normalized * 64f, 6);
        //gunModel.transform.GetChild(0).gameObject.SetActive(false);
    }

    void SwitchWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
        gunModel = Instantiate(weapon.model, handTransform);
        gunModel.layer = 3;
        fireSound.clip = currentWeapon.fireSound;
        reloadSound.clip = currentWeapon.reloadSound;
        currentAmmo = currentWeapon.maxAmmo;
        shootCooldown = 0;
        muzzleFlashParticles.transform.localPosition = currentWeapon.muzzleFlashPoint;
        reloadSound.Play();
        OnWeaponChange?.Invoke(currentWeapon);

    }

    IEnumerator Reload()
    {
        gunAnimator.SetTrigger("Reload");
        if (gunModel != null)
        {
            /*
            gunModel.transform.SetParent(null);
            Rigidbody gunRB = gunModel.AddComponent<Rigidbody>();
            gunModel.layer = 3;
            gunRB.velocity = transform.up * 8f;
            gunRB.angularVelocity = new Vector3(-360f * Mathf.Deg2Rad, 0, 0);
            */
            MeshRenderer renderer = gunModel.GetComponent<MeshRenderer>();
            Material gunMaterial = renderer.material;
            LeanTween.value(gunModel, 0f, 1f, 0.66f).setOnUpdate((value) =>
            {
                gunMaterial.SetFloat("_DissolveAmount", value);
            });
            Destroy(gunModel.gameObject, 0.66f);
        }

        yield return new WaitForSeconds(1f);
        Weapon newWeapon = null;
        if (weaponOverride != null)
        {
            newWeapon = weaponOverride;
        }
        else
        {
            newWeapon = possibleWeapons[UnityEngine.Random.Range(0, possibleWeapons.Count)];
        }
        if (superActivated)
        {
            superActivated = false;
        }
        SwitchWeapon(newWeapon);
        isSoftReloading = false;
        yield return new WaitForSeconds(1f);
        isReloading = false;
        yield return null;
    }
    public void ActivateSuper()
    {
        superActivated = true;
        MeshRenderer renderer = gunModel.GetComponent<MeshRenderer>();
        renderer.material = superMaterial;
        currentAmmo = currentWeapon.maxAmmo * 2;
        OnAmmoCountChange?.Invoke(currentAmmo, currentWeapon.maxAmmo);
    }
}
