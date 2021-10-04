using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip activateClip;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        //make it so others can get it eventually!?!?!?!?
        PlayerManager player = other.GetComponent<PlayerManager>();
        if (player != null)
        {
            audioSource.clip = activateClip;
            audioSource.Play();
            GetComponent<SphereCollider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
            transform.GetChild(0).gameObject.SetActive(false);
            Activate(player);
            Destroy(gameObject, 2f);
            this.enabled = false;
        }
    }

    protected virtual void Activate(PlayerManager player) { }
}
