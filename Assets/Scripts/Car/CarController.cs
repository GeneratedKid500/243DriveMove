using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent (typeof (GetOutOfCar))]
public class CarController : MonoBehaviour
{
    public Rigidbody rb;
    public bool isDriven = true;

    [Header("Texture")]
    public Material[] carMats;

    [Header("Physics and Acceleration")]
    public float forwardAcc = 8f;
    public float reverseAcc = 4f;
    public float turnStr = 180f;
    public float gravityForce = 10f, maxFallSpeed = 40f;
    [HideInInspector] public float speedInput; 
    private float turnInput, dragGround;

    [Header ("Grounded Checks")]
    public bool isGrounded;
    public LayerMask ground;
    public float groundRayLength;
    public GameObject rayPointParent;
    Transform[] rayPoints;

    BoxCollider[] vehicleColliders;
    
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<Renderer>().material = carMats[Random.Range(0, carMats.Length)];
        vehicleColliders = GetComponentsInChildren<BoxCollider>();
        rayPoints = rayPointParent.GetComponentsInChildren<Transform>();
        
        rb.GetComponentInChildren<Rigidbody>();
        dragGround = rb.drag;
        rb.transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        speedInput = 0f;
        if (isDriven)
        {
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                speedInput = Input.GetAxisRaw("Vertical") * forwardAcc * 1000f;
            }
            else if (Input.GetAxisRaw("Vertical") < 0)
            {
                speedInput = Input.GetAxisRaw("Vertical") * reverseAcc * 1000f;
            }
        }


        turnInput = Input.GetAxisRaw("Horizontal");

        if (isGrounded && isDriven) transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, turnInput * turnStr * Time.deltaTime * Input.GetAxisRaw("Vertical"), 0));

        transform.position = rb.transform.position;
    }

    void FixedUpdate()
    {
        foreach (Collider collider in vehicleColliders)
        {
            Physics.IgnoreCollision(rb.GetComponent<SphereCollider>(), collider);
        }

        isGrounded = GroundCheck();

        if (isGrounded)
        {
            rb.drag = dragGround;
            if (Mathf.Abs(speedInput) > 0)
            {
                rb.AddForce(transform.forward * speedInput);
            }
        }
        else
        {
            rb.drag = 0.1f;
            if (Mathf.Abs(rb.velocity.y) < maxFallSpeed)
                rb.AddForce(Vector3.up * -gravityForce * 1000f);
        }
    }

    bool GroundCheck()
    {
        foreach (Transform point in rayPoints)
        {
            if (Physics.Raycast(point.position, -transform.up, out RaycastHit hit, groundRayLength, ground))
            {
                transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

                return true;
            }
        }
        return false;
    }
}
