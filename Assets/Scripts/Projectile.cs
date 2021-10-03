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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += velocity * Time.deltaTime;
    }

    void Explode()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damage);
        foreach (var hitCollider in hitColliders)
        {
            IDamageable damagable = hitCollider.GetComponent<IDamageable>();
            if (damagable != null)
            {
                damagable.Damage(damage);
            }
        }

        Destroy(gameObject);

    }

    void OnTriggerEnter(Collider other)
    {
        Explode();
    }

}
