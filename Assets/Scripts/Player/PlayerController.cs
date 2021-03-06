using Microsoft.CSharp.RuntimeBinder;
using System.Dynamic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;

    [SerializeField]
    Camera mainCamera;

    [SerializeField]
    float cameraRotation;

    [SerializeField]
    float maxSpeed = 8f;

    [SerializeField]
    Vector3 moveDirection;

    [SerializeField]
    float lookRotation;

    [SerializeField]
    float jumpHeight = 4f;

    [SerializeField]
    CharacterController controller;

    [SerializeField]
    Vector3 horizontalMovement;

    [SerializeField]
    Vector3 verticalMovement;
    [SerializeField] bool frozen = false;

    [SerializeField] bool isGrounded = false;
    [SerializeField] Transform badassCape;
    void OnEnable()
    {
        EndPortal.OnEndGame += OnGameEnd;
    }

    void OnDisable()
    {
        EndPortal.OnEndGame -= OnGameEnd;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        cameraRotation = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (frozen) return;
        //camera
        lookRotation += Input.GetAxis("Mouse X") * 2f;
        float mouseY = Input.GetAxis("Mouse Y") * 2f;


        cameraRotation -= mouseY;
        cameraRotation = Mathf.Clamp(cameraRotation, -90, 90);



        var xQuat = Quaternion.AngleAxis(lookRotation, Vector3.up);
        transform.localRotation = xQuat;

        //movement
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        horizontalMovement = transform.right * h + transform.forward * v;

        if (verticalMovement.y > Physics.gravity.y)
        {
            verticalMovement += Physics.gravity * Time.deltaTime;
        }

        //jumping
        if (IsGrounded() && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("jump");
            verticalMovement.y = jumpHeight;
        }

        controller.Move(horizontalMovement * maxSpeed * Time.deltaTime + verticalMovement * Time.deltaTime);

    }

    void LateUpdate()
    {
        mainCamera.transform.position = transform.position + new Vector3(0, 0.5f);
        mainCamera.transform.localRotation = Quaternion.Euler(cameraRotation, transform.eulerAngles.y, 0);
    }

    float GetHorizontalMagnitude()
    {
        return new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
    }

    bool IsGrounded()
    {
        isGrounded = controller.isGrounded;
        return controller.isGrounded;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, (-Vector3.up * 1.05f));
    }

    void OnGameEnd(bool win)
    {
        //freeze us so we cant fall to our dooms
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        frozen = true;
    }
}
