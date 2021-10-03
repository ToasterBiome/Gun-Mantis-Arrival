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
    }

    void FireGun()
    {
        gunAnimator.SetTrigger("Shoot");
        muzzleFlashParticles.Play();
        for (int shots = 0; shots < currentWeapon.shotAmount; shots++)
        {
            RaycastHit hit;
            Vector3 shotAngle = Camera.main.transform.forward + ((Vector3)UnityEngine.Random.insideUnitCircle * currentWeapon.shotSpread);
            if (Physics.Raycast(Camera.main.transform.position, shotAngle, out hit))
            {
                Debug.Log(hit.transform.name);
                IDamageable damageable = hit.transform.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.Damage(currentWeapon.damage);
                }
                GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 1f);
                //Instantiate(spawnCube, hit.point, Quaternion.identity);
            }
        }
    }

    void SwitchWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
        if (gunModel != null)
        {

        }
        gunModel = Instantiate(weapon.model, handTransform);
        gunModel.layer = 6;
        currentAmmo = currentWeapon.maxAmmo;
        shootCooldown = 0;
        muzzleFlashParticles.transform.localPosition = currentWeapon.muzzleFlashPoint;
        OnWeaponChange?.Invoke(currentWeapon);

    }

    IEnumerator Reload()
    {
        gunAnimator.SetTrigger("Reload");
        yield return new WaitForSeconds(0.2f);
        if (gunModel != null)
        {
            gunModel.transform.SetParent(null);
            SphereCollider collider = gunModel.AddComponent<SphereCollider>();
            collider.center = Vector3.zero;
            collider.radius = 2f;
            Rigidbody gunRB = gunModel.AddComponent<Rigidbody>();
            gunModel.layer = 3;
            gunRB.velocity = transform.up * 8f;
            gunRB.angularVelocity = new Vector3(-360f * Mathf.Deg2Rad, 0, 0);
            Destroy(gunModel.gameObject, 1.8f);
        }

        yield return new WaitForSeconds(0.8f);
        Weapon newWeapon = possibleWeapons[UnityEngine.Random.Range(0, possibleWeapons.Count)];
        SwitchWeapon(newWeapon);
        isSoftReloading = false;
        yield return new WaitForSeconds(1f);
        isReloading = false;
        yield return null;
    }
}
