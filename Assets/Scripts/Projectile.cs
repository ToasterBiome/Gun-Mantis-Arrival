using System.Data.Common;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Projectile : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float damageDropoff;
    [SerializeField] Vector3 velocity;
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] int layerMask;

    [SerializeField] AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += velocity * Time.deltaTime;
    }

    public void SetProjectileValues(float dam, float radius, Vector3 vel, int ignoreLayer)
    {
        damage = dam;
        damageDropoff = radius;
        velocity = vel;
        layerMask = ignoreLayer;
    }

    void Explode()
    {
        if (audioSource.clip != null) audioSource.Play();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageDropoff);
        foreach (var hitCollider in hitColliders)
        {
            IDamageable damagable = hitCollider.GetComponent<IDamageable>();
            if (damagable != null)
            {
                if (hitCollider.gameObject.layer == layerMask)
                {
                    damagable.Damage(damage * 0.25f);
                }
                else
                {
                    damagable.Damage(damage);
                }

            }
        }

        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 1f);
        }

        Destroy(gameObject);

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != layerMask)
        {
            Explode();
        }
    }

}
