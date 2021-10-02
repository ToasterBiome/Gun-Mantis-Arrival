using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField]
    Quaternion rotation;

    [SerializeField]
    Vector3 direction;

    [SerializeField]
    Transform spriteTransform;




    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        direction = (new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z) - transform.position).normalized;
        rotation = Quaternion.LookRotation(direction);
        spriteTransform.rotation = rotation;
    }
}
