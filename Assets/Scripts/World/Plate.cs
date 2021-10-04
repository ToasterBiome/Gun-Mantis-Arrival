using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    [SerializeField] bool isMoving = false;
    [SerializeField] float velocity;
    [SerializeField] float maxVelocity;
    [SerializeField] float verticalCutoff;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip releaseClip;

    public void StartDrop(float direction)
    {
        velocity = 0;
        maxVelocity = direction;
        isMoving = true;
        audioSource = GetComponent<AudioSource>();
        if (direction < 0)
        {
            audioSource.clip = releaseClip;
            audioSource.Play();
        }
    }

    public void TelegraphDrop()
    {
        audioSource.Play();
    }

    void Update()
    {
        if (isMoving)
        {
            if (Mathf.Abs(velocity) < Mathf.Abs(maxVelocity))
            {
                velocity += Time.deltaTime;
                if (velocity > maxVelocity)
                {
                    velocity = maxVelocity;
                }
            }
            transform.position += new Vector3(0, velocity * Time.deltaTime, 0);
            if (transform.position.y < verticalCutoff && Mathf.Sign(maxVelocity) == -1)
            {
                Destroy(gameObject);
            }
            if (transform.position.y >= 0 && Mathf.Sign(maxVelocity) == 1)
            {
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                isMoving = false;
            }
        }
    }
}
