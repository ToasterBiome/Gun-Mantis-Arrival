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
    float maxShootCooldown;

    [SerializeField]
    GameObject spawnCube;
    // Start is called before the first frame update
    void Start()
    {
        shootCooldown = maxShootCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        shootCooldown -= Time.deltaTime;
        if (Input.GetButton("Fire1") && shootCooldown <= 0)
        {
            shootCooldown = maxShootCooldown;
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
}
