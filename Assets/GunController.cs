using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField]
    Animator gunAnimator;

    [SerializeField]
    float shootCooldown;

    [SerializeField]
    GameObject spawnCube;

    [SerializeField]
    GameObject gunModel;

    [SerializeField]
    Transform handTransform;

    [SerializeField]
    Weapon currentWeapon;

    [SerializeField]
    Weapon debugWeapon;
    // Start is called before the first frame update
    void Start()
    {
        SwitchWeapon(debugWeapon);
        shootCooldown = currentWeapon.shotCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWeapon == null) return;

        shootCooldown -= Time.deltaTime;
        if (Input.GetButton("Fire1") && shootCooldown <= 0)
        {
            shootCooldown = currentWeapon.shotCooldown;
            FireGun();
        }
    }

    void FireGun()
    {
        gunAnimator.SetTrigger("Shoot");
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
        {
            Debug.Log(hit.transform.name);
            Instantiate(spawnCube, hit.point, Quaternion.identity);
            if (hit.transform.name == "Square")
            {
                Destroy(hit.transform.gameObject);
            }
        }
    }

    void SwitchWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
        if (gunModel != null)
        {
            Destroy(gunModel);
        }
        GameObject newGunModel = Instantiate(weapon.model, handTransform);
    }
}
