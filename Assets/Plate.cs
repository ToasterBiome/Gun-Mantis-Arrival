using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    [SerializeField] bool isDropping = false;
    [SerializeField] float velocity;
    [SerializeField] float maxVelocity;
    [SerializeField] float verticalCutoff;
    [SerializeField] WorldManager worldManager;

    public void StartDrop()
    {
        isDropping = true;
    }

    void Update()
    {
        if (isDropping)
        {
            if (velocity < maxVelocity)
            {
                velocity -= Time.deltaTime;
                if (velocity > maxVelocity)
                {
                    velocity = maxVelocity;
                }
            }
            transform.position += new Vector3(0, velocity * Time.deltaTime, 0);
            if (transform.position.y < verticalCutoff)
            {
                Destroy(gameObject);
            }
        }
    }
}
